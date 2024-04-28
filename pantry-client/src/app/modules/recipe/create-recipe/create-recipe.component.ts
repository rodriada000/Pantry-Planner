import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService } from 'primeng/api';
import { Observable } from 'rxjs';
import Ingredient from 'src/app/data/models/Ingredient';
import Recipe from 'src/app/data/models/Recipe';
import RecipeIngredient from 'src/app/data/models/RecipeIngredient';
import RecipeStep from 'src/app/data/models/RecipeStep';
import { RecipeApiService } from 'src/app/data/services/recipe-api.service';
import { RecipeScraperService } from 'src/app/data/services/recipe-scraper.service';
import { MathUtilService } from 'src/app/shared/services/math-util.service';
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

  public selectedIngredientIdx: number = -1;
  ingredientEditIndex: number = -1;
  public selectedStepIdx: number = -1;
  stepEditIndex: number = -1;

  public isSaving: boolean = false;
  public isCreated: boolean = false;
  public isEditing: boolean = false;
  public isScraped: boolean = false;


  get ingredientQty(): number {
    if (!isNaN(Number(this.ingredientQtyText))) {
      return Number(this.ingredientQtyText);
    }

    if (this.ingredientQtyText.includes('/')) {
      const slashIndex = this.ingredientQtyText.indexOf('/');
      const num = this.ingredientQtyText.substring(0, slashIndex);
      const den = this.ingredientQtyText.substring(slashIndex + 1);

      return Number(Number(num) / Number(den));
    }

    return NaN;
  }

  constructor(private recipeService: RecipeApiService,
    private route: ActivatedRoute,
    private router: Router,
    private mathUtil: MathUtilService,
    private confirmationService: ConfirmationService,
    private scraper: RecipeScraperService,
    private toastService: ToastService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      if (p['params']['id'] && this.recipeId !== +p['params']['id']) {
        this.recipeId = +p['params']['id'];

        if (!!!this.recipeId || this.recipeId === 0) {
          this.isCreated = false;
          this.isEditing = true;
          this.name = '';
          this.recipeUrl = '';
          this.description = '';
          this.prepTime = null;
          this.cookTime = null;
          this.servingSize = '';
          this.steps = [];
          this.ingredients = [];
          return;
        }

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
            this.ingredients = data.ingredients;
            this.ingredients.forEach(i => {
              i.quantityText = i.quantity % 1 == 0 ? i.quantity.toString() : this.mathUtil.decimalToFraction(i.quantity);
            });
          },
          error => {
            this.toastService.showDanger(`Failed to load recipe id ${this.recipeId} - ` + error.error);
          }
        )
      }
    });
  }

  saveRecipe() {
    if (!!!this.name || !!!this.description) {
      return;
    }

    this.isSaving = true;

    if (!this.isCreated) {
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
        steps: this.steps,
        ingredients: this.ingredients
      } as Recipe).subscribe(
        data => {
          this.originalRecipe = { ...data } as Recipe;
          this.isCreated = true;
          this.recipeId = data.recipeId;
          this.toastService.showSuccess('Created recipe!');

          this.router.navigate(['/recipe', this.recipeId], { queryParamsHandling: 'merge' });
          this.isSaving = false;
        },
        error => {
          this.toastService.showDanger("Failed to add recipe - " + error.error);
          this.isSaving = false;
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
        ingredients: this.ingredients
      } as Recipe).subscribe(
        data => {
          this.originalRecipe = data;
          this.toastService.showSuccess('Updated recipe!');
          this.isEditing = false;
          this.isSaving = false;
        },
        error => {
          this.toastService.showDanger("Failed to update recipe - " + error.error);
          this.isSaving = false;
        }
      );
    }
  }

  discardChanges() {
    this.isEditing = false;

    if (!!!this.originalRecipe) {
      return;
    }

    this.name = this.originalRecipe.name;
    this.recipeUrl = this.originalRecipe.recipeUrl;
    this.description = this.originalRecipe.description;
    this.prepTime = this.originalRecipe.prepTime;
    this.cookTime = this.originalRecipe.cookTime;
    this.servingSize = this.originalRecipe.servingSize;
  }

  addStep() {
    let step = this.stepEditIndex >= 0 ? this.steps[this.stepEditIndex] : new RecipeStep();
    step.text = this.stepText;
    step.sortOrder = this.stepEditIndex >= 0 ? this.stepEditIndex + 1 : this.steps.length + 1;
    step.recipeId = this.recipeId;

    let apiRequest: Observable<RecipeStep>;
    if (this.stepEditIndex >= 0) {
      apiRequest = this.recipeService.updateRecipeStep(step)
    } else {
      step.recipeStepId = 0;
      apiRequest = this.recipeService.addRecipeStep(step);
    }

    apiRequest.subscribe(
      data => {
        if (this.stepEditIndex == -1) {
          this.steps.push(data);
        }

        this.stepText = "";
        this.stepEditIndex = -1;
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

    let i = this.ingredientEditIndex >= 0 ? this.ingredients[this.ingredientEditIndex] : new RecipeIngredient();

    i.method = this.method ?? '';
    i.quantity = this.ingredientQty;
    i.quantityText = this.ingredientQtyText;
    i.unitOfMeasure = this.unitOfMeasure ?? '';
    i.sortOrder = this.ingredientEditIndex >= 0 ? this.ingredientEditIndex + 1 : this.ingredients.length + 1;
    i.ingredient = { ...this.selectedIng } as Ingredient;
    i.ingredientId = this.selectedIng?.ingredientId;
    i.recipeId = this.recipeId;

    let apiRequest: Observable<RecipeIngredient>;

    if (this.ingredientEditIndex >= 0 && !!i.recipeIngredientId) {
      apiRequest = this.recipeService.updateRecipeIngredient(i);
    } else {
      apiRequest = this.recipeService.addRecipeIngredient(i);
    }

    apiRequest.subscribe(
      data => {

        if (this.ingredientEditIndex == -1) {
          data.quantityText = data.quantity % 1 == 0 ? data.quantity.toString() : this.mathUtil.decimalToFraction(data.quantity);
          this.ingredients.push(data);
        } else {
          // update recipeIngredientID if it may have changed (due to changing ingredients)
          i.recipeIngredientId = data.recipeIngredientId;
        }

        this.method = "";
        this.selectedIng = null;
        this.unitOfMeasure = "";
        this.ingredientEditIndex = -1;
      },
      error => {
        this.toastService.showDanger("Failed to add ingredient - " + error.error);
      }
    );


  }

  editIngredient(ingredient: RecipeIngredient, index: number, event) {
    this.selectedIng = ingredient.ingredient;
    this.ingredientQtyText = ingredient.quantityText;
    this.unitOfMeasure = ingredient.unitOfMeasure;
    this.method = ingredient.method;
    this.ingredientEditIndex = index;
    this.selectedIngredientIdx = -1;
    event.stopImmediatePropagation();
    event.preventDefault();
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


  editStep(step: RecipeStep, index: number, event) {
    this.stepEditIndex = index;
    this.selectedStepIdx = -1;
    this.stepText = step.text;
    event.preventDefault();
    event.stopImmediatePropagation();
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

  scrapeUrl() {
    if (!!!this.recipeUrl) {
      return;
    }

    this.isSaving = true;

    this.scraper.scrapeUrl(this.recipeUrl).subscribe(r => {

      if (!!r) {
        this.isScraped = true;
        this.name = r.name;
        this.description = r.description;
        this.prepTime = r.prepTime;
        this.cookTime = r.cookTime;
        this.servingSize = r.servingSize;
        this.isPublic = r.isPublic;
        this.steps = r.steps;
        this.ingredients = r.ingredients;
        this.ingredients.forEach(i => i.quantityText = i.quantity % 1 == 0 ? i.quantity.toString() : this.mathUtil.decimalToFraction(i.quantity));
        this.isSaving = false;
      }
    },
      error => {
        this.toastService.showDanger(error.error, 'Failed Scrape');
        this.isSaving = false;
      });
  }

  ingredientClicked(index: number) {
    if (!this.isEditing) {
      return;
    }
    this.selectedIngredientIdx = this.selectedIngredientIdx == index ? -1 : index;
  }

  stepClicked(index: number) {
    if (!this.isEditing) {
      return;
    }
    this.selectedStepIdx = this.selectedStepIdx == index ? -1 : index;
  }

  confirmDelete() {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete ${this.name}?`,
      header: 'Confirm Delete',
      icon: 'pi pi-trash',
      accept: () => {
        this.recipeService.deleteRecipe(this.originalRecipe).subscribe(
          data => {
            this.toastService.showSuccess(`Recipe deleted!`);
            this.router.navigate(['/recipe', 0]);
          },
          error => {
            this.toastService.showDanger(`Failed to delete recipe - ${error.error}`);
          }
        );
      }
    });
  }

}
