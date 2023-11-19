import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserLoginService } from './shared/services/user-login.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  // public forecasts?: WeatherForecast[];
  public isLoggedIn: boolean = false;

  constructor(private userService: UserLoginService, private router: Router) {
    this.userService.token$.subscribe(t => {
      this.isLoggedIn = !!t;
    })
  }

  logout() {
    this.userService.logout();
    this.router.navigate(['/login']);
  }

  title = 'pantry-client';
}