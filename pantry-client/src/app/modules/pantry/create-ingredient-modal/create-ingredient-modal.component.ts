import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import Category from '../../../data/models/Category';
import { ToastService } from '../../../shared/services/toast.service';
import Ingredient from '../../../data/models/Ingredient';
import IngredientApi from '../../../data/services/ingredientApi.service';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';

@Component({
  selector: 'app-create-ingredient-modal',
  templateUrl: './create-ingredient-modal.component.html',
  styleUrls: ['./create-ingredient-modal.component.css']
})
export class CreateIngredientModalComponent implements OnInit, OnChanges {

  @Input()
  public ingredientName: string;

  @Output()
  public closeDialog: EventEmitter<Ingredient> = new EventEmitter<Ingredient>(null);

  public addMode: string = "Pantry"; // "Pantry" or "GroceryList"
  public activeList: KitchenList;
  
  public name: string;
  public description: string;
  public selectedCategoryId: number;
  public isPublic: boolean = false;
  public isAddToPantry: boolean = false;
  public quantity: number;
  public notes: string;
  public isAdding: boolean;

  public categories: Array<Category>;

  public searchText: string;
  public searchResults: Category[] = [];

  constructor(
    private toastService: ToastService,
    private apiService: IngredientApi,
    private listIngredientService: ListIngredientApiService,
    private kitchenIngredientApi: KitchenIngredientApi,
    private activeKitchen: ActiveKitchenService) { }

  ngOnChanges(changes: SimpleChanges): void {
    const isString: boolean = changes['ingredientName'] && typeof changes['ingredientName'].currentValue === 'string';

    if (changes['ingredientName'] && changes['ingredientName'].currentValue !== "" && isString) {
      this.name = changes['ingredientName'].currentValue;
    }
  }

  ngOnInit(): void {
    
    this.name = this.ingredientName;
    this.description = "";
    this.isPublic = true;
    this.isAddToPantry = true;
    this.isAdding = false;
    this.categories = [];
    this.selectedCategoryId = -1;
    this.quantity = 1;
    this.notes = "";
    

    this.apiService.getIngredientCategories().subscribe(
      data => {
        this.categories = data;
      },
      error => {
        this.toastService.showDanger("Failed to load categories. re-open this modal to try again - " + error.error);
      });

  }

  confirmAdd(): void {

    if (this.name === "") {
      this.toastService.showDanger("Name is required.");
      return;
    }

    if (this.selectedCategoryId === null || this.selectedCategoryId === undefined || this.selectedCategoryId <= 0) {
      this.toastService.showDanger("Category is required.");
      return;
    }

    this.isAdding = true;

    const toAdd: Ingredient = new Ingredient();
    toAdd.name = this.name;
    toAdd.description = this.description;
    toAdd.categoryId = this.selectedCategoryId;
    toAdd.isPublic = this.isPublic;

    this.apiService.addIngredient(toAdd).subscribe(
      data => {
        this.toastService.showSuccess("Successfully created ingredient - " + toAdd.name);
        this.closeDialog.emit(data);

        if (this.isAddToPantry) {
          if (this.addMode === 'Pantry') {
            this.addToPantry(data);
          } else {
            this.addToGroceryList(data);
          }
        } else {
          this.isAdding = false;
        }

      },
      error => {
        this.toastService.showDanger("Could not create ingredient - " + error.error);
        this.isAdding = false;
      });
  }

  private addToGroceryList(x: Ingredient) {
    const toAdd: ListIngredient = this.listIngredientService.createEmpty(x, this.activeList);

    if (toAdd.kitchenId === 0 || toAdd.kitchenListId === 0) {
      this.toastService.showDanger("Cannot add to list - id is 0");
      return;
    }

    this.listIngredientService.addIngredientToList(toAdd).subscribe(data => {
      this.listIngredientService.setAddedIngredient(data);
      this.toastService.showSuccess("Added " + x.name);
      this.isAdding = false;
    },
      error => {
        this.toastService.showDanger(error.error);
        this.listIngredientService.setAddedIngredient(null);
        this.isAdding = false;
      },
    );
  }

  private addToPantry(toAdd: Ingredient) {
    const toPantry: KitchenIngredient = this.kitchenIngredientApi.createEmpty(toAdd, this.activeKitchen.getActiveKitchenId());
    toPantry.quantity = this.quantity;
    toPantry.note = this.notes;

    this.kitchenIngredientApi.addIngredientToKitchen(toPantry).subscribe(
      data => {
        this.kitchenIngredientApi.setAddedIngredient(data);
        this.toastService.showSuccess("Added to pantry - " + toAdd.name);
        this.isAdding = false;
      },
      error => {
        this.toastService.showDanger("Could not add " + toAdd.name + " to pantry - " + error.error);
        this.isAdding = false;
      });
  }

  close(): void {
    this.closeDialog.emit(null);
  }

  search(event) {
    this.searchResults = this.categories.filter(c => c.name.toLowerCase().includes(event.query.toLowerCase()));

    if (this.searchResults.length === 0) {
      this.searchResults = [
        {
          name: 'Create New Category',
          categoryId: 0,
          categoryTypeId: 0,
          categoryTypeName: '',
          createdByKitchenId: 0
        }
      ]
    }
  }

  addSelected(value: Category) {
    if (value.name === 'Create New Category') {
      // TODO create category
    } else {
      this.selectedCategoryId = value.categoryId;
    }
  }

}
