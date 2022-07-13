using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class KitchenUser
    {
        public KitchenUser()
        {
            IngredientTags = new HashSet<IngredientTag>();
            KitchenIngredients = new HashSet<KitchenIngredient>();
            MealPlans = new HashSet<MealPlan>();
        }

        public long KitchenUserId { get; set; }
        public string UserId { get; set; } = null!;
        public long KitchenId { get; set; }
        public bool IsOwner { get; set; }
        public bool HasAcceptedInvite { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Kitchen Kitchen { get; set; } = null!;
        public virtual PantryPlannerUser User { get; set; } = null!;
        public virtual ICollection<IngredientTag> IngredientTags { get; set; }
        public virtual ICollection<KitchenIngredient> KitchenIngredients { get; set; }
        public virtual ICollection<MealPlan> MealPlans { get; set; }
    }
}
