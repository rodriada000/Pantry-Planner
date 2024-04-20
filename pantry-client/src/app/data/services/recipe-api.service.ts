import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import Recipe from '../models/Recipe';
import RecipeIngredient from '../models/RecipeIngredient';
import RecipeStep from '../models/RecipeStep';
import { UserLoginService } from 'src/app/shared/services/user-login.service';


@Injectable({
  providedIn: 'root'
})
export class RecipeApiService {
  public endPoint = `${environment.baseUrl}/Recipe`;
  public ingredientEndPoint = `${environment.baseUrl}/RecipeIngredient`;
  public stepEndPoint = `${environment.baseUrl}/RecipeStep`;

  constructor(private http: HttpClient, private userService: UserLoginService) { }

  getRecipesByName(name: string): Observable<Array<Recipe>> {
    return this.http.get<Array<Recipe>>(this.endPoint, {
      params: { 'name': name },
      headers: this.userService.authHeaderOnly
    });
  }

  getRecipeById(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(this.endPoint + '/' + id, {
      headers: this.userService.authHeaderOnly
    });
  }

  addRecipe(newRecipe: Recipe): Observable<Recipe> {
    return this.http.post<Recipe>(this.endPoint, newRecipe, this.userService.authHeader);
  }

  updateRecipe(updated: Recipe): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + updated.recipeId.toString(), updated, this.userService.authHeader);
  }

  deleteRecipe(toDelete: Recipe): Observable<Recipe> {
    return this.http.delete<Recipe>(this.endPoint + "/" + toDelete.recipeId.toString(), this.userService.authHeader);
  }


  addRecipeIngredient(toAdd: RecipeIngredient): Observable<RecipeIngredient> {
    return this.http.post<RecipeIngredient>(this.ingredientEndPoint, toAdd, this.userService.authHeader);
  }

  updateRecipeIngredient(updated: RecipeIngredient): Observable<any> {
    return this.http.put<any>(this.ingredientEndPoint + "/" + updated.recipeIngredientId.toString(), updated, this.userService.authHeader);
  }

  deleteRecipeIngredient(toDelete: RecipeIngredient): Observable<RecipeIngredient> {
    return this.http.delete<RecipeIngredient>(this.ingredientEndPoint + "/" + toDelete.recipeIngredientId.toString(), this.userService.authHeader);
  }


  addRecipeStep(toAdd: RecipeStep): Observable<RecipeStep> {
    return this.http.post<RecipeStep>(this.stepEndPoint, toAdd, this.userService.authHeader);
  }

  updateRecipeStep(updated: RecipeStep): Observable<any> {
    return this.http.put<any>(this.stepEndPoint + "/" + updated.recipeStepId.toString(), updated, this.userService.authHeader);
  }

  deleteRecipeStep(toDelete: RecipeStep): Observable<RecipeStep> {
    return this.http.delete<RecipeStep>(this.stepEndPoint + "/" + toDelete.recipeStepId.toString(), this.userService.authHeader);
  }

}
