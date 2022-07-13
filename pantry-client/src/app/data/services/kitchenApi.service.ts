import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import Kitchen from '../models/Kitchen';
import { UserLoginService } from 'src/app/shared/services/user-login.service';

@Injectable({
  providedIn: 'root'
})
export default class KitchenApi {
  public API = environment.baseUrl;
  public KITCHEN_ENDPOINT = `${this.API}/Kitchen`;

  constructor(private http: HttpClient, private userService: UserLoginService) { }

  getAllKitchens(): Observable<Array<Kitchen>> {
    return this.http.get<Array<Kitchen>>(this.KITCHEN_ENDPOINT, this.userService.authHeader);
  }

  addKitchen(kitchen: Kitchen): Observable<Kitchen> {
    return this.http.post<Kitchen>(this.KITCHEN_ENDPOINT, kitchen, this.userService.authHeader); 
  }

  deleteKitchen(kitchenId: number): Observable<Kitchen> {
    return this.http.delete<Kitchen>(this.KITCHEN_ENDPOINT + "/" + kitchenId.toString(), this.userService.authHeader);
  }
}
