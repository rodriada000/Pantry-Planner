import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import Ingredient from 'src/app/data/models/Ingredient';
import Recipe from 'src/app/data/models/Recipe';
import RecipeIngredient from 'src/app/data/models/RecipeIngredient';
import RecipeStep from 'src/app/data/models/RecipeStep';
import { RecipeApiService } from 'src/app/data/services/recipe-api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-create-recipe',
  templateUrl: './create-recipe.component.html',
  styleUrls: ['./create-recipe.component.css']
})
export class CreateRecipeComponent implements OnInit {


  public originalRecipe: Recipe = null;

  public recipeId: number;
  public name: string;
  public recipeUrl: string;
  public description: string;
  public prepTime: number;
  public cookTime: number;
  public isPublic: boolean = true;
  public servingSize: string;

  public selectedIng: Ingredient;
  public ingredientQtyText: string;
  public unitOfMeasure: string;
  public method: string;

  public stepText: string;

  public ingredients: RecipeIngredient[] = [];
  public steps: RecipeStep[] = [];

  public isSaving: boolean = false;
  public isCreated: boolean = false;
  public isEditing: boolean = false;


  get ingredientQty(): number {
    if (!isNaN(Number(this.ingredientQtyText))) {
      return Number(this.ingredientQtyText);
    }
    
    if (this.ingredientQtyText.includes('/')) {
      const slashIndex = this.ingredientQtyText.indexOf('/');
      const num = this.ingredientQtyText.substring(0, slashIndex);
      const den = this.ingredientQtyText.substring(slashIndex+1);
      
      return Number(Number(num) / Number(den));
    }

    return NaN;
  }

  constructor(private recipeService: RecipeApiService,
    private route: ActivatedRoute,
    private toastService: ToastService) { }

  ngOnInit(): void {
    this.isEditing = true;

    this.route.paramMap.subscribe(p => {
      if (p['params']['id']) {
        this.recipeId = p['params']['id'];
        this.recipeService.getRecipeById(this.recipeId).subscribe(
          data => {
            this.isCreated = true;
            this.originalRecipe = data;
            this.name = data.name;
            this.recipeUrl = data.recipeUrl;
            this.description = data.description;
            this.isPublic = data.isPublic;
            this.prepTime = data.prepTime;
            this.cookTime = data.cookTime;
            this.servingSize = data.servingSize;
            this.steps = data.steps;
            this.ingredients = data.ingredients.map(i => { return {...i, quantityText: i.quantity.toString()} as RecipeIngredient});
          },
          error => {
            this.toastService.showDanger(`Failed to load recipe id ${this.recipeId} - ` + error.error);
          }
        )
      }
    });
  }

  confirmAdd() {
    if (!this.isCreated) {
      if (!!!this.name || !!!this.description) {
        return;
      }
      
      this.recipeService.addRecipe({
        name: this.name,
        recipeUrl: this.recipeUrl,
        description: this.description,
        isPublic: this.isPublic,
        prepTime: this.prepTime,
        cookTime: this.cookTime,
        servingSize: this.servingSize,
        dateCreated: new Date(),
        createdByUserId: '',
        createdByUsername: '',
        steps: [],
        ingredients: []
      } as Recipe).subscribe(
        data => {
          this.originalRecipe = {...data} as Recipe;
          this.isCreated = true;
          this.recipeId = data.recipeId;
          this.toastService.showSuccess('Created recipe!');
        },
        error => {
          this.toastService.showDanger("Failed to add recipe - " + error.error);
        }
      );
    } else {
      this.recipeService.updateRecipe({
        recipeId: this.recipeId,
        createdByUserId: this.originalRecipe.createdByUserId,
        createdByUsername: this.originalRecipe.createdByUsername,
        name: this.name,
        recipeUrl: this.recipeUrl,
        description: this.description,
        isPublic: this.isPublic,
        prepTime: this.prepTime,
        cookTime: this.cookTime,
        servingSize: this.servingSize,
        steps: this.steps,
        ingredients: this.ingredients.map(i => { delete i.quantityText; return i;})
      } as Recipe).subscribe(
        data => {
          this.toastService.showSuccess('Updated recipe!');
        },
        error => {
          this.toastService.showDanger("Failed to update recipe - " + error.error);
        }
      );
    }

  }

  discardChanges() {
    this.name = this.originalRecipe.name;
    this.recipeUrl = this.originalRecipe.recipeUrl;
    this.description = this.originalRecipe.description;
    this.prepTime = this.originalRecipe.prepTime;
    this.cookTime = this.originalRecipe.cookTime;
    this.servingSize = this.originalRecipe.servingSize;
  }

  addStep() {
    let step = new RecipeStep();
    step.text = this.stepText;
    step.recipeStepId = 0;
    step.sortOrder = this.steps.length + 1;
    step.recipeId = this.recipeId;

    this.recipeService.addRecipeStep(step).subscribe(
      data => {
        this.steps.push(data);
        this.stepText = "";
      },
      error => {
        this.toastService.showDanger("Failed to add step - " + error.error);
      }
    );

  }

  addIngredient() {
    if (!!!this.selectedIng || !!!this.ingredientQtyText) {
      return; // don't add until ingredient selected
    }

    let i = new RecipeIngredient();
    i.method = this.method ?? '';
    i.quantity = this.ingredientQty;
    i.quantityText = this.ingredientQtyText;
    i.unitOfMeasure = this.unitOfMeasure ?? '';
    i.sortOrder = this.ingredients.length + 1;
    i.ingredient = { ...this.selectedIng } as Ingredient;
    i.ingredientId = this.selectedIng?.ingredientId;
    i.recipeId = this.recipeId;

    this.recipeService.addRecipeIngredient(i).subscribe(
      data => {
        this.ingredients.push(data);

        this.method = "";
        this.selectedIng = null;
        this.unitOfMeasure = "";
      },
      error => {
        this.toastService.showDanger("Failed to add ingredient - " + error.error);
      }
    );


  }

  removeIngredient(ingredient: RecipeIngredient, index: number) {
    this.recipeService.deleteRecipeIngredient(ingredient).subscribe(
      data => {
        this.ingredients.splice(index, 1);
      },
      error => {
        this.toastService.showDanger("Failed to remove ingredient - " + error.error);
      }
    );
  }

  removeStep(step: RecipeStep, index: number) {
    this.recipeService.deleteRecipeStep(step).subscribe(
      data => {
        this.steps.splice(index, 1);
      },
      error => {
        this.toastService.showDanger("Failed to remove step - " + error.error);
      }
    );
  }

  moveStep(index: number, moveBy: number) {
    const newIndex: number = index + moveBy;

    if (newIndex < 0 || newIndex >= this.steps.length) {
      return;
    }

    this.steps.splice(newIndex, 0, this.steps.splice(index, 1)[0]);

    for (let i = 0; i < this.steps.length; i++) {
      this.steps[i].sortOrder = i + 1;
    }

    this.recipeService.updateRecipeStep(this.steps[index]).subscribe();
    this.recipeService.updateRecipeStep(this.steps[newIndex]).subscribe();
  }

}
