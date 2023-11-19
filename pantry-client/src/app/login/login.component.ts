import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import LoginModel from '../data/models/LoginModel';
import { ToastService } from '../shared/services/toast.service';
import { UserLoginService } from '../shared/services/user-login.service';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public loginModel: LoginModel = new LoginModel();

  public isLoading: boolean = false;

  constructor(private loginService: UserLoginService, private toasts: ToastService, private router: Router) { }

  ngOnInit(): void {

    if (document.readyState == 'complete') {
      this.initGoogle();
    } else {
      // @ts-ignore
      window.onGoogleLibraryLoad = () => {
        this.initGoogle();
      };
    }

  }

  initGoogle() {
    // @ts-ignore
    google.accounts.id.initialize({
      // Ref: https://developers.google.com/identity/gsi/web/reference/js-reference#IdConfiguration
      client_id: '877234124976-td734q5t3ovh7o9uk6inofr6nabiiofh.apps.googleusercontent.com',
      callback: this.handleGoogleLogin.bind(this),
      auto_select: false,
      cancel_on_tap_outside: false
    });

    const parent = document.getElementById('google_btn');
    // @ts-ignore
    google.accounts.id.renderButton(parent, { theme: "filled_blue" });

    this.loginService.token$.subscribe(t => {
      if (!!!t) {
        // @ts-ignore
        google.accounts.id.prompt();
      }
    })
  }

  handleGoogleLogin(response: CredentialResponse) {
    this.isLoading = true;
    this.loginService.externalLogin(response.credential).subscribe(r => {
      this.isLoading = false;
    });
  }

  public login(): void {
    this.isLoading = true;

    this.loginService.login(this.loginModel).subscribe(resp => {
      this.isLoading = false;
    },
      error => {
        this.toasts.showDanger("Check that your username and password is correct.", "Login Failed");
        this.isLoading = false;
      });
  }

}

