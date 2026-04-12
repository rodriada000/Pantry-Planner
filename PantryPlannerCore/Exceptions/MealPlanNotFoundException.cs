using System;
using System.Runtime.Serialization;

namespace PantryPlanner.Exceptions
{
    [Serializable]
    public class MealPlanNotFoundException : Exception
    {
        private long mealPlanId;

        public MealPlanNotFoundException()
        {
        }

        public MealPlanNotFoundException(long mealPlanId) : base($"Meal PLan with ID {mealPlanId} does not exist.")
        {
            this.mealPlanId = mealPlanId;
        }

        public MealPlanNotFoundException(string message) : base(message)
        {
        }

        public MealPlanNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MealPlanNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}