using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class Recipe
    {
        public Recipe()
        {
            KitchenListIngredients = new HashSet<KitchenListIngredient>();
            KitchenRecipes = new HashSet<KitchenRecipe>();
            MealPlanRecipes = new HashSet<MealPlanRecipe>();
            RecipeIngredients = new HashSet<RecipeIngredient>();
            RecipeSteps = new HashSet<RecipeStep>();
        }

        public long RecipeId { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? RecipeUrl { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int? PrepTime { get; set; }
        public int? CookTime { get; set; }
        public string? ServingSize { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsPublic { get; set; }

        public virtual PantryPlannerUser? CreatedByUser { get; set; }
        public virtual ICollection<KitchenListIngredient> KitchenListIngredients { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipes { get; set; }
        public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual ICollection<RecipeStep> RecipeSteps { get; set; }
    }
}
