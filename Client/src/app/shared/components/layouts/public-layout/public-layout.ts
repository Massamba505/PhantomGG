import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastComponent } from '@/app/shared/components/ui/toast/toast';
import { LucideAngularModule, Menu, X, LogIn, UserPlus, Home, Trophy, Users, Calendar } from 'lucide-angular';

@Component({
  selector: 'app-public-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, ToastComponent, LucideAngularModule],
  template: `
    <div class="min-h-screen bg-background">
      <!-- Navigation Header -->
      <header class="border-b border-border bg-card">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between items-center h-16">
            <!-- Logo -->
            <div class="flex items-center">
              <a routerLink="/" class="flex items-center space-x-2">
                <img src="/assets/PhantomGG_LOGO.png" alt="PhantomGG" class="h-8 w-auto">
                <span class="text-xl font-bold text-primary">PhantomGG</span>
              </a>
            </div>

            <!-- Desktop Navigation -->
            <nav class="hidden md:flex items-center space-x-8">
              <a routerLink="/public/tournaments" 
                 routerLinkActive="text-primary font-medium"
                 class="text-muted-foreground hover:text-foreground transition-colors flex items-center space-x-1">
                <lucide-angular [img]="Trophy" size="16"></lucide-angular>
                <span>Tournaments</span>
              </a>
              <a routerLink="/public/teams"
                 routerLinkActive="text-primary font-medium" 
                 class="text-muted-foreground hover:text-foreground transition-colors flex items-center space-x-1">
                <lucide-angular [img]="Users" size="16"></lucide-angular>
                <span>Teams</span>
              </a>
              <a routerLink="/public/results"
                 routerLinkActive="text-primary font-medium"
                 class="text-muted-foreground hover:text-foreground transition-colors flex items-center space-x-1">
                <lucide-angular [img]="Calendar" size="16"></lucide-angular>
                <span>Results</span>
              </a>
            </nav>

            <!-- Auth Buttons -->
            <div class="hidden md:flex items-center space-x-3">
              @if (authService.isAuthenticated()) {
                <button 
                  (click)="goToDashboard()"
                  class="btn btn-outline">
                  Dashboard
                </button>
                <button 
                  (click)="logout()"
                  class="btn btn-primary">
                  Logout
                </button>
              } @else {
                <a routerLink="/auth/login" class="btn btn-outline flex items-center space-x-1">
                  <lucide-angular [img]="LogIn" size="16"></lucide-angular>
                  <span>Sign In</span>
                </a>
                <a routerLink="/auth/register" class="btn btn-primary flex items-center space-x-1">
                  <lucide-angular [img]="UserPlus" size="16"></lucide-angular>
                  <span>Sign Up</span>
                </a>
              }
            </div>

            <!-- Mobile menu button -->
            <button 
              (click)="toggleMobileMenu()"
              class="md:hidden p-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
              <lucide-angular [img]="mobileMenuOpen ? X : Menu" size="20"></lucide-angular>
            </button>
          </div>

          <!-- Mobile Navigation -->
          @if (mobileMenuOpen) {
            <div class="md:hidden pb-4 pt-2 border-t border-border">
              <div class="flex flex-col space-y-2">
                <a routerLink="/public/tournaments" 
                   (click)="closeMobileMenu()"
                   class="flex items-center space-x-2 px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                  <lucide-angular [img]="Trophy" size="16"></lucide-angular>
                  <span>Tournaments</span>
                </a>
                <a routerLink="/public/teams"
                   (click)="closeMobileMenu()"
                   class="flex items-center space-x-2 px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                  <lucide-angular [img]="Users" size="16"></lucide-angular>
                  <span>Teams</span>
                </a>
                <a routerLink="/public/results"
                   (click)="closeMobileMenu()"
                   class="flex items-center space-x-2 px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                  <lucide-angular [img]="Calendar" size="16"></lucide-angular>
                  <span>Results</span>
                </a>
                
                <div class="border-t border-border pt-2 mt-2">
                  @if (authService.isAuthenticated()) {
                    <button 
                      (click)="goToDashboard(); closeMobileMenu()"
                      class="w-full text-left px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                      Dashboard
                    </button>
                    <button 
                      (click)="logout(); closeMobileMenu()"
                      class="w-full text-left px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                      Logout
                    </button>
                  } @else {
                    <a routerLink="/auth/login" 
                       (click)="closeMobileMenu()"
                       class="flex items-center space-x-2 px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                      <lucide-angular [img]="LogIn" size="16"></lucide-angular>
                      <span>Sign In</span>
                    </a>
                    <a routerLink="/auth/register"
                       (click)="closeMobileMenu()"
                       class="flex items-center space-x-2 px-3 py-2 rounded-md text-muted-foreground hover:text-foreground hover:bg-muted">
                      <lucide-angular [img]="UserPlus" size="16"></lucide-angular>
                      <span>Sign Up</span>
                    </a>
                  }
                </div>
              </div>
            </div>
          }
        </div>
      </header>

      <!-- Main Content -->
      <main class="flex-1">
        <ng-content></ng-content>
      </main>

      <!-- Footer -->
      <footer class="bg-card border-t border-border mt-auto">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div class="col-span-1 md:col-span-2">
              <div class="flex items-center space-x-2 mb-4">
                <img src="/assets/PhantomGG_LOGO.png" alt="PhantomGG" class="h-6 w-auto">
                <span class="text-lg font-bold text-primary">PhantomGG</span>
              </div>
              <p class="text-muted-foreground text-sm">
                The ultimate platform for organizing and participating in gaming tournaments.
                Join the competition today!
              </p>
            </div>
            
            <div>
              <h3 class="font-semibold mb-3">Quick Links</h3>
              <ul class="space-y-2 text-sm">
                <li><a routerLink="/public/tournaments" class="text-muted-foreground hover:text-foreground">Browse Tournaments</a></li>
                <li><a routerLink="/public/teams" class="text-muted-foreground hover:text-foreground">Browse Teams</a></li>
                <li><a routerLink="/public/results" class="text-muted-foreground hover:text-foreground">Match Results</a></li>
              </ul>
            </div>
            
            <div>
              <h3 class="font-semibold mb-3">Get Started</h3>
              <ul class="space-y-2 text-sm">
                <li><a routerLink="/auth/register" class="text-muted-foreground hover:text-foreground">Sign Up</a></li>
                <li><a routerLink="/auth/login" class="text-muted-foreground hover:text-foreground">Sign In</a></li>
              </ul>
            </div>
          </div>
          
          <div class="border-t border-border mt-8 pt-8 text-center text-sm text-muted-foreground">
            <p>&copy; {{ currentYear }} PhantomGG. All rights reserved.</p>
          </div>
        </div>
      </footer>

      <!-- Toast Component -->
      <app-toast></app-toast>
    </div>
  `,
  styles: []
})
export class PublicLayout {
  authService = inject(AuthStateService);
  router = inject(Router);
  
  mobileMenuOpen = false;
  currentYear = new Date().getFullYear();
  
  // Icons
  Trophy = Trophy;
  Users = Users;
  Calendar = Calendar;
  Menu = Menu;
  X = X;
  LogIn = LogIn;
  UserPlus = UserPlus;
  Home = Home;

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  closeMobileMenu() {
    this.mobileMenuOpen = false;
  }

  goToDashboard() {
    const userRole = this.authService.user()?.role;
    
    switch (userRole) {
      case 'Admin':
        this.router.navigate(['/admin']);
        break;
      case 'Organizer':
        this.router.navigate(['/dashboard']); // Organizer dashboard
        break;
      case 'User':
        this.router.navigate(['/user/dashboard']); // User dashboard
        break;
      default:
        this.router.navigate(['/dashboard']);
    }
  }

  async logout() {
    try {
      await this.authService.logout().toPromise();
      this.router.navigate(['/']);
    } catch (error) {
      console.error('Logout failed:', error);
    }
  }
}
