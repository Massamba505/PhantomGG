import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { Roles } from '@/app/shared/constants/roles';

/**
 * Role guard for checking specific role permissions
 * More flexible than the auth guard for complex role checking
 */
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  // First check if user is authenticated
  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }

  const requiredRoles = route.data['roles'] as Array<Roles>;
  const userRole = authService.user()?.role;
  
  // If no roles specified, just check authentication
  if (!requiredRoles || requiredRoles.length === 0) {
    return true;
  }

  // Check if user has required role
  if (!userRole || !requiredRoles.includes(userRole)) {
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};

/**
 * Check if user has specific role
 */
export const hasRole = (role: Roles): boolean => {
  const authService = inject(AuthStateService);
  return authService.user()?.role === role;
};

/**
 * Check if user has any of the specified roles
 */
export const hasAnyRole = (roles: Roles[]): boolean => {
  const authService = inject(AuthStateService);
  const userRole = authService.user()?.role;
  return userRole ? roles.includes(userRole) : false;
};

/**
 * Check if user can manage tournaments (Organizer or Admin)
 */
export const canManageTournaments = (): boolean => {
  return hasAnyRole([Roles.Organizer, Roles.Admin]);
};

/**
 * Check if user can manage users (Admin only)
 */
export const canManageUsers = (): boolean => {
  return hasRole(Roles.Admin);
};

/**
 * Check if user can participate in tournaments (any authenticated user)
 */
export const canParticipate = (): boolean => {
  const authService = inject(AuthStateService);
  return authService.isAuthenticated();
};
