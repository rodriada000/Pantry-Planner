import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchIngredientsComponent } from './search-ingredients/search-ingredients.component';
import { AddIngredientModalComponent } from './add-ingredient-modal/add-ingredient-modal.component';
import { MyIngredientsComponent } from './my-ingredients/my-ingredients.component';
import { PantryComponent } from './pantry.component';
import { ManageUsersComponent } from './manage-users/manage-users.component';
import { CreateIngredientModalComponent } from './create-ingredient-modal/create-ingredient-modal.component';
import {ToastModule} from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CardModule } from 'primeng/card';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DialogModule} from 'primeng/dialog';
import {SplitButtonModule} from 'primeng/splitbutton'
import {InputTextareaModule} from 'primeng/inputtextarea';
import {CheckboxModule} from 'primeng/checkbox';
import {RippleModule} from 'primeng/ripple';
import {ProgressSpinnerModule} from 'primeng/progressspinner';
import {ProgressBarModule} from 'primeng/progressbar';

@NgModule({
  declarations: [
    PantryComponent,
    SearchIngredientsComponent,
    AddIngredientModalComponent,
    MyIngredientsComponent,
    ManageUsersComponent,
    CreateIngredientModalComponent
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
    ProgressBarModule
  ],
  providers: [
  ],
  exports: [
    PantryComponent,
    SearchIngredientsComponent,
    MyIngredientsComponent,
    ManageUsersComponent
  ]
})
export class PantryModule {

}
