using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PantryPlannerCore.Models
{
    public partial class RecipeIngredient
    {
        public int RecipeIngredientId { get; set; }
        public long IngredientId { get; set; }
        public long RecipeId { get; set; }
        public decimal Quantity { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string? UnitOfMeasure { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string? Method { get; set; }
        public int SortOrder { get; set; }

        public virtual Ingredient Ingredient { get; set; } = null!;
        public virtual Recipe Recipe { get; set; } = null!;
    }
}
