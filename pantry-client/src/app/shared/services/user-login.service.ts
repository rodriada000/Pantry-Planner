import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, publishReplay, refCount, Subject } from 'rxjs';
import LoginModel from 'src/app/data/models/LoginModel';
import { environment } from 'src/environments/environment';
import { SocialAuthService, SocialUser } from "@abacritt/angularx-social-login";
import { GoogleLoginProvider } from "@abacritt/angularx-social-login";
import { ToastService } from './toast.service';
import { ActiveKitchenService } from './active-kitchen.service';

@Injectable({
  providedIn: 'root'
})
export class UserLoginService {


  public API = environment.baseUrl;
  public AUTH_ENDPOINT = `api/Authenticate`;

  private authChangeSub: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private extAuthChangeSub: BehaviorSubject<SocialUser> = new BehaviorSubject<SocialUser>(null);
  public authChanged = this.authChangeSub.asObservable();
  public extAuthChanged = this.extAuthChangeSub.asObservable();

  public token$: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  public get token(): string {
    return this.token$.value;
  };

  public get authHeader() {
    return { headers: { 'Authorization': `Bearer ${this.token}` } };
  }

  public get authHeaderOnly() {
    return { 'Authorization': `Bearer ${this.token}` };
  }

  constructor(private http: HttpClient, private router: Router, private toasts: ToastService, private kitchen: ActiveKitchenService, private ngZone: NgZone) {
    this.token$.next(localStorage.getItem("token"));

    if (!!!this.token) {



    }
  }

  // constructor(private http: HttpClient, private router: Router) { 
  //   this.token$.next(localStorage.getItem("token"));
  // }

  login(loginDto: LoginModel): Observable<any> {
    let sub = this.http.post<any>(`${this.AUTH_ENDPOINT}/Login`, loginDto).pipe(publishReplay(), refCount());

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
    let sub = this.http.post<any>(`api/GoogleTokenValidator/Login?idToken=${token}`, null).pipe(publishReplay(), refCount());

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

}
