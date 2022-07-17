import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import LoginModel from '../data/models/LoginModel';
import { ToastService } from '../shared/services/toast.service';
import { UserLoginService } from '../shared/services/user-login.service';

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

  public loginWithGoogle() {
    this.isLoading = true;

    this.loginService.loginWithGoogle();

    this.loginService.extAuthChanged.subscribe(user => {

      if (user) {
        this.validateExternalAuth(user.idToken);
      }
    })
  }

  private validateExternalAuth(token: string) {
    this.isLoading = true;
    this.loginService.externalLogin(token)
      .subscribe({
        next: (res) => {
          this.isLoading = false;
      },
        error: (err: HttpErrorResponse) => {
          this.toasts.showDanger(err.message, "Google Login Failed");
          this.loginService.signOutExternal();
          this.isLoading = false;
        }
      });
  }

}
