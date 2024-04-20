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
import {SelectButtonModule} from 'primeng/selectbutton'


@NgModule({
  declarations: [
    RecipeComponent,
    CreateRecipeComponent
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
    SelectButtonModule
  ]
})
export class RecipeModule { }
