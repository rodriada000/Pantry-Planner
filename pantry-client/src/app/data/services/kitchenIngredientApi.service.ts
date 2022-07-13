import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenIngredient from '../models/KitchenIngredient';
import Ingredient from '../models/Ingredient';
import { UserLoginService } from 'src/app/shared/services/user-login.service';

@Injectable({
  providedIn: 'root'
})
export default class KitchenIngredientApi {
  public endPoint = `${environment.baseUrl}/KitchenIngredient`;

  private addedIngredient: KitchenIngredient;
  public observableAddedIngredient: BehaviorSubject<KitchenIngredient>;

  constructor(private http: HttpClient, private userService: UserLoginService) {
    this.addedIngredient = null;
    this.observableAddedIngredient = new BehaviorSubject<KitchenIngredient>(this.addedIngredient);
  }

  public addIngredientToKitchen(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.post<KitchenIngredient>(this.endPoint, ingredient, this.userService.authHeader);
  }

  public updateKitchenIngredient(ingredient: KitchenIngredient): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + ingredient.kitchenIngredientId.toString(), ingredient, this.userService.authHeader);
  }

  public removeKitchenIngredient(ingredient: KitchenIngredient): Observable<KitchenIngredient> {
    return this.http.delete<KitchenIngredient>(this.endPoint + "/" + ingredient.kitchenIngredientId.toString(), this.userService.authHeader);
  }

  public setAddedIngredient(ingredient: KitchenIngredient): void {
    this.addedIngredient = ingredient;
    this.observableAddedIngredient.next(this.addedIngredient);
  }

  public getIngredientsForKitchen(kitchenId: number): Observable<Array<KitchenIngredient>> {
    return this.http.get<Array<KitchenIngredient>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() },
      headers: this.userService.authHeaderOnly
    });
  }

  public createEmpty(i: Ingredient, kitchenId: number): KitchenIngredient {
    const toAdd: KitchenIngredient = new KitchenIngredient();
    toAdd.ingredientId = i.ingredientId;
    toAdd.categoryId = i.categoryId;
    toAdd.kitchenId = kitchenId;
    toAdd.note = "";
    toAdd.quantity = 1;

    return toAdd;
  }
}
