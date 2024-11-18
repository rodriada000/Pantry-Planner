import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GroceryListModule } from './modules/grocery-list/grocery-list.module';
import { PantryModule } from './modules/pantry/pantry.module';
import { RecipeModule } from './modules/recipe/recipe.module';
import { KitchenNavComponent } from './shared/components/kitchen-nav/kitchen-nav.component';
import { MenubarModule } from 'primeng/menubar';
import { MessageService } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { ButtonModule } from 'primeng/button';
import { LoginComponent } from './login/login.component';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import {OverlayPanelModule} from 'primeng/overlaypanel';
import {ToastModule} from 'primeng/toast';
import {AutoCompleteModule} from 'primeng/autocomplete';
import { SocialLoginModule, SocialAuthServiceConfig } from '@abacritt/angularx-social-login';
import { GoogleLoginProvider } from '@abacritt/angularx-social-login';
import {ProgressBarModule} from 'primeng/progressbar';

@NgModule({ declarations: [
        AppComponent,
        KitchenNavComponent,
        LoginComponent,
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        FormsModule,
        AppRoutingModule,
        PantryModule,
        RecipeModule,
        GroceryListModule,
        MenubarModule,
        MenuModule,
        ButtonModule,
        InputTextModule,
        CardModule,
        BrowserAnimationsModule,
        OverlayPanelModule,
        ToastModule,
        AutoCompleteModule,
        SocialLoginModule,
        ProgressBarModule], providers: [
        MessageService,
        {
            provide: 'SocialAuthServiceConfig',
            useValue: {
                autoLogin: false,
                providers: [
                    {
                        id: GoogleLoginProvider.PROVIDER_ID,
                        provider: new GoogleLoginProvider('877234124976-ishurf4f1fsmbgqh0fi80kfpjo71eev4.apps.googleusercontent.com')
                    },
                ],
                onError: (err) => {
                    console.error(err);
                },
            } as SocialAuthServiceConfig
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule { }
