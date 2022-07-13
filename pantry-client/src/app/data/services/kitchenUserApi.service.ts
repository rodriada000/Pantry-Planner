import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import KitchenUser from '../models/KitchenUser';
import { UserLoginService } from 'src/app/shared/services/user-login.service';

@Injectable({
  providedIn: 'root'
})
export default class KitchenUserApi {
  public endPoint = `${environment.baseUrl}/KitchenUser`;

  constructor(private http: HttpClient, private userService: UserLoginService) { }

  getAllUsersForKitchen(kitchenId: number): Observable<Array<KitchenUser>> {
    return this.http.get<Array<KitchenUser>>(this.endPoint, {
      params: { 'kitchenId': kitchenId.toString() },
      headers: this.userService.authHeaderOnly
    });
  }

  getKitchenInvitesForLoggedInUser(): Observable<Array<KitchenUser>> {
    return this.http.get<Array<KitchenUser>>(this.endPoint + "/Invite", this.userService.authHeader);
  }

  inviteUserToKitchen(username: string, kitchenId: number): Observable<any> {
    return this.http.post<any>(this.endPoint + "/Invite", null, {
      params: {
        'username': username,
        'kitchenId': kitchenId.toString()
      },
      headers: this.userService.authHeaderOnly
    });
  }

  acceptKitchenInvite(kitchenId: number): Observable<any> {
    return this.http.put<any>(this.endPoint + "/Invite", null, {
      params: {
        'kitchenId': kitchenId.toString()
      },
      headers: this.userService.authHeaderOnly
    });
  }

  denyKitchenInvite(kitchenId: number): Observable<any> {
    return this.http.delete<any>(this.endPoint + "/Invite", {
      params: {
        'kitchenId': kitchenId.toString()
      },
      headers: this.userService.authHeaderOnly
    });
  }

  deleteKitchenUserByKitchenUserId(kitchenUserId: number): Observable<any> {
    return this.http.delete<any>(this.endPoint + "/" + kitchenUserId.toString(), this.userService.authHeader);
  }

  deleteSelfFromKitchen(kitchenId: number): Observable<KitchenUser> {
    return this.http.delete<KitchenUser>(this.endPoint, {
      params: {
        'kitchenId': kitchenId.toString()
      },
      headers: this.userService.authHeaderOnly
    });
  }

  isOwnerOfKitchen(kitchenId: number): Observable<boolean> {
    return this.http.get<boolean>(this.endPoint + "/IsOwner", {
      params: {
        'kitchenId': kitchenId.toString()
      },
      headers: this.userService.authHeaderOnly
    });
  }

}
