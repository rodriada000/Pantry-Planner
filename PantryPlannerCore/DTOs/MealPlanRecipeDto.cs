using PantryPlannerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PantryPlanner.DTOs
{
    public class MealPlanRecipeDto
    {
        public int MealPlanRecipeId { get; set; }
        public long RecipeId { get; set; }
        public long MealPlanId { get; set; }
        public int SortOrder { get; set; }

        #region Additional Properties Not In Model

        public virtual RecipeDto Recipe { get; set; } = null!;

        #endregion

        public MealPlanRecipeDto()
        {
        }

        public MealPlanRecipeDto(MealPlanRecipe pr)
        {
            if (pr == null)
            {
                return;
            }

            MealPlanId = pr.MealPlanId;
            SortOrder = pr.SortOrder;
            MealPlanRecipeId = pr.MealPlanRecipeId;
            RecipeId = pr.RecipeId;
            Recipe = new RecipeDto(pr.Recipe);
        }

        public static List<MealPlanRecipeDto> ToList(List<MealPlanRecipe> list)
        {
            return list?.Select(k => new MealPlanRecipeDto(k))?.ToList();
        }

        public override string ToString()
        {
            string str = $"mpr: {Recipe.Name}; so = {SortOrder}";

            return str;
        }

        /// <summary>
        /// Return a <see cref="MealPlanRecipe"/> object based on the DTO
        /// </summary>
        internal MealPlanRecipe MealPlanRecipe()
        {
            return new MealPlanRecipe()
            {
                SortOrder = this.SortOrder,
                MealPlanId = this.MealPlanId,
                MealPlanRecipeId = this.MealPlanRecipeId,
                RecipeId = this.RecipeId,                
            };
        }
    }
}
