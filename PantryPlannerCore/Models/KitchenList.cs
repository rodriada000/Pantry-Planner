using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class KitchenList
    {
        public KitchenList()
        {
            KitchenListIngredients = new HashSet<KitchenListIngredient>();
        }

        public long KitchenListId { get; set; }
        public long KitchenId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; } = null!;

        public virtual Category? Category { get; set; }
        public virtual Kitchen Kitchen { get; set; } = null!;
        public virtual ICollection<KitchenListIngredient> KitchenListIngredients { get; set; }
    }
}
