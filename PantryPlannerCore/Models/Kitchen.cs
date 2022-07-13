using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class Kitchen
    {
        public Kitchen()
        {
            Categories = new HashSet<Category>();
            IngredientTags = new HashSet<IngredientTag>();
            KitchenIngredients = new HashSet<KitchenIngredient>();
            KitchenLists = new HashSet<KitchenList>();
            KitchenRecipes = new HashSet<KitchenRecipe>();
            KitchenUsers = new HashSet<KitchenUser>();
            MealPlans = new HashSet<MealPlan>();
        }

        public long KitchenId { get; set; }
        public Guid UniquePublicGuid { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public string? CreatedByUserId { get; set; }

        public virtual PantryPlannerUser? CreatedByUser { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredients { get; set; }
        public virtual ICollection<KitchenList> KitchenLists { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipes { get; set; }
        public virtual ICollection<KitchenUser> KitchenUsers { get; set; }
        public virtual ICollection<MealPlan> MealPlans { get; set; }
    }
}
