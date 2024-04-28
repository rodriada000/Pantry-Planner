using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlannerCore.Models;
using PantryPlanner.Services;
using PantryPlanner.Extensions;
using Microsoft.AspNetCore.Authorization;
using PantryPlannerCore.Services;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class RecipeScrapeController : ControllerBase
    {
        private readonly RecipeScrapeService _webScrapeService;


        public RecipeScrapeController(RecipeScrapeService webScrapeService)
        {
            _webScrapeService = webScrapeService;
        }

        [HttpGet]
        public async Task<ActionResult<RecipeDto>> ScrapeGenericWebsite(string url)
        {

            try
            {
                RecipeDto r = _webScrapeService.ScrapeGenericWebsite(url);
                return Ok(r);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }

        [HttpGet("AllRecipes")]
        public async Task<ActionResult<RecipeDto>> ScrapeAllRecipesWebsite(string url)
        {

            try
            {
                RecipeDto r = _webScrapeService.ScrapeAllRecipesWebsite(url);
                return Ok(r);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

        }


    }
}
