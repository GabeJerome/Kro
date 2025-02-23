using KroApp.Server.Models.DTOs;
using KroApp.Server.Services.Ingredients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KroApp.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class IngredientsController : ControllerBase
  {
    private readonly IIngredientsService _ingredientsService;

    public IngredientsController(IIngredientsService ingredientsService)
    {
      _ingredientsService = ingredientsService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] IngredientSearchRequest body)
    {
      if (body == null || string.IsNullOrWhiteSpace(body.Query))
      {
        return BadRequest("Invalid search request.");
      }

      var results = await _ingredientsService.SearchIngredients(body);

      if (results == null || !results.Any())
      {
        return NotFound("No ingredients found.");
      }

      return Ok(results);
    }
  }
}
