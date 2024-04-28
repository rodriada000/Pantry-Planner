import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListDetailComponent } from './list-detail/list-detail.component';
import { ManageListComponent } from './manage-list/manage-list.component';
import { GroceryListComponent } from './grocery-list.component';
import { PantryModule } from '../pantry/pantry.module';
import { FormsModule } from '@angular/forms';
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
import {ConfirmDialogModule} from 'primeng/confirmdialog';
import {ConfirmationService} from 'primeng/api';
import {MenuModule} from 'primeng/menu';

@NgModule({
  declarations: [
    ListDetailComponent,
    ManageListComponent,
    GroceryListComponent
  ],
  imports: [
    PantryModule,
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
    ConfirmDialogModule,
    MenuModule,
  ],
  providers: [
    ConfirmationService
  ],
  exports: [
    GroceryListComponent,
    ManageListComponent,
    ListDetailComponent,
  ]
})
export class GroceryListModule {
}
