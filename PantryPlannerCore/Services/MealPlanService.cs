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
    public class MealPlanService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        private readonly RecipeIngredientService _recipeIngredientService;
        private readonly RecipeStepService _recipeStepService;

        private readonly RecipeService _receiptService;
        private readonly KitchenUserService _kitchenUserService;
        private readonly KitchenService _kitchenService;


        public MealPlanService(PantryPlannerContext context, RecipeIngredientService recipeIngredientService, RecipeStepService recipeStepService, RecipeService receiptService, KitchenUserService kitchenUserService, KitchenService kitchenService)
        {
            Context = context;
            Permissions = new PermissionService(Context);
            _recipeIngredientService = recipeIngredientService;
            _recipeStepService = recipeStepService;
            _receiptService = receiptService;
            _kitchenUserService = kitchenUserService;
            _kitchenService = kitchenService;
        }


        #region Get Methods

        /// <summary>
        /// Return meal plan for <paramref name="mealPlanId"/>
        /// </summary>
        public MealPlan GetMealPlanById(long mealPlanId, PantryPlannerUser user)
        {
            if (Context.MealPlanExists(mealPlanId) == false)
            {
                throw new MealPlanNotFoundException(mealPlanId);
            }

            MealPlan plan = GetMealPlanById(mealPlanId);

            if (plan != null && Permissions.UserHasRightsToKitchen(user, plan.KitchenId) == false)
            {
                throw new PermissionsException("You do not have rights to this meal plan");
            }

            return plan;
        }

        /// <summary>
        /// Return meal plan for <paramref name="mealPlanId"/>
        /// </summary>
        public MealPlan? GetMealPlanById(long mealPlanId)
        {
            return Context.MealPlans.Where(r => r.MealPlanId == mealPlanId)
                                            .Include(i => i.Category)
                                            .Include(i => i.MealPlanRecipes.OrderBy(r => r.SortOrder))
                                            .ThenInclude(m => m.Recipe)
                                            .FirstOrDefault();
        }

        #endregion


        #region Add Methods


        /// <summary>
        /// Adds a Meal Plan to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newPlan"> plan to add </param>
        /// <param name="user"> user who is adding plan </param>
        public MealPlan AddMealPlan(MealPlanDto newPlan, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newPlan == null)
            {
                throw new ArgumentNullException(nameof(newPlan));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            MealPlan planToAdd = newPlan.MealPlan();

            var added = AddMealPlan(planToAdd, user);

            if (newPlan.Recipes.Count > 0)
            {
                newPlan.Recipes.ForEach(s =>
                {
                    //s.RecipeId = added.RecipeId;
                    //_recipeStepService.AddRecipeStep(s, user);
                });
            }

            return added;
        }

        /// <summary>
        /// Adds a MealPlan to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newPlan"> meal plan to add </param>
        /// <param name="user"> user who is adding recipe </param>
        public MealPlan AddMealPlan(MealPlan newPlan, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newPlan == null)
            {
                throw new ArgumentNullException(nameof(newPlan));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate name passed in
            if (String.IsNullOrWhiteSpace(newPlan.Name))
            {
                throw new InvalidOperationException("Meal Plan name is required");
            }

            newPlan.DateCreated = DateTime.Now;

            Context.MealPlans.Add(newPlan);
            Context.SaveChanges();

            return newPlan;
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
