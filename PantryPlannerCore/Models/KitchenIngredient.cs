using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class KitchenIngredient
    {
        public long KitchenIngredientId { get; set; }
        public long IngredientId { get; set; }
        public long KitchenId { get; set; }
        public long? AddedByKitchenUserId { get; set; }
        public long? CategoryId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? Quantity { get; set; }
        public string Note { get; set; } = null!;

        public virtual KitchenUser? AddedByKitchenUser { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Kitchen Kitchen { get; set; } = null!;
    }
}
