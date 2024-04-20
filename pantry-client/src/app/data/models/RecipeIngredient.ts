import Ingredient from './Ingredient';

export default class RecipeIngredient {
  recipeIngredientId: number;
  ingredientId: number;
  recipeId: number;
  quantity: number;
  quantityText: string;
  unitOfMeasure: string = '';
  method: string = '';
  sortOrder: number;
  ingredient: Ingredient;
}
