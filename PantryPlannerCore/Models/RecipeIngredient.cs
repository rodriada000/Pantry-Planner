using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class RecipeIngredient
    {
        public int RecipeIngredientId { get; set; }
        public long IngredientId { get; set; }
        public long RecipeId { get; set; }
        public decimal Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public string? Method { get; set; }
        public int SortOrder { get; set; }

        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
