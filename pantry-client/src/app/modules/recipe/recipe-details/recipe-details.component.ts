import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ConfirmationService, MenuItem } from 'primeng/api';
import KitchenIngredient from 'src/app/data/models/KitchenIngredient';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import Recipe from 'src/app/data/models/Recipe';
import RecipeIngredient from 'src/app/data/models/RecipeIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';
import GroceryListApi from 'src/app/data/services/grocery-list.service';
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

  menuItems: MenuItem[] = [];
  groceryList: KitchenList[] = [];
  selectedGroceryListId: number = -1;

  steps: any[] = [];
  kitchenIngredients: KitchenIngredient[] = [];

  constructor(private recipeService: RecipeApiService,
    private mathUtil: MathUtilService,
    private kitchenApi: KitchenIngredientApi,
    private activeKitchen: ActiveKitchenService,
    private confirmationService: ConfirmationService,
    private listService: GroceryListApi,
    private groceryListItemService: ListIngredientApiService,
    private pantryService: KitchenIngredientApi,
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

    this.getGroceryLists();
  }

  getGroceryLists() {
    this.listService.getAllGroceryLists().subscribe(
      lists => {
        this.groceryList = lists ?? [];
        this.listService.setObservable(this.groceryList);
      },
      error => this.toastService.showDanger(error.error)
    );
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

  refreshMenuOptions(ingredient: RecipeIngredient) {
    this.menuItems = [];

    if (this.ingredientInKitchen(ingredient.ingredientId)) {
      this.menuItems.push({ label: 'Remove Ingredient from Pantry', icon: 'pi pi-sign-out', command: (event) => { event.originalEvent.stopImmediatePropagation(); event.originalEvent.preventDefault(); this.confirmRemoveFromPantry(ingredient); }});
    } else {
      this.menuItems.push({ label: 'Add Ingredient to Pantry', icon: 'pi pi-sign-in', command: (event) => { event.originalEvent.stopImmediatePropagation(); event.originalEvent.preventDefault(); this.confirmAddToPantry(ingredient); } });
    }

    if (this.groceryList.length > 0) {
      this.menuItems.push({ label: 'Add Ingredient to Grocery List', icon: 'pi pi-shopping-cart', command: (event) => { event.originalEvent.stopImmediatePropagation(); event.originalEvent.preventDefault(); this.confirmAddToGroceryList(ingredient); }});
    }
  }

  confirmAddToPantry(recipeIngr: RecipeIngredient) {
    this.confirmationService.confirm({
      message: `Are you sure you want to add ${recipeIngr?.ingredient?.name} to the pantry?`,
      header: 'Confirm Add',
      icon: 'pi pi-info-circle',
      accept: () => {
        let k = this.pantryService.createEmpty(recipeIngr.ingredient, this.activeKitchen.activeKitchenId);
        k.quantity = recipeIngr.quantity ?? 1;
  
        this.pantryService.addIngredientToKitchen(k, true).subscribe(data => {
          this.pantryService.setAddedIngredient(data);
          this.kitchenIngredients.push(data);
        }, error => {
          this.toastService.showDanger(`could not add ${recipeIngr.ingredient?.name} to pantry - ${error.error}`);
        });
      }
    });
  }

  confirmRemoveFromPantry(recipeIngr: RecipeIngredient) {
    this.confirmationService.confirm({
      message: `Are you sure you want to remove ${recipeIngr?.ingredient?.name} from the pantry?`,
      header: 'Confirm Removal',
      icon: 'pi pi-trash',
      accept: () => {
        const k = this.kitchenIngredients.find(ki => ki.ingredientId === recipeIngr.ingredientId);

        this.pantryService.removeKitchenIngredient(k).subscribe(data => {
          const i: number = this.kitchenIngredients.findIndex(i => i.kitchenIngredientId == k.kitchenIngredientId);
          this.kitchenIngredients.splice(i, 1);
        }, error => {
          this.toastService.showDanger(`could not remove ${recipeIngr.ingredient?.name} from pantry - ${error.error}`);
        });
      }
    });
  }

  confirmAddToGroceryList(recipeIngr: RecipeIngredient) {
    this.confirmationService.confirm({
      key: 'add-to-grocery',
      message: `Select the grocery list you want to add ${recipeIngr?.ingredient?.name} to.`,
      header: 'Confirm Add',
      icon: 'pi pi-shopping-cart',
      accept: () => { 
        if (this.selectedGroceryListId == -1) {
          this.toastService.showDanger(`You must select a grocery list first`);
        }

        let selectedList = this.groceryList.find(g => g.kitchenListId == this.selectedGroceryListId);
        let toAdd = this.groceryListItemService.createEmpty(recipeIngr.ingredient, selectedList);
        toAdd.quantity = recipeIngr.quantity;

        this.groceryListItemService.addIngredientToList(toAdd).subscribe(
          data => {
            this.toastService.showSuccess(`Added ${recipeIngr.ingredient?.name} to ${selectedList.name} list`);
          },
          error => {
            this.toastService.showDanger(`could not add ${recipeIngr.ingredient?.name} to list - ${error.error}`);
          }
        );
      }
    });
  }

}
