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
    public class WebScraperController : ControllerBase
    {
        private readonly RecipeService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;
        private readonly BaseWebScrapeService _webScrapeService;


        public WebScraperController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager, BaseWebScrapeService webScrapeService)
        {
            _service = new RecipeService(context);
            _userManager = userManager;
            _webScrapeService = webScrapeService;
        }

        [HttpGet]
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
