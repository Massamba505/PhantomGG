import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { tap } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  // Check if the user is authenticated
  const isAuthenticated = authService.isAuthenticated();

  if (!isAuthenticated) {
    console.log('User is not authenticated, redirecting to login');
    router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }

  // If route has data.roles, check if user has required role
  const requiredRoles = route.data['roles'] as Array<string>;
  if (requiredRoles && requiredRoles.length > 0) {
    const userRole = authService.user()?.role;
    if (!userRole || !requiredRoles.includes(userRole)) {
      console.log(
        'User does not have required role, redirecting to unauthorized'
      );
      router.navigate(['/unauthorized']);
      return false;
    }
  }

  return true;
};

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  // First, check if user is authenticated
  if (!authService.isAuthenticated()) {
    console.log('User is not authenticated, redirecting to login');
    router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }

  // Then check if user has admin role
  const userRole = authService.user()?.role;
  if (userRole !== 'Admin') {
    console.log('User is not an admin, redirecting to unauthorized');
    router.navigate(['/unauthorized']);
    return false;
  }

  return true;
};
