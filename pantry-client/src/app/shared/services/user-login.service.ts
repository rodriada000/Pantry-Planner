import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, publishReplay, refCount, Subject } from 'rxjs';
import LoginModel from 'src/app/data/models/LoginModel';
import { environment } from 'src/environments/environment';
import { SocialAuthService, SocialUser } from "@abacritt/angularx-social-login";
import { GoogleLoginProvider } from "@abacritt/angularx-social-login";
import { ToastService } from './toast.service';

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
    return { headers: { 'Authorization' :`Bearer ${this.token}`}};
  }

  public get authHeaderOnly() {
    return { 'Authorization' :`Bearer ${this.token}`};
  }

  constructor(private http: HttpClient, private externalAuthService: SocialAuthService, private router: Router, private toasts: ToastService) { 
      this.token$.next(localStorage.getItem("token"));

      this.externalAuthService.authState.subscribe((user) => {
        console.log(user)
        this.extAuthChangeSub.next(user);
        this.externalLogin(user.idToken)
        .subscribe({
          next: (res) => {
        },
          error: (err: HttpErrorResponse) => {
            this.toasts.showDanger(err.message, "Google Login Failed");
            this.signOutExternal();
          }
        });
      })
    }

  // constructor(private http: HttpClient, private router: Router) { 
  //   this.token$.next(localStorage.getItem("token"));
  // }

  login(loginDto: LoginModel): Observable<any> {
    let sub = this.http.post<any>(`${this.AUTH_ENDPOINT}/Login`, loginDto).pipe(publishReplay(), refCount());
    
    sub.subscribe(resp => {
      localStorage.setItem("token", resp.token);
      this.token$.next(resp.token);
      this.router.navigate(['/pantry']);
    });

    return sub;
  }

  logout() {
    localStorage.removeItem('token');
    this.token$.next("");
  }

  public loginWithGoogle() {
    console.log('google login', this.externalAuthService);
    this.externalAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }

  public signOutExternal() {
    this.externalAuthService.signOut();
    this.logout();
  }

  externalLogin(token: string,) {
    let sub = this.http.post<any>(`api/GoogleTokenValidator/Login?idToken=${token}`, null).pipe(publishReplay(), refCount());
    
    sub.subscribe(resp => {
      localStorage.setItem("token", resp.token);
      this.token$.next(resp.token);
      this.sendAuthStateChangeNotification(resp.isAuthSuccessful);
      this.router.navigate(['/pantry']);
    });

    return sub;
  }

  sendAuthStateChangeNotification(isAuthSuccessful: any) {
    throw new Error('Method not implemented.');
  }

}
