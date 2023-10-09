import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {

  constructor() { }

  public get height() {
    return window.innerHeight;
  }
  
  public get width() {
    return window.innerWidth;
  }

  public get isMobileScreen() {
    return this.width <= 720;
  }

  public get isMedScreen() {
    return this.width > 720 && this.width <= 1200;
  }

  public get isLargeScreen() {
    return this.width > 1200;
  }

  public get sideMenuSize(): string {
    if (this.isMobileScreen) {
      return '100vw';
    } else if (this.isMedScreen) {
      return '45vw';
    } else {
      return '25vw';
    }
  }
}
