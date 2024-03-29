﻿using Microsoft.EntityFrameworkCore;
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
    public class ListIngredientService : IPantryService
    {
        public PantryPlannerContext Context { get; set; }

        public PermissionService Permissions { get; set; }
        public KitchenListService ListService { get; set; }


        public ListIngredientService(PantryPlannerContext context)
        {
            Context = context;
            Permissions = new PermissionService(Context);
            ListService = new KitchenListService(Context, new KitchenService(context));
        }


        #region Get Methods

        /// <summary>
        /// Return all ingredients in a shopping list
        /// </summary>
        public List<KitchenListIngredient> GetKitchenListIngredients(long listId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!Context.KitchenListExists(listId))
            {
                throw new KitchenListNotFoundException(listId);
            }

            var kitchenList = ListService.GetKitchenListById(listId, user);

            if (!Permissions.UserHasRightsToKitchen(user, kitchenList.KitchenId))
            {
                throw new PermissionsException("You do not have rights to this kitchen");
            }

            return Context.KitchenListIngredients.Where(k => k.KitchenListId == listId && !k.IsDeleted)
                                                .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                .Include(k => k.KitchenList)
                                                .ToList();
        }


        /// <summary>
        /// Return KitchenListIngredient for <paramref name="id"/>
        /// </summary>
        public KitchenListIngredient GetKitchenListIngredientById(long id, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!Context.KitchenListIngredientExists(id))
            {
                throw new IngredientNotFoundException($"No KitchenListIngredient exists with ID {id}");
            }

            KitchenListIngredient ingredient = GetKitchenListIngredientById(id);


            if (!Permissions.UserHasRightsToKitchen(user, ingredient.KitchenList.KitchenId))
            {
                throw new PermissionsException("You do not have rights to this ingredient");
            }

            return ingredient;
        }

        /// <summary>
        /// Return Ingredient for <paramref name="id"/>
        /// </summary>
        private KitchenListIngredient GetKitchenListIngredientById(long id)
        {
            return Context.KitchenListIngredients.Where(i => i.Id == id)
                                                .Include(i => i.Ingredient).ThenInclude(i => i.Category)
                                                .Include(i => i.KitchenList)
                                                .FirstOrDefault();
        }

        #endregion


        #region Add Methods

        /// <summary>
        /// Adds an ingredient to the kitchen that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenListIngredient AddKitchenListIngredient(ListIngredientDto newIngredient, PantryPlannerUser user)
        {
            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            KitchenListIngredient ingredientToAdd = ListIngredientDto.Create(newIngredient);

            return AddKitchenListIngredient(ingredientToAdd, newIngredient.KitchenId.Value, user);
        }

        /// <summary>
        /// Adds an ingredient to the list that was added by the <paramref name="user"/>.
        /// </summary>
        /// <param name="newIngredient"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenListIngredient AddKitchenListIngredient(KitchenListIngredient newIngredient, long kitchenId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (newIngredient == null)
            {
                throw new ArgumentNullException(nameof(newIngredient));
            }

            if (!Context.KitchenExists(kitchenId))
            {
                throw new KitchenNotFoundException(kitchenId);
            }

            if (!Context.IngredientExists(newIngredient.IngredientId))
            {
                throw new IngredientNotFoundException(newIngredient.IngredientId);
            }

            if (!Context.UserExists(user.Id))
            {
                throw new UserNotFoundException(user.UserName);
            }

            if (!Permissions.UserHasRightsToKitchen(user, kitchenId))
            {
                throw new PermissionsException("You do not have rights to add ingredients to this kitchen list");
            }


            if (Context.IngredientExistsForKitchenList(newIngredient.IngredientId, newIngredient.KitchenListId))
            {
                throw new InvalidOperationException($"This ingredient has already been added to the list.");
            }

            // get KitchenUserID for the current user
            KitchenUser kitchenUser = Context.GetKitchenUser(kitchenId, user.Id);

            if (kitchenUser == null)
            {
                throw new KitchenUserNotFoundException("Failed to find a user associated with this kitchen");
            }


            if (newIngredient.SortOrder < 0 || newIngredient.Id == 0)
            {
                if (Context.KitchenListIngredients.Any(k => k.KitchenListId == newIngredient.KitchenListId))
                {
                    newIngredient.SortOrder = Context.KitchenListIngredients.Where(k => k.KitchenListId == newIngredient.KitchenListId).Max(i => i.SortOrder) + 1;
                }
                else
                {
                    newIngredient.SortOrder = 0;
                }
            }

            // check if the ingredient has been deleted - just need to update instead of add
            KitchenListIngredient existingIngredient = Context.KitchenListIngredients.Where(i => i.KitchenListId == newIngredient.KitchenListId && i.IngredientId == newIngredient.IngredientId && i.IsDeleted)
                                                                                    .Include(i => i.Category)
                                                                                    .Include(i => i.KitchenList)
                                                                                    .Include(i => i.Ingredient).ThenInclude(j => j.Category)
                                                                                    .FirstOrDefault();

            if (existingIngredient != null)
            {
                existingIngredient.IsDeleted = false;
                existingIngredient.IsChecked = false;
                Context.Entry(existingIngredient).State = EntityState.Modified;
                Context.SaveChanges();

                return existingIngredient;
            }

            Context.KitchenListIngredients.Add(newIngredient);
            Context.SaveChanges();

            if (newIngredient.Ingredient == null)
            {
                newIngredient.Ingredient = Context.Ingredients.Where(i => i.IngredientId == newIngredient.IngredientId)
                                                               .Include(i => i.Category)
                                                               .Include(i => i.AddedByUser)
                                                               .FirstOrDefault();
            }
            newIngredient.KitchenList = Context.KitchenLists.Where(k => k.KitchenListId == newIngredient.KitchenListId)
                                                       .Include(k => k.Category)
                                                       .FirstOrDefault();

            return newIngredient;
        }


        /// <summary>
        /// Adds an ingredient to the kitchen list
        /// </summary>
        /// <param name="ingredientId"> ingredient to add </param>
        /// <param name="user"> user who is adding ingredient </param>
        public KitchenListIngredient AddIngredientToKitchenList(long ingredientId, long kitchenListId, PantryPlannerUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!Context.KitchenListExists(kitchenListId))
            {
                throw new KitchenListNotFoundException(kitchenListId);
            }

            if (!Context.UserExists(user.Id))
            {
                throw new UserNotFoundException(user.UserName);
            }

            if (!Permissions.UserHasRightsToKitchenList(user, kitchenListId))
            {
                throw new PermissionsException("You do not have rights to add ingredients to this kitchen");
            }

            KitchenListIngredient ingredientToAdd = new KitchenListIngredient()
            {
                IngredientId = ingredientId,
                Quantity = 1,
                IsChecked = false
            };

            ingredientToAdd.SortOrder = Context.KitchenListIngredients.Where(k => k.KitchenListId == kitchenListId).Max(i => i.SortOrder) + 1;

            KitchenList list = ListService.GetKitchenListById(kitchenListId, user);

            return AddKitchenListIngredient(ingredientToAdd, list.KitchenId, user);
        }

        #endregion


        #region Update Methods

        /// <summary>
        /// Updates ingredient if user has rights to it (i.e. added the ingredient)
        /// </summary>
        public async Task<bool> UpdateKitchenListIngredientAsync(ListIngredientDto ingredientDto, PantryPlannerUser userUpdating)
        {
            if (ingredientDto == null)
            {
                throw new ArgumentNullException(nameof(ingredientDto));
            }

            if (userUpdating == null)
            {
                throw new ArgumentNullException(nameof(userUpdating));
            }

            if (!Context.KitchenListIngredientExists(ingredientDto.Id))
            {
                throw new IngredientNotFoundException($"KitchenListIngredient with ID {ingredientDto.Id} does not exist.");
            }

            if (!Permissions.UserHasRightsToKitchen(userUpdating, ingredientDto.KitchenId.Value))
            {
                throw new PermissionsException("You do not have rights to update this ingredient");
            }


            KitchenListIngredient ingredientToUpdate = Context.KitchenListIngredients
                                                              .Where(ki => ki.Id == ingredientDto.Id)
                                                              .FirstOrDefault();

            // only update fields that are not null in the DTO
            if (ingredientDto.Quantity != null)
            {
                ingredientToUpdate.Quantity = ingredientDto.Quantity;
            }

            if (ingredientDto.SortOrder != null)
            {
                ingredientToUpdate.SortOrder = ingredientDto.SortOrder.Value;
            }

            ingredientToUpdate.Note = ingredientDto.Note;

            CreateCategoryIfNotExists(ingredientDto);

            if (ingredientDto.CategoryId != null)
            {
                ingredientToUpdate.CategoryId = ingredientDto.CategoryId;
            }

            ingredientToUpdate.IsChecked = ingredientDto.IsChecked;

            Context.Entry(ingredientToUpdate).State = EntityState.Modified;
            await Context.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        private void CreateCategoryIfNotExists(ListIngredientDto ingredientDto)
        {
            if (ingredientDto.CategoryId.GetValueOrDefault(0) == 0 && !string.IsNullOrWhiteSpace(ingredientDto?.Category?.Name))
            {
                int typeId = Context.CategoryTypes.Where(i => i.Name == "ListIngredient").Select(c => c.CategoryTypeId).FirstOrDefault();

                Category newCat = new Category()
                {
                    Name = ingredientDto.Category.Name,
                    CreatedByKitchenId = ingredientDto.KitchenId,
                    CategoryTypeId = typeId
                };

                Context.Categories.Add(newCat);
                Context.SaveChanges();

                ingredientDto.CategoryId = newCat.CategoryId;
            }
        }

        #endregion


        #region Delete Methods

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. user has rights to kitchen)
        /// </summary>
        public KitchenListIngredient DeleteKitchenListIngredient(KitchenListIngredient ingredient, PantryPlannerUser userDeleting)
        {
            if (ingredient == null)
            {
                throw new ArgumentNullException(nameof(ingredient));
            }

            return DeleteKitchenListIngredient(ingredient.Id, userDeleting);
        }

        /// <summary>
        /// Deletes ingredient if user has rights to it (i.e. user has rights to kitchen)
        /// </summary>
        public KitchenListIngredient DeleteKitchenListIngredient(long id, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (!Context.KitchenListIngredientExists(id))
            {
                throw new IngredientNotFoundException($"KitchenListIngredient with ID {id} does not exist.");
            }

            KitchenListIngredient ingredientToDelete = Context.KitchenListIngredients.Where(k => k.Id == id)
                                                                            .FirstOrDefault();

            if (!Permissions.UserHasRightsToKitchenList(userDeleting, ingredientToDelete.KitchenListId))
            {
                throw new PermissionsException($"You do not have rights to delete this ingredient");
            }

            ingredientToDelete.IsDeleted = true;

            Context.Entry(ingredientToDelete).State = EntityState.Modified;
            Context.SaveChanges();

            return ingredientToDelete;
        }


        public async Task DeleteCheckedIngredientsInList(long kitchenListId, PantryPlannerUser userDeleting)
        {
            if (userDeleting == null)
            {
                throw new ArgumentNullException(nameof(userDeleting));
            }

            if (!Context.KitchenListExists(kitchenListId))
            {
                throw new IngredientNotFoundException($"KitchenListIngredient with ID {kitchenListId} does not exist.");
            }

            if (!Permissions.UserHasRightsToKitchenList(userDeleting, kitchenListId))
            {
                throw new PermissionsException($"You do not have rights to delete from this list");
            }

            List<KitchenListIngredient>? ingredientsToDelete = await Context.KitchenListIngredients.Where(k => k.KitchenListId == kitchenListId && k.IsChecked).ToListAsync();

            foreach (var item in ingredientsToDelete)
            {
                item.IsDeleted = true;
            }

            Context.KitchenListIngredients.UpdateRange(ingredientsToDelete);
            await Context.SaveChangesAsync();
        }

        #endregion

    }
}
