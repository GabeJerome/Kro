using KroApp.Server.Models;
using KroApp.Server.Models.SeedData;
using KroApp.Server.Models.Users;
using KroApp.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure the database context before setting up Identity
builder.Services.AddDbContext<KroContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("KroDatabase") ?? throw new InvalidOperationException("Connection string 'KroDatabase' not found.")));

// Configure Identity services
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<KroContext>()
    .AddDefaultTokenProviders();

var frontendUrl = builder.Configuration["FrontendUrl"];
if (string.IsNullOrEmpty(frontendUrl))
{
  throw new InvalidOperationException("FrontendUrl must be configured in the app settings.");
}

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend", policy =>
  {
    policy.WithOrigins(frontendUrl)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
  });
});

// Register other services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
  throw new InvalidOperationException("JWT Key must be configured in the app settings or user secrets.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
  x.RequireHttpsMetadata = true;
  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false
  };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  SeedDatabase.Initialize(services, Assembly.GetExecutingAssembly());
}

app.UseCors("AllowFrontend");

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Make sure to call UseAuthentication before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
