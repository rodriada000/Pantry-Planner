import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { CreateIngredientModalComponent } from './create-ingredient-modal/create-ingredient-modal.component';
import { ActiveKitchenService } from '../../shared/services/active-kitchen.service';
import KitchenUserApi from '../../data/services/kitchenUserApi.service';
import { ToastService } from '../../shared/services/toast.service';
import KitchenApi from '../../data/services/kitchenApi.service';
import { BehaviorSubject, Subscription } from 'rxjs';
import { skipWhile, skipUntil } from 'rxjs/operators';
import { PantryPageService } from './pantry-page.service';
import { LayoutService } from 'src/app/shared/services/layout-service.service';

@Component({
  selector: 'pantry-root',
  templateUrl: './pantry.component.html',
  styleUrls: ['./pantry.component.css']
})
export class PantryComponent implements OnInit, OnDestroy {

  public isAddPageSelected: boolean;
  public isSearchPantryPageSelected: boolean;
  public isManagePantryPageSelected: boolean;
  public isLeavePantryPageSelected: boolean;
  public isOwnerOfKitchen: boolean;
  public activeKitchenName: string;
  public showSideMenu: boolean;

  
  private pageSelection: Subscription;
  private observingKitchen: Subscription;
  showNewIngredientDialog: boolean;
  
  public get sideMenuSize() {
    return this.layoutService.sideMenuSize;
  }

  constructor(
    // private modalService: NgbModal,
    private activeKitchenService: ActiveKitchenService,
    private toastService: ToastService,
    private layoutService: LayoutService,
    private kitchenUserApi: KitchenUserApi,
    private kitchenApi: KitchenApi,
    private pageService: PantryPageService) { 
    }

  ngOnInit(): void {
    this.showSideMenu = false;
    this.activeKitchenName = "";
    this.switchToAddIngredients();
    this.isOwnerOfKitchen = false;

    this.observingKitchen = this.activeKitchenService.observableKitchen.pipe(skipWhile(k => !k)).subscribe(k => {
      if (k !== null && k !== undefined) {
        this.activeKitchenName = k.name;

        if (this.isManagePantryPageSelected || this.isLeavePantryPageSelected) {
          this.switchToAddIngredients(); // don't leave user on manage/leave pantry page after they switch kitchens in case their ownership changes 
        }

        this.kitchenUserApi.isOwnerOfKitchen(k.kitchenId).subscribe(
          data => {
            this.isOwnerOfKitchen = data;
          }
        );
      }
    });

    this.pageSelection = this.pageService.observableSelectedPages.subscribe(p => {
      if (p !== null && p !== undefined) {
        this.isAddPageSelected = this.pageService.isAddPageSelected;
        this.isLeavePantryPageSelected = this.pageService.isLeavePantryPageSelected;
        this.isManagePantryPageSelected = this.pageService.isManagePantryPageSelected;
        this.isSearchPantryPageSelected = this.pageService.isSearchPantryPageSelected;
      }
    })

  }

  ngOnDestroy(): void {
    this.observingKitchen.unsubscribe();
    this.pageSelection?.unsubscribe();
  }

  public switchToAddIngredients(): void {
    this.pageService.switchToAddIngredients();
    this.showSideMenu = false;
  }

  public switchToSearchPantry(): void {
    this.pageService.switchToSearchPantry();
    this.showSideMenu = false;
  }

  public switchToManagePantry(): void {
    this.pageService.switchToManagePantry();
    this.showSideMenu = false;
  }

  public switchToLeavePantry(): void {
    this.pageService.switchToLeavePantry();
    this.showSideMenu = false;
  }


  openCreateIngredientModal(): void {
    this.showNewIngredientDialog = true;
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }

  deleteKitchen(): void {
    this.kitchenApi.deleteKitchen(this.activeKitchenService.getActiveKitchenId()).subscribe(
      data => {
        this.activeKitchenService.clearActiveKitchen(false);
        window.location.reload();
      },
      error => {
        this.toastService.showDanger("Failed to delete pantry - " + error.error);
      }
    );
  }

  removeSelfFromKitchen(): void {
    this.kitchenUserApi.deleteSelfFromKitchen(this.activeKitchenService.getActiveKitchenId()).subscribe(
      data => {
        this.activeKitchenService.clearActiveKitchen(false);
        window.location.reload();
      },
      error => {
        this.toastService.showDanger("Failed to leave pantry - " + error.error);
      }
    );
  }

}
