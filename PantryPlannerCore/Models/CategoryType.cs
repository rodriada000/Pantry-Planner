using System;
using System.Collections.Generic;

namespace PantryPlannerCore.Models
{
    public partial class CategoryType
    {
        public CategoryType()
        {
            Categories = new HashSet<Category>();
        }

        public int CategoryTypeId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Category> Categories { get; set; }
    }
}
