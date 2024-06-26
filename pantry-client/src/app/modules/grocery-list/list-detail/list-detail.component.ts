import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { forkJoin, Observable, publishReplay, refCount, Subscription } from 'rxjs';
import Category from 'src/app/data/models/Category';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';
import KitchenIngredientApi from 'src/app/data/services/kitchenIngredientApi.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'grocery-list-detail',
  templateUrl: './list-detail.component.html',
  styleUrls: ['./list-detail.component.css']
})
export class ListDetailComponent implements OnInit, OnDestroy, OnChanges {

  public sortItems: MenuItem[] = [
    {
      label: 'Name', command: () => {
        this.sortBy('name', false)
      }
    },
    {
      label: 'Category', command: () => {
        this.sortBy('category', false)
      }
    }
  ];

  @Input()
  public selected: KitchenList;
  public selectedName: string = "";


  public allIngredients: Array<ListIngredient> = [];
  public filteredList: Array<ListIngredient> = [];

  public lastCatSearch: string;
  public filterText: string = "";
  public selectedSortOrder: number = 1;
  public selectedSort: string = "name";
  public hoveredIndex: number;
  public isLoading: boolean;
  public isEditing: boolean = false;
  public isSaving: boolean = false;
  
  private itemAddedSub: Subscription;
  origIngredient: ListIngredient;

  menuItems: MenuItem[] = [];

  get hasCheckedItems(): boolean {
    return this.allIngredients.some(i => i.isChecked);
  }

  get categories(): string[] {
    let cats: string[] = [];
    this.allIngredients.forEach(i => {
      if (!cats.includes(i.ingredient.categoryName)) {
        cats.push(i.ingredient.categoryName);
      }
    });

    return cats;
  }

  constructor(
    private service: ListIngredientApiService,
    private pantryService: KitchenIngredientApi,
    private toasts: ToastService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selected'] && changes['selected'].currentValue !== null && changes['selected'].currentValue !== undefined) {
      this.selected = changes['selected'].currentValue;
      this.selectedName = this.selected.name;
      this.refreshList();
    }
  }


  ngOnInit(): void {

    this.refreshMenuOptions();

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.service.observableAddedIngredient.subscribe(newIngredient => {
      if (newIngredient !== null && newIngredient !== undefined) {
        this.allIngredients.push(newIngredient);
        this.sortBy(this.selectedSort);
        this.doFilter();
      }
    });

    this.refreshList();
  }

  refreshMenuOptions() {
    this.menuItems = [
      { label: 'Add Checked Items to Pantry', icon: 'pi pi-sign-in', command: (event) => { event.originalEvent.stopImmediatePropagation(); event.originalEvent.preventDefault(); this.confirmAddToPantry(); }, disabled: !this.hasCheckedItems },
      { label: 'Remove Checked Items', icon: 'pi pi-check-square', command: () => this.removeChecked(), disabled: !this.hasCheckedItems },
      { label: 'Uncheck All Items', icon: 'pi pi-clone', command: () => this.uncheckAll(), disabled: !this.hasCheckedItems },
    ];
  }

  ngOnDestroy(): void {
    this.itemAddedSub?.unsubscribe();
  }

  refreshList(): void {
    if (this.selected === null || this.selected === undefined) {
      return;
    }

    this.isLoading = true;

    this.service.getIngredientsForList(this.selected?.kitchenListId).subscribe(data => {
      this.allIngredients = data;


      this.sortBy(this.selectedSort)
      this.doFilter();
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  setSelected(index: number, $event) {
    if (this.hoveredIndex != index) {
      this.cancelEdit(this.filteredList[this.hoveredIndex], $event);
      this.hoveredIndex = index;
    }
  }

  unselect(index: number, $event) {
    if (index == this.hoveredIndex) {
      this.hoveredIndex = -1;
      $event.preventDefault();
      $event.stopPropagation();
    }
  }

  toggleSortOrder() {
    this.sortBy(this.selectedSort, true);
    this.doFilter();
  }

  uncheckAll() {

    this.confirmationService.confirm({
      message: 'Are you sure you want to uncheck all?',
      key: 'confirm-check',
      header: 'Confirm Uncheck',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.allIngredients.filter(i => i.isChecked).forEach(i => {
          i.isChecked = !i.isChecked;
          this.updateIngredient(i);
        });
      }
    });


  }

  toggleChecked(ingredient: ListIngredient) {
    ingredient.isChecked = !ingredient.isChecked;
    this.updateIngredient(ingredient);
    this.sortBy(this.selectedSort);
    this.doFilter();
  }

  updateIngredient(ingredient: ListIngredient, showToast: boolean = false) {
    this.service.updateListIngredient(ingredient).subscribe(response => {
      if (showToast) {
      }
    },
    error => { this.toasts.showDanger(error.message + " - " + error.error); })
  }

  removeFromList(ingredient: ListIngredient, index: number, showToast: boolean = true) {
    this.service.removeListIngredient(ingredient).subscribe(data => {
      if (showToast) {
        this.toasts.showStandard("Removed " + ingredient.ingredient.name + " from list.");
      }

      this.allIngredients.splice(index, 1);
      this.sortBy(this.selectedSort);
      this.doFilter();
    },
    error => { this.toasts.showDanger(error.message + " - " + error.error); })
  }

  doFilter() {
    if (this.allIngredients === null || this.allIngredients === undefined) {
      this.filteredList = [];
      return;
    }

    this.filteredList = this.allIngredients.filter(p => this.filterText === "" || (p.ingredient.name.toLowerCase().includes(this.filterText.toLowerCase())));

    if (this.hoveredIndex >= this.filteredList.length) {
      this.hoveredIndex = -1;
    }
  }

  sortBy(field: string, toggleSort: boolean = false): void {
    field = field.toLowerCase();
    if (toggleSort) {
      if (this.selectedSort.toLowerCase() === field) {
        // toggle sort asc/desc when clicking same field already sorted by
        this.selectedSortOrder *= -1;
      } else {
        this.selectedSortOrder = 1; // set back to A->Z
      }
    }

    this.selectedSort = field;

    let checkedItems = this.allIngredients.filter(i => i.isChecked).sort((a, b) => this.sortFn(a,b));
    let uncheckedItems = this.allIngredients.filter(i => !i.isChecked).sort((a, b) => this.sortFn(a,b));


    this.allIngredients = uncheckedItems.concat(checkedItems);
  }

  sortFn(a: ListIngredient, b: ListIngredient) {
    let valA: string = a.ingredient.name;
    let valB: string = b.ingredient.name;

    if (this.selectedSort == 'category') {
      valA = a.ingredient.categoryName;
      valB = b.ingredient.categoryName;
    }

    if (valA.toLowerCase() > valB.toLowerCase()) {
      return 1 * this.selectedSortOrder;
    } else if (valA.toLowerCase() < valB.toLowerCase()) {
      return -1 * this.selectedSortOrder;
    }

    if (this.selectedSort == 'category') {
      if (a.ingredient.name.toLowerCase() > b.ingredient.name.toLowerCase()) {
        return 1 * this.selectedSortOrder;
      } else if (a.ingredient.name.toLowerCase() < b.ingredient.name.toLowerCase()) {
        return -1 * this.selectedSortOrder;
      }
    }

    return 0;
  }

  getIngredientsByCategory(category: string) {
    return this.allIngredients.filter(i => i.ingredient.categoryName.toLowerCase() == category.toLowerCase());
  }

  confirmAddToPantry() {
    this.confirmationService.confirm({
      message: 'Are you sure you want to add checked items to the pantry?',
      header: 'Confirm Add',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.addCheckedToPantry();
      }
    });
  }

  addCheckedToPantry() {

    let observables: any[] = [];
    
    this.allIngredients.forEach(i => {
      if (!i.isChecked) {
        return;
      }

      let k = this.pantryService.createEmpty(i.ingredient, this.selected.kitchenId);
      k.quantity = i.quantity ?? 1;

      let addObs = this.pantryService.addIngredientToKitchen(k, true).pipe(publishReplay(), refCount());

      addObs.subscribe(data => {
        this.pantryService.setAddedIngredient(data);
      }, error => {
        this.toasts.showDanger('could not add ' + i.ingredient?.name + ' to pantry - ' + error.error);
      });

      observables.push(addObs);
    })

    forkJoin(observables).subscribe(o => {
      this.removeChecked(true);
    })
  }

  editIngredient(ingredient: ListIngredient, $event): void {
    this.isEditing = true;
    this.origIngredient = { ...ingredient };
    $event.preventDefault();
    $event.stopPropagation();
  }

  quickEditQty(ingredient: ListIngredient, qtyToAdd: number) {
    if (this.isSaving) {
      return;
    }

    if (ingredient.quantity + qtyToAdd <= 0) {
      return; // cant have 0 or negative qty
    }

    ingredient.quantity += qtyToAdd;
    this.saveEdit(ingredient, false);
  }

  saveEdit(ingredient: ListIngredient, showToast: boolean = true): void {
    if (this.isSaving) {
      return;
    }
    
    if (ingredient.quantity <= 0) {
      return; // cant have 0 or negative qty
    }

    this.isSaving = true;

    this.service.updateListIngredient(ingredient).subscribe(data => {
      if (showToast) {
        this.toasts.showSuccess("Updated " + ingredient.ingredient.name);
      }
      this.isEditing = false;
      this.isSaving = false;
    },
      error => {
        this.toasts.showDanger("Could not update - " + error.error);
        this.isEditing = false;
        this.isSaving = false;
      });
  }

  cancelEdit(ingredient: ListIngredient, $event): void {
    if (ingredient === null || ingredient === undefined || !this.isEditing) {
      return;
    }

    ingredient.note = this.origIngredient?.note;
    ingredient.quantity = this.origIngredient?.quantity;
    this.origIngredient = null;
    this.isEditing = false;
    $event.preventDefault();
    $event.stopPropagation();
  }

  removeChecked(afterAdd: boolean = false) {

    let msg = 'Are you sure you want to remove all checked items from the list?';

    if (afterAdd) {
      msg = 'Do you also want to remove all checked items from the list?';
    }

    this.confirmationService.confirm({
      message: msg,
      key: 'confirm-check',
      header: 'Confirm Clear',
      icon: 'pi pi-info-circle',
      accept: () => {
        this.service.removeCheckedIngredients(this.selected.kitchenListId).subscribe(resp => {
          this.allIngredients = this.allIngredients.filter(i => !i.isChecked);
          this.sortBy(this.selectedSort);
          this.doFilter();
        },
        error => {
          this.toasts.showDanger('Could not remove checked items from list: ' + error.message ,'Update Failed')
        })
      }
    });
    

  }


}
