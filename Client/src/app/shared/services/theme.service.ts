import { Injectable, signal, effect, computed } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'phantomgg-theme';
  private readonly DARK_CLASS = 'dark';
  private readonly PRIMENG_DARK_CLASS = 'phantomGG-dark';
  
  private themeMode = signal<ThemeMode>(this.getInitialTheme());
  public currentTheme = computed(() => this.themeMode());
  
  constructor() {
    this.applyTheme();
    
    effect(() => {
      const theme = this.themeMode();
      this.saveTheme(theme);
      this.applyTheme();
    });
  }
  
  private getInitialTheme(): ThemeMode {
    // Try to get saved preference
    const savedTheme = localStorage.getItem(this.THEME_KEY) as ThemeMode;
    if (savedTheme && ['light', 'dark'].includes(savedTheme)) {
      return savedTheme;
    }
    
    return 'dark';
  }
  
  private saveTheme(theme: ThemeMode): void {
    localStorage.setItem(this.THEME_KEY, theme);
  }
  
  private applyTheme(): void {
    const isDarkMode = this.isDarkMode();
    if (isDarkMode) {
      document.documentElement.classList.add(this.DARK_CLASS);
      document.documentElement.classList.add(this.PRIMENG_DARK_CLASS);
    } else {
      document.documentElement.classList.remove(this.DARK_CLASS);
      document.documentElement.classList.remove(this.PRIMENG_DARK_CLASS);
    }
  }
  
  public setTheme(theme: ThemeMode): void {
    this.themeMode.set(theme);
  }
  
  public toggleTheme(): void {
    const currentTheme = this.themeMode();
    if (currentTheme === 'light') {
      this.setTheme('dark');
    } else {
      this.setTheme('light');
    }
  }
  
  public isDarkMode(): boolean {
    return this.themeMode() === 'dark';
  }
}
