using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class Category
    {
        public Category()
        {
            Ingredients = new HashSet<Ingredient>();
            KitchenIngredients = new HashSet<KitchenIngredient>();
            KitchenListIngredients = new HashSet<KitchenListIngredient>();
            KitchenLists = new HashSet<KitchenList>();
            KitchenRecipes = new HashSet<KitchenRecipe>();
            MealPlans = new HashSet<MealPlan>();
        }

        public long CategoryId { get; set; }
        public int? CategoryTypeId { get; set; }
        public long? CreatedByKitchenId { get; set; }
        public string Name { get; set; } = null!;

        public virtual CategoryType? CategoryType { get; set; }
        public virtual Kitchen? CreatedByKitchen { get; set; }
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredients { get; set; }
        public virtual ICollection<KitchenListIngredient> KitchenListIngredients { get; set; }
        public virtual ICollection<KitchenList> KitchenLists { get; set; }
        public virtual ICollection<KitchenRecipe> KitchenRecipes { get; set; }
        public virtual ICollection<MealPlan> MealPlans { get; set; }
    }
}
