using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class Ingredient
    {
        public Ingredient()
        {
            IngredientTags = new HashSet<IngredientTag>();
            KitchenIngredients = new HashSet<KitchenIngredient>();
            KitchenListIngredients = new HashSet<KitchenListIngredient>();
            RecipeIngredients = new HashSet<RecipeIngredient>();
        }

        public long IngredientId { get; set; }
        public string? AddedByUserId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public byte[]? PreviewPicture { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsPublic { get; set; }

        public virtual PantryPlannerUser? AddedByUser { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredients { get; set; }
        public virtual ICollection<KitchenListIngredient> KitchenListIngredients { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
