﻿using Microsoft.EntityFrameworkCore;
using PantryPlanner.Exceptions;
using PantryPlanner.Extensions;
using PantryPlannerCore.Extensions;
using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.Services
{
    public class IngredientService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        private PermissionService Permissions { get; set; }

        public IngredientService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
        }


        #region Get Methods

        /// <summary>
        /// Return Ingredient for <paramref name="ingredientId"/>
        /// </summary>
        public Ingredient GetIngredientById(long ingredientId, PantryPlannerUser user)
        {
            if (Context.IngredientExists(ingredientId) == false)
            {
                throw new IngredientNotFoundException(ingredientId);
            }

            Ingredient ingredient = GetIngredientById(ingredientId);


            if (Permissions.UserHasViewRightsToIngredient(ingredient, user) == false)
            {
                throw new PermissionsException("You do not have rights to this ingredient");
            }

            return ingredient;
        }

        /// <summary>
        /// Return Ingredient for <paramref name="ingredientId"/>
        /// </summary>
        public Ingredient GetIngredientById(long ingredientId)
        {
            return Context.Ingredients.Where(i => i.IngredientId == ingredientId)
                                            .Include(i => i.Category)
                                            .Include(i => i.AddedByUser)
                                            .FirstOrDefault();
        }

        /// <summary>
        /// Return list of Ingredients with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        /// <param name="name"> name to search for </param>
        public List<Ingredient> GetIngredientByName(string name, string userId)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            // first check for exact match
            if (Context.Ingredients.Any(i => i.Name.ToUpper() == name.ToUpper() && (i.IsPublic || i.AddedByUserId == userId)))
            {
                return Context.Ingredients.Where(i => i.Name.ToUpper() == name.ToUpper() && (i.IsPublic || i.AddedByUserId == userId))
                                         .Include(i => i.Category)
                                         .Include(i => i.AddedByUser).ToList();
            }


            // second check for any matches that have all the words entered
            IEnumerable<string> wordsToSearchFor = name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).AsEnumerable();

            List<Ingredient> ingredients = new List<Ingredient>();
            //List<Ingredient> ingredients = Context.Ingredients.Where(i => wordsToSearchFor.AsEnumerable().All(w => EF.Functions.Like(i.Name, w)) && (i.IsPublic || i.AddedByUserId == userId))
            //                .Include(i => i.Category)
            //                .Include(i => i.AddedByUser)
            //                .ToList();



            if (ingredients.Count == 0)
            {
                // if no matches then lastly check if any word entered matches
                foreach (var word in wordsToSearchFor)
                {
                    ingredients.AddRange(Context.Ingredients.Where(i => EF.Functions.Like(i.Name, '%'+word+'%') && (i.IsPublic || i.AddedByUserId == userId))
                            .Include(i => i.Category)
                            .Include(i => i.AddedByUser)
                            .ToList());
                }
                //ingredients = Context.Ingredients.Where(i => wordsToSearchFor.Any(w => EF.Functions.Like(i.Name, w)) && (i.IsPublic || i.AddedByUserId == userId))
                //            .Include(i => i.Category)
                //            .Include(i => i.AddedByUser)
                //            .ToList();
            }

            return ingredients;
        }

        /// <summary>
        /// Return list of Ingredients in the passed in <paramref name="category"/> and with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        public List<Ingredient> GetIngredientByNameAndCategory(string name, Category category, string userId)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return GetIngredientByName(name, userId).Where(i => i.CategoryId == category.CategoryId).ToList();
        }

        /// <summary>
        /// Return list of Ingredients with names that match the given <paramref name="name"/> passed in.
        /// </summary>
        public List<Ingredient> GetIngredientByNameAndCategory(string name, string categoryName, string userId)
        {
            if (String.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            Category foundCategory = (from category in Context.Categories
                                      join catType in Context.CategoryTypes on category.CategoryTypeId equals catType.CategoryTypeId
                                      where catType.Name == "Ingredient" && category.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase)
                                      select category).FirstOrDefault();

            if (foundCategory == null)
            {
                throw new CategoryNotFoundException();
            }


            return GetIngredientByNameAndCategory(name, foundCategory, userId);
        }

        /// <summary>
        /// Return list of ingredients with descriptions that match the given <paramref name="description"/> passed in.
        /// </summary>
        /// <param name="description"> text to search for in Ingredient description </param>
        public List<Ingredient> GetIngredientByDescription(string description, string userId)
        {
            if (String.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }


            return Context.Ingredients.Where(i => i.Description.Contains(description, StringComparison.OrdinalIgnoreCase) && (i.IsPublic || i.AddedByUserId == userId))
                                    .Include(i => i.Category)
                                    .Include(i => i.AddedByUser)
                                    .ToList();
        }

        /// <summary>
        /// Return list of ingredients with in the the given <paramref name="category"/>.
        /// </summary>
        public List<Ingredient> GetIngredientByCategory(Category category, string userId)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return GetIngredientByCategory(category.CategoryId, userId);
        }

        /// <summary>
        /// Return list of ingredients with in the the given <paramref name="categoryId"/>.
        /// </summary>
        public List<Ingredient> GetIngredientByCategory(long categoryId, string userId)
        {
            if (Context.CategoryExists(categoryId) == false)
            {
                throw new CategoryNotFoundException(categoryId);
            }

            return Context.Ingredients.Where(i => i.CategoryId == categoryId && (i.IsPublic || i.AddedByUserId == userId))
                                .Include(i => i.Category)
                                .Include(i => i.AddedByUser)
                                .ToList();
        }

        /// <summary>
        /// Return list of categories for ingredients that user has rights to.
        /// </summary>
        public List<Category> GetIngredientCategories(PantryPlannerUser user)
        {
            // ensure CategoryType 'Ingredient' exists
            CategoryType ingredientCategoryType;

            if (Context.CategoryTypes.Any(c => c.Name == "Ingredient") == false)
            {
                ingredientCategoryType = new CategoryType()
                {
                    Name = "Ingredient"
                };

                Context.CategoryTypes.Add(ingredientCategoryType);
                Context.SaveChanges();
            }
            else
            {
                ingredientCategoryType = Context.CategoryTypes.Where(c => c.Name == "Ingredient").FirstOrDefault();
            }

            List<Category> publicCategories = Context.Categories.Where(c => c.CategoryTypeId == ingredientCategoryType.CategoryTypeId && c.CreatedByKitchenId == null)
                                                              .Include(c => c.CategoryType)
                                                              .ToList();

            List<long> kitchens = Context.KitchenUsers.Where(k => k.HasAcceptedInvite == true && k.UserId == user.Id)
                                                     .Select(k => k.KitchenId)
                                                     .ToList();


            foreach (long kitchenID in kitchens)
            {
                List<Category> privateCategories = Context.Categories.Where(c => c.CategoryTypeId == ingredientCategoryType.CategoryTypeId && c.CreatedByKitchenId.GetValueOrDefault(0) == kitchenID)
                                                                  .Include(c => c.CategoryType)
                                                                  .ToList();

                publicCategories.AddRange(privateCategories);
            }

            return publicCategories.OrderBy(c => c.Name).ToList();
        }


        #endregion


        #region Add Methods


        /// <summary>
        /// Adds an ingredient to the <see cref="Context"/> that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public void AddIngredient(Ingredient newIngredient, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            if (Context.UserExists(user.Id) == false)
            {
                throw new UserNotFoundException(user.UserName);
            }

            // validate name passed in
            if (String.IsNullOrWhiteSpace(newIngredient.Name))
            {
                throw new InvalidOperationException("Ingredient Name is required");
            }

            // validate an ingredient with same name in the same category doesn't already exist
            if (Context.IngredientExistsForUser(newIngredient, user))
            {
                throw new InvalidOperationException($"An ingredient with the name {newIngredient.Name} already exists");
            }

            newIngredient.Name = newIngredient.Name?.ToProperCase();
            newIngredient.AddedByUserId = user.Id;
            newIngredient.DateAdded = DateTime.Now;

            Context.Ingredients.Add(newIngredient);
            Context.SaveChanges();
        }

        /// <summary>
        /// Adds an ingredient marked as IsPublic=true to the <see cref="Context"/>.
        /// The AddedByUserId will be null.
        /// </summary>
        public void AddIngredient(Ingredient newIngredient)
        {
            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            // validate an ingredient with same name in the same category doesn't already exist
            if (Context.IngredientExistsPublicly(newIngredient))
            {
                throw new InvalidOperationException($"An ingredient with the name {newIngredient.Name} already exists");
            }

            newIngredient.Name = newIngredient.Name?.ToProperCase();
            newIngredient.AddedByUserId = null;
            newIngredient.IsPublic = true;
            newIngredient.DateAdded = DateTime.Now;

            Context.Ingredients.Add(newIngredient);
            Context.SaveChanges();
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public void UpdateIngredient(Ingredient ingredient, PantryPlannerUser userUpdating)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (Permissions.UserAddedIngredient(ingredient, userUpdating) == false)
            {
                throw new PermissionsException("You do not have rights to update this ingredient");
            }

            Context.Entry(ingredient).State = EntityState.Modified;
            Context.SaveChanges();
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public Ingredient DeleteIngredient(Ingredient ingredient, PantryPlannerUser userDeleting)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            return DeleteIngredient(ingredient.IngredientId, userDeleting);
        }

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public Ingredient DeleteIngredient(long ingredientId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (Context.IngredientExists(ingredientId) == false)
            {
                throw new IngredientNotFoundException(ingredientId);
            }

            if (Permissions.UserAddedIngredient(ingredientId, userDeleting) == false)
            {
                throw new PermissionsException($"You do not have rights to delete this ingredient");
            }

            Ingredient ingredientToDelete = Context.Ingredients.Find(ingredientId);

            Context.Ingredients.Remove(ingredientToDelete);
            Context.SaveChanges();

            return ingredientToDelete;
        }

        #endregion

    }
}
