import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import Recipe from 'src/app/data/models/Recipe';
import { RecipeApiService } from 'src/app/data/services/recipe-api.service';
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

  constructor(private recipeService: RecipeApiService,
    private route: ActivatedRoute,
    private mathUtil: MathUtilService,
    private toastService: ToastService) { }

  ngOnInit(): void {
    if (!!this.recipeId) {
      this.recipeService.getRecipeById(this.recipeId).subscribe(
        data => {
          this.recipe = data;
          this.recipe.ingredients.forEach(i => {
            i.quantityText = i.quantity % 1 == 0 ? i.quantity.toString() : this.mathUtil.decimalToFraction(i.quantity);
          })
        },
        error => {
          this.toastService.showDanger(`Failed to load recipe id ${this.recipeId} - ` + error.error);
        }
      );
    }
  }

}
