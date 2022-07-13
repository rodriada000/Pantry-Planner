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

  constructor(private loginService: UserLoginService, private toasts: ToastService, private router: Router) { }

  ngOnInit(): void {
  }

  public login(): void {
    this.loginService.login(this.loginModel).subscribe(resp => {

    }, 
    error => {
      this.toasts.showDanger("Check that your username and password is correct.", "Login Failed");
    });
  }

  public loginWithGoogle() {
    this.loginService.loginWithGoogle();

    this.loginService.extAuthChanged.subscribe(user => {
      console.log('extAuthChanged')
      this.validateExternalAuth(user.idToken);
    })
  }

  private validateExternalAuth(token: string) {
    this.loginService.externalLogin(token)
      .subscribe({
        next: (res) => {
      },
        error: (err: HttpErrorResponse) => {
          this.toasts.showDanger(err.message, "Google Login Failed");
          this.loginService.signOutExternal();
        }
      });
  }

}
