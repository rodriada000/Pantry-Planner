import { Component, OnInit } from '@angular/core';
import { skipWhile, Subscription } from 'rxjs';
import KitchenApi from 'src/app/data/services/kitchenApi.service';
import KitchenUserApi from 'src/app/data/services/kitchenUserApi.service';
import { ActiveKitchenService } from 'src/app/shared/services/active-kitchen.service';
import { LayoutService } from 'src/app/shared/services/layout-service.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { PantryPageService } from '../pantry/pantry-page.service';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  public isMyRecipesSelected: boolean;
  public isSearchRecipesSelected: boolean;
  public isCreateRecipeSelected: boolean;
  public isOwnerOfKitchen: boolean;
  public activeKitchenName: string;
  public showSideMenu: boolean;

  
  private observingKitchen: Subscription;
  
  public get sideMenuSize() {
    return this.layoutService.sideMenuSize;
  }

  constructor(
    // private modalService: NgbModal,
    private activeKitchenService: ActiveKitchenService,
    private toastService: ToastService,
    private layoutService: LayoutService,
    private kitchenUserApi: KitchenUserApi,
    private kitchenApi: KitchenApi) { 
    }

  ngOnInit(): void {
    this.showSideMenu = true;
    this.activeKitchenName = "";
    this.switchToCreateRecipe();
    this.isOwnerOfKitchen = false;

    this.observingKitchen = this.activeKitchenService.observableKitchen.pipe(skipWhile(k => !k)).subscribe(k => {
      if (k !== null && k !== undefined) {
        this.activeKitchenName = k.name;

        this.kitchenUserApi.isOwnerOfKitchen(k.kitchenId).subscribe(
          data => {
            this.isOwnerOfKitchen = data;
          }
        );
      }
    });

  }

  ngOnDestroy(): void {
    this.observingKitchen.unsubscribe();
  }

  public switchToMyRecipes(): void {
    this.isMyRecipesSelected = true;
    this.isCreateRecipeSelected = false;
    this.isSearchRecipesSelected = false;
    this.showSideMenu = false;
  }

  public switchToSearchRecipes(): void {
    this.isSearchRecipesSelected = true;
    this.isCreateRecipeSelected = false;
    this.isMyRecipesSelected = false;
    this.showSideMenu = false;
  }

  public switchToCreateRecipe(): void {
    this.isCreateRecipeSelected = true;
    this.isMyRecipesSelected = false;
    this.isSearchRecipesSelected = false;
    this.showSideMenu = false;
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }


}
