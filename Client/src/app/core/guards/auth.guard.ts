import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { Roles } from '@/app/shared/constants/roles';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/auth/login'], {
      queryParams: { returnUrl: state.url },
    });
    return false;
  }

  const requiredRoles = route.data['roles'] as Array<Roles>;
  if (requiredRoles && requiredRoles.length > 0) {
    const userRole = authService.user()?.role;
    if (!userRole || !requiredRoles.includes(userRole)) {
      router.navigate(['/unauthorized']);
      return false;
    }
  }

  return true;
};
