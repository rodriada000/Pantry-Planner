import { Component, OnInit } from '@angular/core';
import RecipeIngredient from 'src/app/data/models/RecipeIngredient';
import RecipeStep from 'src/app/data/models/RecipeStep';

@Component({
  selector: 'app-create-recipe',
  templateUrl: './create-recipe.component.html',
  styleUrls: ['./create-recipe.component.css']
})
export class CreateRecipeComponent implements OnInit {

  public name: string;
  public recipeUrl: string;
  public description: string;
  public prepTime: number;
  public cookTime: number;
  public isPublic: boolean = true;
  public servingSize: string;

  public ingredientQty: number;
  public unitOfMeasure: string;
  public method: string;

  public stepText: string;

  public ingredients: RecipeIngredient[] = [];
  public steps: RecipeStep[] = [];

  public isSaving: boolean = false;
  public isCreated: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

  confirmAdd() {

  }

  discardChanges() {

  }

  addStep() {
    let step = new RecipeStep();
    step.text = this.stepText;
    step.recipeStepId = 0;
    step.sortOrder = this.steps.length + 1;
    this.steps.push(step);

    this.stepText = "";
  }

  addIngredient() {
    let i = new RecipeIngredient();
    i.method = this.method;
    i.quantity = this.ingredientQty;
    i.unitOfMeasure = this.unitOfMeasure;
    i.sortOrder = this.ingredients.length + 1;

    this.ingredients.push(i);

    this.method = "";
    this.ingredientQty = 1;
    this.unitOfMeasure = "";
  }

}
