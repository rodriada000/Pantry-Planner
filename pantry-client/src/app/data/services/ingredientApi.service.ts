import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Ingredient } from '../models/Ingredient';
import { Category } from '../models/Category';
import { UserLoginService } from 'src/app/shared/services/user-login.service';

@Injectable({
  providedIn: 'root'
})
export class IngredientApi {
  public endPoint = `${environment.baseUrl}/Ingredient`;

  constructor(private http: HttpClient, private userService: UserLoginService) { }

  getIngredientsByName(name: string): Observable<Array<Ingredient>> {
    return this.http.get<Array<Ingredient>>(this.endPoint, {
      params: { 'name': name }
    });
  }

  getIngredientCategories(): Observable<Array<Category>> {
    return this.http.get<Array<Category>>(this.endPoint + "/Category");
  }

  addIngredient(newIngred: Ingredient): Observable<Ingredient> {
    return this.http.post<Ingredient>(this.endPoint, newIngred);
  }

  updateIngredient(newIngred: Ingredient): Observable<any> {
    return this.http.put<any>(this.endPoint + "/" + newIngred.ingredientId.toString(), newIngred);
  }

  deleteIngredient(ingredientId: number): Observable<Ingredient> {
    return this.http.delete<Ingredient>(this.endPoint + "/" + ingredientId.toString());
  }


}
