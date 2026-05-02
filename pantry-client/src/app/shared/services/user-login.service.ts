import { HttpClient } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, publishReplay, refCount } from 'rxjs';
import { LoginModel } from 'src/app/data/models/LoginModel';
import { TokenDto } from 'src/app/data/models/TokenDto';
import { RefreshTokenRequest } from 'src/app/data/models/RefreshTokenRequest';
import { environment } from 'src/environments/environment';
import { SocialUser } from "@abacritt/angularx-social-login";
import { ToastService } from './toast.service';
import { ActiveKitchenService } from './active-kitchen.service';

@Injectable({
  providedIn: 'root'
})
export class UserLoginService {


  public API = environment.baseUrl;

  private authChangeSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private extAuthChangeSub: BehaviorSubject<SocialUser> = new BehaviorSubject<SocialUser>(null);
  public authChanged = this.authChangeSub.asObservable();
  public extAuthChanged = this.extAuthChangeSub.asObservable();

  public token$: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  public get token(): string {
    return this.token$.value;
  };

  constructor(private http: HttpClient, private router: Router, private toasts: ToastService, private kitchen: ActiveKitchenService, private ngZone: NgZone) {
    this.token$.next(localStorage.getItem("token"));
  }


  login(loginDto: LoginModel): Observable<any> {
    let sub = this.http.post<any>(`${this.API}/Authenticate/Login`, loginDto).pipe(publishReplay(), refCount());

    sub.subscribe(resp => {
      localStorage.setItem("token", resp.token);
      localStorage.setItem("token-expiration", resp.validTo);
      this.token$.next(resp.token);
      this.router.navigate(['/pantry']);
    });

    return sub;
  }

  logout() {
    localStorage.removeItem('token');
    this.kitchen.clearActiveKitchen(true);
    this.token$.next("");
  }


  public signOutExternal() {
    // @ts-ignore
    google?.accounts?.id?.disableAutoSelect();
    this.logout();
  }

  externalLogin(token: string,) {
    let sub = this.http.post<any>(`${this.API}/GoogleTokenValidator/Login?idToken=${token}`, null).pipe(publishReplay(), refCount());

    sub.subscribe(resp => {
      localStorage.setItem("token", resp.token);
      localStorage.setItem("token-expiration", resp.validTo);
      this.token$.next(resp.token);

      this.ngZone.run(() => {
        this.router.navigate(['/pantry']);
      });
    });

    return sub;
  }

  refreshToken(token: string): Observable<TokenDto> {
    const request = new RefreshTokenRequest();
    request.token = token;
    return this.http.post<TokenDto>(`${this.API}/Authenticate/RefreshToken`, request);
  }

  /**
   * Persist token to local storage and update observable
   */
  public setToken(token: string, validTo?: string | Date) {
    if (token) {
      localStorage.setItem('token', token);
      if (validTo) {
        localStorage.setItem('token-expiration', typeof validTo === 'string' ? validTo : validTo.toString());
      }
      this.token$.next(token);
    }
    else {
      localStorage.removeItem('token');
      localStorage.removeItem('token-expiration');
      this.token$.next(null);
    }
  }

}
