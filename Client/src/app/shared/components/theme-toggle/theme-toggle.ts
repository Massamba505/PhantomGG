import { Component, computed, inject } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  templateUrl: "./theme-toggle.html",
})
export class ThemeToggleComponent {
  themeService = inject(ThemeService);
  currentIcon = computed(() => this.themeService.currentTheme());

  buttonLabel = computed(() => {
    const theme = this.themeService.currentTheme();
    switch (theme) {
      case 'light': return 'Switch to dark mode';
      case 'dark': return 'Switch to system mode';
      case 'system': return 'Switch to light mode';
      default: return 'Toggle theme';
    }
  });

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
