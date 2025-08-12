import { Component } from '@angular/core';
import { ThemeToggleComponent } from '../../ui/theme-toggle/theme-toggle';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.html',
  imports: [ThemeToggleComponent],
  standalone: true,
})
export class MainLayout {}
