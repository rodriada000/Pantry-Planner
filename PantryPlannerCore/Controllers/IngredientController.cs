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
    public class IngredientController : ControllerBase
    {
        private readonly IngredientService _service;
        private readonly UserManager<PantryPlannerUser> _userManager;


        public IngredientController(PantryPlannerContext context, UserManager<PantryPlannerUser> userManager)
        {
            _service = new IngredientService(context);
            _userManager = userManager;
        }

        // GET: api/Ingredient
        [HttpGet]
        public async Task<ActionResult<List<IngredientDto>>> GetIngredientByName(string name)
        {
            List<IngredientDto> ingredients = null;
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                await Task.Run(() =>
                {
                    ingredients = IngredientDto.ToList(_service.GetIngredientByName(name, user.Id));
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

            return ingredients;
        }

        // GET: api/Ingredient/ByCategory
        [HttpGet("ByCategory")]
        public async Task<ActionResult<List<IngredientDto>>> GetIngredientByNameAndCategory(string name, string categoryName)
        {
            List<IngredientDto> ingredients = null;
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            try
            {
                ingredients = IngredientDto.ToList(_service.GetIngredientByNameAndCategory(name, categoryName, user.Id));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (CategoryNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return ingredients;
        }

        // GET: api/Ingredient/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetIngredient(long id)
        {
            PantryPlannerUser user;
            Ingredient ingredient;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                ingredient = _service.GetIngredientById(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (IngredientNotFoundException e)
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

            return new IngredientDto(ingredient);
        }

        // GET: api/Ingredient/Category
        [HttpGet("Category")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllIngredientCategories()
        {
            PantryPlannerUser user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            List<CategoryDto> categories = null;

            try
            {
                categories = CategoryDto.ToList(_service.GetIngredientCategories(user));
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (CategoryNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return categories;
        }


        // PUT: api/Ingredient/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateIngredient(long id, Ingredient ingredient)
        {
            if (id != ingredient.IngredientId)
            {
                return BadRequest($"id {id} does not match the ingredient to update");
            }


            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                _service.UpdateIngredient(ingredient, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
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

        // POST: api/Ingredient
        [HttpPost]
        public async Task<ActionResult<IngredientDto>> AddIngredient(Ingredient ingredient)
        {
            PantryPlannerUser user;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }


            try
            {
                _service.AddIngredient(ingredient, user);
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

            return StatusCode(StatusCodes.Status201Created, new IngredientDto(ingredient));
        }

        // DELETE: api/Ingredient/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IngredientDto>> DeleteIngredient(long id)
        {
            PantryPlannerUser user;
            Ingredient ingredient;

            try
            {
                user = await _userManager.GetUserFromCookieOrJwtAsync(this.User);
            }
            catch (PermissionsException e)
            {
                // this will be thrown if the user is null
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            try
            {
                ingredient = _service.DeleteIngredient(id, user);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (IngredientNotFoundException e)
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

            return Ok(new IngredientDto(ingredient));
        }

    }
}
