import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { ThemeService } from '@/app/shared/services/theme.service';
import { LucideIcons } from '../icons/lucide-icons';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './theme-toggle.html',
})
export class ThemeToggle {
  themeService = inject(ThemeService);
  currentIcon = computed(() => this.themeService.currentTheme());


  readonly icons = LucideIcons;

  buttonLabel = computed(() => {
    const theme = this.themeService.currentTheme();
    switch (theme) {
      case 'light':
        return 'Switch to dark mode';
      case 'dark':
        return 'Switch to light mode';
      default:
        return 'Toggle theme';
    }
  });

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
