import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { GroceryListComponent } from './modules/grocery-list/grocery-list.component';
import { PantryComponent } from './modules/pantry/pantry.component';
import { RecipeComponent } from './modules/recipe/recipe.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'pantry', component: PantryComponent },
  { path: 'grocery', component: GroceryListComponent },
  { path: 'recipe', component: RecipeComponent },
  { path: 'recipe/:id', component: RecipeComponent },


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
