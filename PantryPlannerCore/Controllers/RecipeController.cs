﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlannerCore.Models;
using PantryPlanner.Services;
using PantryPlanner.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace PantryPlanner.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;


        public RecipeController(UserManager<PantryPlannerUser> userManager, RecipeService recipeService)
        {
            _service = recipeService;
            _userManager = userManager;
        }

        // GET: api/Recipe
        [HttpGet]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipeByNameAsync(string name)
        {
            List<RecipeDto> recipes = null;

            try
            {
                await Task.Run(() =>
                {
                    recipes = RecipeDto.ToList(_service.GetRecipeByName(name));
                }).ConfigureAwait(false);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok(recipes);
        }


        // GET: api/Recipe/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipeAsync(long id)
        {
            PantryPlannerUser user;
            Recipe recipe;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                recipe = _service.GetRecipeById(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok(new RecipeDto(recipe));
        }

        // PUT: api/Recipe/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRecipeAsync(long id, [FromBody] RecipeDto recipe)
        {
            if (id != recipe.RecipeId)
            {
                return BadRequest($"id {id} does not match the recipe to update");
            }


            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                await _service.UpdateRecipeAsync(recipe, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        // POST: api/Recipe
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> AddRecipeAsync([FromBody] RecipeDto recipe)
        {
            PantryPlannerUser user;
            Recipe addedRecipe;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                addedRecipe = _service.AddRecipe(recipe, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status405MethodNotAllowed, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return StatusCode(StatusCodes.Status201Created, new RecipeDto(addedRecipe));
        }

        // DELETE: api/Recipe/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecipeDto>> DeleteRecipeAsync(long id)
        {
            PantryPlannerUser user;
            Recipe recipe;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);

                recipe = _service.DeleteRecipe(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecipeNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (PermissionsException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok(new RecipeDto(recipe));
        }

    }
}
