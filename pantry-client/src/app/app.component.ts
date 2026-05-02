import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserLoginService } from './shared/services/user-login.service';
import { LayoutService } from './shared/services/layout-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent {
  public isLoggedIn: boolean = false;

  constructor(private userService: UserLoginService, private router: Router, public layoutService: LayoutService
  ) {
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