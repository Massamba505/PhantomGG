import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastComponent } from '@/app/shared/components/ui/toast/toast';
import { ProfileDropdown } from '@/app/shared/components/profile-dropdown/profile-dropdown';
import { ThemeToggle } from '@/app/shared/components/ui/theme-toggle/theme-toggle';
import { 
  LucideAngularModule, 
  Menu, 
  Home, 
  Trophy, 
  Users, 
  Calendar, 
  User,
  Settings,
  Bell,
  Plus
} from 'lucide-angular';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  badge?: string;
}

@Component({
  selector: 'app-user-layout',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    ToastComponent, 
    ProfileDropdown, 
    ThemeToggle,
    LucideAngularModule
  ],
  template: `
    <div class="flex h-screen overflow-hidden bg-background">
      <!-- Sidebar -->
      <aside class="flex-shrink-0 transition-all duration-300"
             [ngClass]="sidebarOpen() ? 'w-64' : 'w-16'">
        <div class="flex flex-col h-full bg-card border-r border-border">
          <!-- Sidebar Header -->
          <div class="flex items-center justify-between p-4 border-b border-border">
            @if (sidebarOpen()) {
              <div class="flex items-center space-x-2">
                <img src="/assets/PhantomGG_LOGO.png" alt="PhantomGG" class="h-6 w-auto">
                <span class="font-bold text-primary">PhantomGG</span>
              </div>
            }
            <button 
              (click)="toggleSidebar()"
              class="p-2 rounded-md hover:bg-muted text-muted-foreground hover:text-foreground">
              <lucide-angular [img]="Menu" size="20"></lucide-angular>
            </button>
          </div>

          <!-- Navigation Menu -->
          <nav class="flex-1 p-4 space-y-2">
            @for (item of menuItems; track item.url) {
              <a 
                [routerLink]="item.url"
                routerLinkActive="bg-primary text-primary-foreground"
                class="flex items-center space-x-3 px-3 py-2 rounded-md text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted transition-colors">
                <lucide-angular [img]="item.icon" size="20"></lucide-angular>
                @if (sidebarOpen()) {
                  <span>{{ item.title }}</span>
                  @if (item.badge) {
                    <span class="ml-auto bg-primary text-primary-foreground text-xs px-2 py-1 rounded-full">
                      {{ item.badge }}
                    </span>
                  }
                }
              </a>
            }
          </nav>

          <!-- Quick Actions -->
          @if (sidebarOpen()) {
            <div class="p-4 border-t border-border">
              <h3 class="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2">
                Quick Actions
              </h3>
              <div class="space-y-2">
                @for (action of quickActions; track action.url) {
                  <a 
                    [routerLink]="action.url"
                    class="flex items-center space-x-3 px-3 py-2 rounded-md text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted transition-colors">
                    <lucide-angular [img]="action.icon" size="16"></lucide-angular>
                    <span>{{ action.title }}</span>
                  </a>
                }
              </div>
            </div>
          }
        </div>
      </aside>

      <!-- Main Content Area -->
      <div class="flex flex-col flex-1 h-full overflow-hidden">
        <!-- Top Header -->
        <header class="h-16 flex items-center justify-between px-6 bg-card border-b border-border">
          <h1 class="text-xl font-semibold text-foreground">{{ pageTitle() }}</h1>

          <div class="flex items-center space-x-4">
            <!-- Notifications -->
            <button class="p-2 rounded-md hover:bg-muted text-muted-foreground hover:text-foreground relative">
              <lucide-angular [img]="Bell" size="20"></lucide-angular>
              <!-- Notification badge -->
              <span class="absolute -top-1 -right-1 bg-destructive text-destructive-foreground text-xs w-5 h-5 rounded-full flex items-center justify-center">
                3
              </span>
            </button>

            <!-- Theme Toggle -->
            <app-theme-toggle></app-theme-toggle>

            <!-- Profile Dropdown -->
            <app-profile-dropdown></app-profile-dropdown>
          </div>
        </header>

        <!-- Page Content -->
        <main class="flex-1 p-6 overflow-y-auto bg-background">
          <ng-content></ng-content>
        </main>
      </div>

      <!-- Toast Component -->
      <app-toast></app-toast>
    </div>
  `,
  styles: []
})
export class UserLayout implements OnInit, OnDestroy {
  authService = inject(AuthStateService);
  
  private internalSidebarOpen = signal(true);
  
  sidebarOpen = computed(() => {
    if (typeof window !== 'undefined') {
      return window.innerWidth > 768 ? this.internalSidebarOpen() : false;
    }
    return this.internalSidebarOpen();
  });

  pageTitle = computed(() => {
    // This could be dynamic based on current route
    return 'My Dashboard';
  });

  // Icons
  Menu = Menu;
  Home = Home;
  Trophy = Trophy;
  Users = Users;
  Calendar = Calendar;
  User = User;
  Settings = Settings;
  Bell = Bell;
  Plus = Plus;

  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/user/dashboard',
      icon: Home,
    },
    {
      title: 'My Tournaments',
      url: '/user/tournaments',
      icon: Trophy,
      badge: '2',
    },
    {
      title: 'My Teams',
      url: '/user/teams',
      icon: Users,
    },
    {
      title: 'Schedule',
      url: '/user/schedule',
      icon: Calendar,
    },
    {
      title: 'Browse Tournaments',
      url: '/tournaments', // Public tournament browsing
      icon: Trophy,
    },
    {
      title: 'Profile',
      url: '/profile',
      icon: User,
    },
  ];

  quickActions: MenuItem[] = [
    {
      title: 'Join Tournament',
      url: '/user/join-tournament',
      icon: Plus,
    },
  ];

  ngOnInit() {
    if (typeof window !== 'undefined') {
      window.addEventListener('resize', this.handleResize);
      this.handleResize(); // Set initial state
    }
  }

  ngOnDestroy() {
    if (typeof window !== 'undefined') {
      window.removeEventListener('resize', this.handleResize);
    }
  }

  toggleSidebar() {
    this.internalSidebarOpen.update(open => !open);
  }

  private handleResize = () => {
    if (typeof window !== 'undefined') {
      // Auto-collapse on mobile
      if (window.innerWidth <= 768) {
        this.internalSidebarOpen.set(false);
      }
    }
  };
}
