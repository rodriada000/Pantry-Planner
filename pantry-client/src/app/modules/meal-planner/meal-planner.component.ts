import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';

@Component({
  selector: 'app-meal-planner',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, InputTextareaModule],
  templateUrl: './meal-planner.component.html',
  styleUrl: './meal-planner.component.css'
})
export class MealPlannerComponent {

}
