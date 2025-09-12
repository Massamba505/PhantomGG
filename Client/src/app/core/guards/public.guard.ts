import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { Roles } from '@/app/shared/constants/roles';

export const publicGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthStateService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const userRole = authService.user()?.role;
    
    switch (userRole) {
      case Roles.Admin:
        router.navigate(['/admin']);
        break;
      case Roles.Organizer:
        router.navigate(['/dashboard']);
        break;
      case Roles.User:
        router.navigate(['/user/dashboard']);
        break;
      default:
        router.navigate(['/dashboard']);
    }
    
    return false;
  }

  return true;
};

