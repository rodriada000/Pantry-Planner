import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserLoginService } from 'src/app/shared/services/user-login.service';
import { environment } from 'src/environments/environment';
import Recipe from '../models/Recipe';
import { EMPTY, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecipeScraperService {

  public endPoint = `${environment.baseUrl}/RecipeScrape`;

  constructor(private http: HttpClient, private userService: UserLoginService) { }
  
  scrapeUrl(url: string): Observable<Recipe> {
    if (url.includes("allrecipes.com")) {
      return this.scrapeFromAllRecipes(url);
    }

    return EMPTY;
  }

  scrapeFromAllRecipes(url: string): Observable<Recipe> {
    return this.http.get<Recipe>(this.endPoint + '/AllRecipes', {
      headers: this.userService.authHeaderOnly,
      params: { 'url': url }
    });
  }
}
