import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipeComponent } from './recipe.component';
import { FormsModule } from '@angular/forms';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RippleModule } from 'primeng/ripple';
import { SplitButtonModule } from 'primeng/splitbutton';
import { CreateRecipeComponent } from './create-recipe/create-recipe.component';
import { PantryModule } from '../pantry/pantry.module';
import {SelectButtonModule} from 'primeng/selectbutton';
import { RecipeDetailsComponent } from './recipe-details/recipe-details.component';
import { RecipeSearchComponent } from './recipe-search/recipe-search.component'
import {MenuModule} from 'primeng/menu';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import {DropdownModule} from 'primeng/dropdown';

@NgModule({
  declarations: [
    RecipeComponent,
    CreateRecipeComponent,
    RecipeDetailsComponent,
    RecipeSearchComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ToastModule,
    ButtonModule,
    InputTextModule,
    CardModule,
    AutoCompleteModule,
    DialogModule,
    SplitButtonModule,
    InputTextareaModule,
    CheckboxModule,
    RippleModule,
    ProgressSpinnerModule,
    ProgressBarModule,
    PantryModule,
    MenuModule,
    ConfirmDialogModule,
    DropdownModule,
    SelectButtonModule
  ],
  providers: [
    ConfirmationService
  ],
})
export class RecipeModule { }
