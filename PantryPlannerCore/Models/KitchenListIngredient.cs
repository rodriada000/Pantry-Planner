﻿using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class KitchenListIngredient
    {
        public int Id { get; set; }
        public long KitchenListId { get; set; }
        public long IngredientId { get; set; }
        public long? AddedFromRecipeId { get; set; }
        public int? Quantity { get; set; }
        public int SortOrder { get; set; }
        public bool IsChecked { get; set; }
        public long? CategoryId { get; set; }
        public string? Note { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Recipe? AddedFromRecipe { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual KitchenList KitchenList { get; set; } = null!;
    }
}
