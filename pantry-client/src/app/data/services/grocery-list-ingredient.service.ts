import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { environment } from '../../../environments/environment';
import ListIngredient from '../models/ListIngredient';
import Ingredient from '../models/Ingredient';
import KitchenList from '../models/KitchenList';
import { UserLoginService } from 'src/app/shared/services/user-login.service';

@Injectable({
  providedIn: 'root'
})
export default class ListIngredientApiService {
  public endPoint = `${environment.baseUrl}/ListIngredient`;

  private addedIngredient: ListIngredient;
  public observableAddedIngredient: BehaviorSubject<ListIngredient>;

  constructor(private http: HttpClient, private userService: UserLoginService) {
    this.addedIngredient = null;
    this.observableAddedIngredient = new BehaviorSubject<ListIngredient>(this.addedIngredient);
  }

  public addIngredientToList(ingredient: ListIngredient): Observable<ListIngredient> {
    return this.http.post<ListIngredient>(this.endPoint, ingredient, this.userService.authHeader);
  }

  public updateListIngredient(ingredient: ListIngredient): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + ingredient.id.toString(), ingredient, this.userService.authHeader);
  }

  public removeListIngredient(ingredient: ListIngredient): Observable<ListIngredient> {
    return this.http.delete<ListIngredient>(this.endPoint + "/" + ingredient.id.toString(), this.userService.authHeader);
  }

  public removeCheckedIngredients(kitchenListId: number): Observable<any> {
    return this.http.delete<ListIngredient>(this.endPoint + "/Checked/" + kitchenListId.toString(), this.userService.authHeader);
  }


  public setAddedIngredient(ingredient: ListIngredient): void {
    this.addedIngredient = ingredient;
    this.observableAddedIngredient.next(this.addedIngredient);
  }

  public getIngredientsForList(listId: number): Observable<Array<ListIngredient>> {
    return this.http.get<Array<ListIngredient>>(this.endPoint, {
      params: { 'kitchenListId': listId?.toString() },
      headers: this.userService.authHeaderOnly
    });
  }

  public createEmpty(i: Ingredient, kitchenList: KitchenList): ListIngredient {
    const toAdd: ListIngredient = new ListIngredient();
    toAdd.ingredientId = i.ingredientId;
    toAdd.kitchenId = kitchenList.kitchenId;
    toAdd.kitchenListId = kitchenList.kitchenListId;
    toAdd.isChecked = false;
    toAdd.quantity = 1;

    return toAdd;
  }
}
