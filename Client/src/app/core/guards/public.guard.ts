import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';

/**
 * Public guard - redirects authenticated users away from public-only routes
 * Useful for login/register pages that shouldn't be accessible when logged in
 */
export const publicGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    // Redirect to appropriate dashboard based on role
    const userRole = authService.user()?.role;
    
    switch (userRole) {
      case 'Admin':
        router.navigate(['/admin']);
        break;
      case 'Organizer':
        router.navigate(['/dashboard']); // Organizer dashboard
        break;
      case 'User':
        router.navigate(['/user/dashboard']); // User dashboard
        break;
      default:
        router.navigate(['/dashboard']);
    }
    
    return false;
  }

  return true;
};

/**
 * Guest guard - allows access to routes for both authenticated and unauthenticated users
 * Useful for public browsing pages that show different content based on auth status
 */
export const guestGuard: CanActivateFn = (route, state) => {
  // Always allow access - component can decide what to show based on auth status
  return true;
};
