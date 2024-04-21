import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import Recipe from 'src/app/data/models/Recipe';
import { RecipeApiService } from 'src/app/data/services/recipe-api.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-recipe-search',
  templateUrl: './recipe-search.component.html',
  styleUrls: ['./recipe-search.component.css']
})
export class RecipeSearchComponent implements OnInit {

  @Output() 
  recipeSelected = new EventEmitter<Recipe>();

  searchText: string = '';
  results: Recipe[] = [];

  constructor(private recipeService: RecipeApiService,
    private route: ActivatedRoute,
    private toastService: ToastService) { }

  ngOnInit(): void {
    this.searchText = '';
  }

  searchKeyPress(event: KeyboardEvent) {
    if (event.key == 'Enter') {
      this.doSearch();
    }
  }

  doSearch() {
    this.recipeService.getRecipesByName(this.searchText).subscribe(
      data => {
        this.results = data;
      },
      error => {
        this.toastService.showDanger("Failed to search for recipes - " + error.error);
      }
    );
  }

  viewRecipe(r: Recipe) {
    this.recipeSelected.emit(r);
  }

}
