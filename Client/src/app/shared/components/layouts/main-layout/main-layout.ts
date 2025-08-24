import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '@/app/shared/services/theme.service';
import { ToastComponent } from '@/app/shared/components/ui/toast/toast';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.html',
  imports: [CommonModule, RouterModule, ToastComponent],
  standalone: true,
})
export class MainLayout implements OnInit {
  private themeService = inject(ThemeService);
  
  ngOnInit() {
    // Apply the theme transition class to make theme changes smooth
    document.documentElement.classList.add('theme-transition');
  }
}
