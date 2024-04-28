using Microsoft.EntityFrameworkCore;
using PantryPlanner.DTOs;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class RecipeService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        private readonly RecipeIngredientService _recipeIngredientService;
        private readonly RecipeStepService _recipeStepService;


        public RecipeService(PantryPlannerContext context, RecipeIngredientService recipeIngredientService, RecipeStepService recipeStepService)
        {
            Context = context;
            Permissions = new PermissionService(Context);
            _recipeIngredientService = recipeIngredientService;
            _recipeStepService = recipeStepService;
        }


        #region Get Methods

        /// <summary>
        /// Return Recipe for <paramref name="recipeId"/>
        /// </summary>
        public Recipe GetRecipeById(long recipeId, PantryPlannerUser user)
        {
            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            Recipe recipe = GetRecipeById(recipeId);

            if (!recipe.IsPublic.Value && Permissions.UserAddedRecipe(recipe, user) == false)
            {
                throw new PermissionsException("You do not have rights to this recipe");
            }

            return recipe;
        }

        /// <summary>
        /// Return Recipe for <paramref name="recipeId"/>
        /// </summary>
        public Recipe GetRecipeById(long recipeId)
        {
            return Context.Recipes.Where(r => r.RecipeId == recipeId)
                                            .Include(i => i.RecipeIngredients.OrderBy(r => r.SortOrder))
                                            .ThenInclude(i => i.Ingredient)
                                            .Include(i => i.RecipeSteps.OrderBy(r => r.SortOrder))
                                            .Include(i => i.CreatedByUser)
                                            .FirstOrDefault();
        }

        /// <summary>
        /// Return list of public Recipes with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        /// <param name="name"> name to search for </param>
        public List<Recipe> GetRecipeByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            // first check for exact match
            if (Context.Recipes.Any(r => r.Name.ToUpper() == name.ToUpper() && r.IsPublic == true))
            {
                return Context.Recipes.Where(r => r.Name.ToUpper() == name.ToUpper() && r.IsPublic == true)
                                     .Include(i => i.RecipeIngredients)
                                     .Include(i => i.RecipeSteps)
                                     .Include(i => i.CreatedByUser)
                                     .ToList();
            }


            // second check for any matches that have all the words entered
            List<Recipe> recipes = new List<Recipe>();
            List<string> wordsToSearchFor = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var word in wordsToSearchFor)
            {
                recipes.AddRange(Context.Recipes.Where(r => EF.Functions.Like(r.Name, '%' + word + '%') && r.IsPublic == true)
                     .Include(i => i.RecipeIngredients)
                     .Include(i => i.RecipeSteps)
                     .Include(i => i.CreatedByUser)
                     .ToList());
            }

            return recipes;
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds a Recipe to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newRecipe"> recipe to add </param>
        /// <param name="user"> user who is adding recipe </param>
        public Recipe AddRecipe(RecipeDto newRecipe, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipe == null)
            {
                throw new ArgumentNullException(nameof(newRecipe));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            Recipe recipeToAdd = newRecipe.Create();

            var added = AddRecipe(recipeToAdd, user);

            if (newRecipe.Ingredients.Count > 0)
            {
                newRecipe.Ingredients.Where(i => i.IngredientId != 0).ToList().ForEach(i =>
                {
                    i.RecipeId = added.RecipeId;
                    _recipeIngredientService.AddRecipeIngredient(i, user);
                });
            }

            if (newRecipe.Steps.Count > 0)
            {
                newRecipe.Steps.ForEach(s =>
                {
                    s.RecipeId = added.RecipeId;
                    _recipeStepService.AddRecipeStep(s, user);
                });
            }

            return added;
        }

        /// <summary>
        /// Adds a Recipe to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newRecipe"> recipe to add </param>
        /// <param name="user"> user who is adding recipe </param>
        public Recipe AddRecipe(Recipe newRecipe, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newRecipe == null)
            {
                throw new ArgumentNullException(nameof(newRecipe));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate name passed in
            if (String.IsNullOrWhiteSpace(newRecipe.Name))
            {
                throw new InvalidOperationException("Recipe name is required");
            }

            newRecipe.CreatedByUserId = user.Id;
            newRecipe.DateCreated = DateTime.Now;

            Context.Recipes.Add(newRecipe);
            Context.SaveChanges();

            return newRecipe;
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task<bool> UpdateRecipeAsync(RecipeDto recipeDto, PantryPlannerUser userUpdating)
        {
            if (recipeDto == null)
            {
                throw new ArgumentNullException(nameof(recipeDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeExists(recipeDto.RecipeId) == false)
            {
                throw new RecipeNotFoundException(recipeDto.RecipeId);
            }

            if (Permissions.UserAddedRecipe(recipeDto.RecipeId, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            Recipe recipeToUpdate = Context.Recipes
                                           .Where(r => r.RecipeId == recipeDto.RecipeId)
                                           .FirstOrDefault();

            if (recipeDto.CookTime != null)
            {
                recipeToUpdate.CookTime = recipeDto.CookTime;
            }

            if (recipeDto.Description != null)
            {
                recipeToUpdate.Description = recipeDto.Description;
            }

            if (recipeDto.Name != null)
            {
                recipeToUpdate.Name = recipeDto.Name;
            }

            if (recipeDto.PrepTime != null)
            {
                recipeToUpdate.PrepTime = recipeDto.PrepTime;
            }

            if (recipeDto.ServingSize != null)
            {
                recipeToUpdate.ServingSize = recipeDto.ServingSize;
            }

            if (recipeDto.RecipeUrl != null)
            {
                recipeToUpdate.RecipeUrl = recipeDto.RecipeUrl;
            }

            if (recipeDto.IsPublic != null)
            {
                recipeToUpdate.IsPublic = recipeDto.IsPublic;
            }

            Context.Entry(recipeToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Updates recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public async Task<bool> UpdateRecipeAsync(Recipe recipe, PantryPlannerUser userUpdating)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Context.RecipeExists(recipe) == false)
            {
                throw new RecipeNotFoundException(recipe.RecipeId);
            }

            if (Permissions.UserAddedRecipe(recipe, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this recipe");
            }

            Context.Entry(recipe).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public Recipe DeleteRecipe(Recipe recipe, PantryPlannerUser userDeleting)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            return DeleteRecipe(recipe.RecipeId, userDeleting);
        }

        /// <summary>
        /// Deletes recipe if user has rights to it (i.e. added the recipe)
        /// </summary>
        public Recipe DeleteRecipe(long recipeId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.RecipeExists(recipeId) == false)
            {
                throw new RecipeNotFoundException(recipeId);
            }

            if (Permissions.UserAddedRecipe(recipeId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to delete this recipe");
            }

            Recipe recipeToDelete = Context.Recipes.Find(recipeId);

            Context.Recipes.Remove(recipeToDelete);
            Context.SaveChanges();

            return recipeToDelete;
        }

        #endregion

    }
}
