import { Component, signal, computed, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, RouterModule } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

import { Roles } from '@/app/shared/constants/roles';
import { Sidebar } from '../../sidebar/sidebar';
import { ThemeToggle } from '../../ui/theme-toggle/theme-toggle';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, Sidebar, ThemeToggle],
  templateUrl: './dashboard-layout.html',
  styleUrls: ['./dashboard-layout.css']
})
export class DashboardLayout implements OnInit, OnDestroy {
  private router = inject(Router);
  private authState = inject(AuthStateService);
  
  sidebarOpen = signal(window.innerWidth > 768);
  pageTitle = signal('Dashboard');
  private routerSubscription: Subscription | null = null;

  // User info
  userRole: Roles = Roles.Organizer;
  userName = signal('John Doe');
  userInitials = computed(() => {
    const name = this.userName();
    if (!name) return 'U';
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  });

  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
    
    // Set page title based on route
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        const path = this.router.url.split('/').pop() || 'dashboard';
        this.pageTitle.set(this.formatPageTitle(path));
      });
      
    // Get current page title
    const currentPath = this.router.url.split('/').pop() || 'dashboard';
    this.pageTitle.set(this.formatPageTitle(currentPath));
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
    if (this.routerSubscription) {
      this.routerSubscription.unsubscribe();
    }
  }

  formatPageTitle(path: string): string {
    // Remove query params or hash
    path = path.split('?')[0].split('#')[0];
    
    // Replace hyphens with spaces and capitalize
    return path
      .replace(/-/g, ' ')
      .split(' ')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');
  }

  handleResize = () => {
    this.sidebarOpen.set(window.innerWidth > 768);
  };

  toggleSidebar() {
    this.sidebarOpen.update((prev) => !prev);
  }
}
