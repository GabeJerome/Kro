namespace KroApp.Server.Services
{
  public interface IAuthService
  {
    string GenerateJwtToken(string email);
  }
}
