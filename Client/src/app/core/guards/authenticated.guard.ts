import { AuthStateService } from '@/app/store/AuthStateService';
import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';

export const authenticatedGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  const isAuthenticated = authService.isAuthenticated();
  if (isAuthenticated) {
    router.navigate(['/dashboard']);
    return false;
  }

  return true;
};
