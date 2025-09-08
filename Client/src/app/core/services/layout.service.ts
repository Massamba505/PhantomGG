import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { Roles } from '@/app/shared/constants/roles';

export type LayoutContext = 'public' | 'user' | 'organizer' | 'admin';

@Injectable({
  providedIn: 'root'
})
export class LayoutService {
  private authService = inject(AuthStateService);
  private router = inject(Router);

  private currentContext = signal<LayoutContext>('public');

  readonly context = this.currentContext.asReadonly();
  
  readonly availableContexts = computed(() => {
    const user = this.authService.user();
    if (!user) return ['public'];

    const contexts: LayoutContext[] = ['user']; // All authenticated users can access user context

    if (user.role === Roles.Organizer) {
      contexts.push('organizer');
    }

    if (user.role === Roles.Admin) {
      contexts.push('organizer', 'admin'); // Admins can access all contexts
    }

    return contexts;
  });

  readonly canSwitchTo = computed(() => (context: LayoutContext) => {
    return this.availableContexts().includes(context);
  });

  /**
   * Switch to a different layout context
   */
  switchContext(context: LayoutContext) {
    if (!this.canSwitchTo()(context)) {
      console.warn(`Cannot switch to ${context} context`);
      return;
    }

    this.currentContext.set(context);
    this.navigateToContextDashboard(context);
  }

  /**
   * Get the appropriate layout based on current route and user role
   */
  getLayoutForRoute(route: string): LayoutContext {
    const user = this.authService.user();
    
    // Public routes
    if (route.startsWith('/public') || route === '/' || route.startsWith('/auth')) {
      return 'public';
    }

    if (!user) {
      return 'public';
    }

    // Admin routes
    if (route.startsWith('/admin')) {
      return 'admin';
    }

    // Organizer routes
    if (route.startsWith('/organizer') || route.startsWith('/tournaments') || route.startsWith('/teams')) {
      // Check if user can access organizer features
      if (user.role === Roles.Organizer || user.role === Roles.Admin) {
        return 'organizer';
      }
      // Fallback to user context for regular users
      return 'user';
    }

    // User routes
    if (route.startsWith('/user') || route.startsWith('/profile')) {
      return 'user';
    }

    // Legacy dashboard route - determine based on role
    if (route.startsWith('/dashboard')) {
      switch (user.role) {
        case Roles.Admin:
          return 'admin';
        case Roles.Organizer:
          return 'organizer';
        default:
          return 'user';
      }
    }

    // Default to user context for authenticated users
    return 'user';
  }

  /**
   * Navigate to the appropriate dashboard for a context
   */
  private navigateToContextDashboard(context: LayoutContext) {
    switch (context) {
      case 'public':
        this.router.navigate(['/']);
        break;
      case 'user':
        this.router.navigate(['/user/dashboard']);
        break;
      case 'organizer':
        this.router.navigate(['/organizer/dashboard']);
        break;
      case 'admin':
        this.router.navigate(['/admin/dashboard']);
        break;
    }
  }

  /**
   * Initialize the service based on current route
   */
  initializeFromRoute(route: string) {
    const context = this.getLayoutForRoute(route);
    this.currentContext.set(context);
  }

  /**
   * Get default route for user role
   */
  getDefaultRouteForUser(role: Roles): string {
    switch (role) {
      case Roles.Admin:
        return '/admin/dashboard';
      case Roles.Organizer:
        return '/organizer/dashboard';
      case Roles.User:
        return '/user/dashboard';
      default:
        return '/user/dashboard';
    }
  }
}
