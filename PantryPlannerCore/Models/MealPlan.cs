using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class MealPlan
    {
        public MealPlan()
        {
            MealPlanRecipes = new HashSet<MealPlanRecipe>();
        }

        public long MealPlanId { get; set; }
        public long KitchenId { get; set; }
        public long? CreatedByKitchenUserId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int SortOrder { get; set; }
        public bool IsFavorite { get; set; }

        public virtual Category? Category { get; set; }
        public virtual KitchenUser? CreatedByKitchenUser { get; set; }
        public virtual Kitchen Kitchen { get; set; } = null!;
        public virtual ICollection<MealPlanRecipe> MealPlanRecipes { get; set; }
    }
}
