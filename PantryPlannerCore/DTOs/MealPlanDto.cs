using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class MealPlanDto
    {
        public long MealPlanId { get; set; }
        public long KitchenId { get; set; }
        public long? CreatedByKitchenUserId { get; set; }
        public long? CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int SortOrder { get; set; }
        public bool IsFavorite { get; set; }
        public int StartDay { get; set; }

        #region Additional Properties Not In Model

        public string? CreatedByUsername { get; set; }
        public List<MealPlanRecipeDto> Recipes { get; set; }
        public CategoryDto? Category { get; set; }


        #endregion

        public MealPlanDto()
        {
        }

        public MealPlanDto(MealPlan plan)
        {
            if (plan == null)
            {
                return;
            }

            MealPlanId = plan.MealPlanId;
            CreatedByKitchenUserId = plan.CreatedByKitchenUserId;
            Name = plan.Name;
            Description = plan.Description;
            DateCreated = plan.DateCreated;
            KitchenId = plan.KitchenId;
            CategoryId = plan.CategoryId;
            SortOrder = plan.SortOrder;
            IsFavorite = plan.IsFavorite;
            StartDay = plan.StartDay;

            Category = new CategoryDto(plan.Category);
            Recipes = MealPlanRecipeDto.ToList(plan.MealPlanRecipes?.ToList());
            CreatedByUsername = plan.CreatedByKitchenUser?.User?.UserName;
        }

        public static List<MealPlanDto> ToList(List<MealPlan> list)
        {
            return list?.Select(k => new MealPlanDto(k))?.ToList();
        }

        public override string ToString()
        {
            string str = $"mp: {Name}; sd = {StartDay}, num recipes: {Recipes?.Count}";

            return str;
        }

        /// <summary>
        /// Return a <see cref="MealPlan"/> object based on the DTO
        /// </summary>
        internal MealPlan MealPlan()
        {
            return new MealPlan()
            {
                Name = this.Name,
                DateCreated = this.DateCreated,
                StartDay = this.StartDay,
                IsFavorite = this.IsFavorite,
                SortOrder = this.SortOrder,
                CategoryId = this.CategoryId,
                CreatedByKitchenUserId = this.CreatedByKitchenUserId,
                Description = this.Description,
                KitchenId = this.KitchenId,
                MealPlanId = this.MealPlanId,
            };
        }
    }
}
