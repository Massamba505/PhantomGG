import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
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
  Settings,
  Bell,
  Shield,
  BarChart,
  Database,
  Activity,
  Eye,
  User
} from 'lucide-angular';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  badge?: string;
}

@Component({
  selector: 'app-admin-layout',
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
                <span class="bg-destructive text-destructive-foreground text-xs px-2 py-1 rounded-full">ADMIN</span>
              </div>
            }
            <button 
              (click)="toggleSidebar()"
              class="p-2 rounded-md hover:bg-muted text-muted-foreground hover:text-foreground">
              <lucide-angular [img]="Menu" size="20"></lucide-angular>
            </button>
          </div>

          <!-- Context Switcher -->
          @if (sidebarOpen()) {
            <div class="p-4 border-b border-border">
              <div class="flex items-center justify-between">
                <span class="text-sm font-medium text-destructive">Admin Mode</span>
                <div class="flex space-x-1">
                  <button 
                    (click)="switchToUserView()"
                    class="flex items-center space-x-1 text-xs bg-muted text-muted-foreground hover:bg-muted/80 px-2 py-1 rounded-md transition-colors">
                    <lucide-angular [img]="User" size="12"></lucide-angular>
                    <span>User</span>
                  </button>
                  <button 
                    (click)="switchToOrganizerView()"
                    class="flex items-center space-x-1 text-xs bg-muted text-muted-foreground hover:bg-muted/80 px-2 py-1 rounded-md transition-colors">
                    <lucide-angular [img]="Eye" size="12"></lucide-angular>
                    <span>Organizer</span>
                  </button>
                </div>
              </div>
            </div>
          }

          <!-- Navigation Menu -->
          <nav class="flex-1 p-4 space-y-2">
            <!-- Main Navigation -->
            <div class="space-y-1">
              @for (item of menuItems; track item.url) {
                <a 
                  [routerLink]="item.url"
                  routerLinkActive="bg-primary text-primary-foreground"
                  class="flex items-center space-x-3 px-3 py-2 rounded-md text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted transition-colors">
                  <lucide-angular [img]="item.icon" size="20"></lucide-angular>
                  @if (sidebarOpen()) {
                    <span>{{ item.title }}</span>
                    @if (item.badge) {
                      <span class="ml-auto bg-destructive text-destructive-foreground text-xs px-2 py-1 rounded-full">
                        {{ item.badge }}
                      </span>
                    }
                  }
                </a>
              }
            </div>

            <!-- System Management -->
            @if (sidebarOpen()) {
              <div class="pt-4">
                <h3 class="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2">
                  System Management
                </h3>
                <div class="space-y-1">
                  @for (sysItem of systemItems; track sysItem.url) {
                    <a 
                      [routerLink]="sysItem.url"
                      routerLinkActive="bg-primary text-primary-foreground"
                      class="flex items-center space-x-3 px-3 py-2 rounded-md text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted transition-colors">
                      <lucide-angular [img]="sysItem.icon" size="16"></lucide-angular>
                      <span>{{ sysItem.title }}</span>
                      @if (sysItem.badge) {
                        <span class="ml-auto bg-orange-500 text-white text-xs px-2 py-1 rounded-full">
                          {{ sysItem.badge }}
                        </span>
                      }
                    </a>
                  }
                </div>
              </div>
            }

            <!-- Monitoring -->
            @if (sidebarOpen()) {
              <div class="pt-4">
                <h3 class="text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-2">
                  Monitoring
                </h3>
                <div class="space-y-1">
                  @for (monitorItem of monitoringItems; track monitorItem.url) {
                    <a 
                      [routerLink]="monitorItem.url"
                      routerLinkActive="bg-primary text-primary-foreground"
                      class="flex items-center space-x-3 px-3 py-2 rounded-md text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted transition-colors">
                      <lucide-angular [img]="monitorItem.icon" size="16"></lucide-angular>
                      <span>{{ monitorItem.title }}</span>
                    </a>
                  }
                </div>
              </div>
            }
          </nav>
        </div>
      </aside>

      <!-- Main Content Area -->
      <div class="flex flex-col flex-1 h-full overflow-hidden">
        <!-- Top Header -->
        <header class="h-16 flex items-center justify-between px-6 bg-card border-b border-border">
          <div class="flex items-center space-x-3">
            <h1 class="text-xl font-semibold text-foreground">{{ pageTitle() }}</h1>
            <span class="bg-destructive text-destructive-foreground text-xs px-2 py-1 rounded-full">ADMIN</span>
          </div>

          <div class="flex items-center space-x-4">
            <!-- System Status Indicator -->
            <div class="flex items-center space-x-2">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-sm text-muted-foreground">System Online</span>
            </div>

            <!-- Notifications -->
            <button class="p-2 rounded-md hover:bg-muted text-muted-foreground hover:text-foreground relative">
              <lucide-angular [img]="Bell" size="20"></lucide-angular>
              <!-- Critical alerts badge -->
              <span class="absolute -top-1 -right-1 bg-destructive text-destructive-foreground text-xs w-5 h-5 rounded-full flex items-center justify-center">
                2
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
export class AdminLayout implements OnInit, OnDestroy {
  authService = inject(AuthStateService);
  router = inject(Router);
  
  private internalSidebarOpen = signal(true);
  
  sidebarOpen = computed(() => {
    if (typeof window !== 'undefined') {
      return window.innerWidth > 768 ? this.internalSidebarOpen() : false;
    }
    return this.internalSidebarOpen();
  });

  pageTitle = computed(() => {
    // This could be dynamic based on current route
    return 'Admin Dashboard';
  });

  // Icons
  Menu = Menu;
  Home = Home;
  Trophy = Trophy;
  Users = Users;
  Settings = Settings;
  Bell = Bell;
  Shield = Shield;
  BarChart = BarChart;
  Database = Database;
  Activity = Activity;
  Eye = Eye;
  User = User;

  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/admin/dashboard',
      icon: Home,
    },
    {
      title: 'Users',
      url: '/admin/users',
      icon: Users,
      badge: '24',
    },
    {
      title: 'Tournaments',
      url: '/admin/tournaments',
      icon: Trophy,
    },
    {
      title: 'Analytics',
      url: '/admin/analytics',
      icon: BarChart,
    },
  ];

  systemItems: MenuItem[] = [
    {
      title: 'System Settings',
      url: '/admin/settings',
      icon: Settings,
    },
    {
      title: 'Database',
      url: '/admin/database',
      icon: Database,
    },
    {
      title: 'Security',
      url: '/admin/security',
      icon: Shield,
      badge: '1',
    },
  ];

  monitoringItems: MenuItem[] = [
    {
      title: 'System Health',
      url: '/admin/health',
      icon: Activity,
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

  switchToUserView() {
    // Navigate to user dashboard
    this.router.navigate(['/user/dashboard']);
  }

  switchToOrganizerView() {
    // Navigate to organizer dashboard  
    this.router.navigate(['/organizer/dashboard']);
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
