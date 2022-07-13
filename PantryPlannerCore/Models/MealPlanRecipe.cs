using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class MealPlanRecipe
    {
        public int MealPlanRecipeId { get; set; }
        public long RecipeId { get; set; }
        public long MealPlanId { get; set; }
        public int SortOrder { get; set; }

        public virtual MealPlan MealPlan { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
