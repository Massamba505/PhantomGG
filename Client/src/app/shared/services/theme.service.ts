import { Injectable, signal, effect, computed } from '@angular/core';

export type ThemeMode = 'light' | 'dark' | 'system';

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
    // Apply theme on initialization
    this.applyTheme();
    
    // Listen for system preference changes when in 'system' mode
    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', (e) => {
      if (this.themeMode() === 'system') {
        this.applyTheme();
      }
    });
    
    // Create an effect to handle theme changes
    effect(() => {
      const theme = this.themeMode();
      this.saveTheme(theme);
      this.applyTheme();
    });
  }
  
  private getInitialTheme(): ThemeMode {
    // Try to get saved preference
    const savedTheme = localStorage.getItem(this.THEME_KEY) as ThemeMode;
    if (savedTheme && ['light', 'dark', 'system'].includes(savedTheme)) {
      return savedTheme;
    }
    
    // Default to system
    return 'system';
  }
  
  private saveTheme(theme: ThemeMode): void {
    localStorage.setItem(this.THEME_KEY, theme);
  }
  
  private applyTheme(): void {
    const isDarkMode = this.isDarkMode();
    
    // Apply transition class before changing theme
    this.addTransitionClass();
    
    // Add/remove the dark class on the document
    if (isDarkMode) {
      document.documentElement.classList.add(this.DARK_CLASS);
      document.documentElement.classList.add(this.PRIMENG_DARK_CLASS);
    } else {
      document.documentElement.classList.remove(this.DARK_CLASS);
      document.documentElement.classList.remove(this.PRIMENG_DARK_CLASS);
    }
  }
  
  /**
   * Adds a transition class that's automatically removed after transition completes
   * to prevent transitions on page load
   */
  private addTransitionClass(): void {
    // Add class to enable transitions
    document.documentElement.classList.add('theme-transition');
    
    // Remove the transition class after the transition is complete
    setTimeout(() => {
      document.documentElement.classList.remove('theme-transition');
    }, 400); // Slightly longer than the CSS transition time
  }
  
  public setTheme(theme: ThemeMode): void {
    this.themeMode.set(theme);
  }
  
  public toggleTheme(): void {
    const currentTheme = this.themeMode();
    if (currentTheme === 'light') {
      this.setTheme('dark');
    } else if (currentTheme === 'dark') {
      this.setTheme('system');
    } else {
      this.setTheme('light');
    }
  }
  
  public isDarkMode(): boolean {
    const theme = this.themeMode();
    return theme === 'dark' || 
      (theme === 'system' && window.matchMedia('(prefers-color-scheme: dark)').matches);
  }
}
