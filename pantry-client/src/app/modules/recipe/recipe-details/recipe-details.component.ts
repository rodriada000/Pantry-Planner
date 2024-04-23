import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import KitchenIngredient from 'src/app/data/models/KitchenIngredient';
import Recipe from 'src/app/data/models/Recipe';
import KitchenIngredientApi from 'src/app/data/services/kitchenIngredientApi.service';
import { RecipeApiService } from 'src/app/data/services/recipe-api.service';
import { ActiveKitchenService } from 'src/app/shared/services/active-kitchen.service';
import { MathUtilService } from 'src/app/shared/services/math-util.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-recipe-details',
  templateUrl: './recipe-details.component.html',
  styleUrls: ['./recipe-details.component.css']
})
export class RecipeDetailsComponent implements OnInit {
  
  @Input()
  recipeId: number;
  recipe: Recipe;

  steps: any[] = [];
  kitchenIngredients: KitchenIngredient[] = [];

  constructor(private recipeService: RecipeApiService,
    private route: ActivatedRoute,
    private mathUtil: MathUtilService,
    private kitchenApi: KitchenIngredientApi,
    private activeKitchen: ActiveKitchenService,
    private toastService: ToastService) { }

  ngOnInit(): void {
    if (!!this.recipeId) {
      this.recipeService.getRecipeById(this.recipeId).subscribe(
        data => {
          this.recipe = data;
          this.recipe.ingredients.forEach(i => {
            i.quantityText = i.quantity % 1 == 0 ? i.quantity.toString() : this.mathUtil.decimalToFraction(i.quantity);
          });
          this.calcStepNum();
          this.getKitchenIngredients();
        },
        error => {
          this.toastService.showDanger(`Failed to load recipe id ${this.recipeId} - ` + error.error);
        }
      );
    }
  }

  getKitchenIngredients() {
    this.kitchenApi.getExistingIngredientsInKitchen(this.activeKitchen.activeKitchenId, this.recipe.ingredients.map(i => i.ingredientId)).subscribe(
      data => {
        this.kitchenIngredients = data;
      },
      error => {
        this.toastService.showDanger(`Failed to get existing kitchen ingredients - ` + error.error);
      }
    );
  }

  ingredientInKitchen(ingredientId: number): boolean {
    return !!this.kitchenIngredients?.find(i => i.ingredientId == ingredientId);
  }

  calcStepNum() {
    let stepNum: number = 1;
    this.steps = [...this.recipe.steps];

    this.steps.forEach(s => {
      if (s?.text?.startsWith('#')) {
        stepNum = 1;
      } else {
        s.displayStep = stepNum++;
      }
    });
  }

}
