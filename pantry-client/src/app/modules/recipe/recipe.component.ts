import { Component, OnInit } from '@angular/core';
import { skipWhile, Subscription } from 'rxjs';
import KitchenApi from 'src/app/data/services/kitchenApi.service';
import KitchenUserApi from 'src/app/data/services/kitchenUserApi.service';
import { ActiveKitchenService } from 'src/app/shared/services/active-kitchen.service';
import { LayoutService } from 'src/app/shared/services/layout-service.service';
import { ToastService } from 'src/app/shared/services/toast.service';
import { PantryPageService } from '../pantry/pantry-page.service';
import { ActivatedRoute, Router } from '@angular/router';
import Recipe from 'src/app/data/models/Recipe';

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
  recipeId: any;
  
  public get sideMenuSize() {
    return this.layoutService.sideMenuSize;
  }

  constructor(
    // private modalService: NgbModal,
    private activeKitchenService: ActiveKitchenService,
    private toastService: ToastService,
    private layoutService: LayoutService,
    private kitchenUserApi: KitchenUserApi,
    private route: ActivatedRoute,
    private router: Router,
    private kitchenApi: KitchenApi) { 
    }

  ngOnInit(): void {
    this.showSideMenu = true;
    this.activeKitchenName = "";
    this.isOwnerOfKitchen = false;
    this.switchToSearchRecipes();

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

    this.route.paramMap.subscribe(p => {
      if (p['params']['id']) {
        this.recipeId = p['params']['id'];
        if (this.route.snapshot.url.find(u => u.path === 'details')) {
          this.isSearchRecipesSelected = true;
          this.isCreateRecipeSelected = false;
        } else {
          this.isCreateRecipeSelected = true;
          this.isSearchRecipesSelected = false;
        }
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
    this.recipeId = null;
  }

  public switchToCreateRecipe(): void {
    this.isCreateRecipeSelected = true;
    this.isMyRecipesSelected = false;
    this.isSearchRecipesSelected = false;
    this.showSideMenu = false;
    this.recipeId = null;
    this.router.navigate(['/recipe', '0']);
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }

  viewRecipe(r: Recipe) {
    this.router.navigate(['/recipe', 'details', r.recipeId]);
    this.recipeId = r.recipeId; 
  }

}
