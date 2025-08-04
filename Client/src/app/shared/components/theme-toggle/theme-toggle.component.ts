import { Component, computed } from '@angular/core';
import { ThemeService } from '../../services/theme.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      class="theme-toggle"
      (click)="toggleTheme()"
      [attr.aria-label]="buttonLabel()"
      [title]="buttonLabel()"
    >
      <span *ngIf="currentIcon() === 'light'">🌞</span>
      <span *ngIf="currentIcon() === 'dark'">🌜</span>
      <span *ngIf="currentIcon() === 'system'">🖥️</span>
    </button>
  `,
  styles: [`
    .theme-toggle {
      background: none;
      border-radius: 6px;
      padding: 8px 12px;
      font-size: 18px;
      cursor: pointer;
      transition: background 0.2s;
    }
  `]
})
export class ThemeToggleComponent {
  constructor(private themeService: ThemeService) {}

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
