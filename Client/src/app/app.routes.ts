import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard, adminGuard } from './core/guards/auth.guard';
import { UnauthorizedComponent } from './shared/components/unauthorized/unauthorized.component';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/landing').then((m) => m.Landing),
    title: 'PhantomGG',
  },
  {
    path: 'auth',
    loadComponent: () => import('./features/auth/auth').then((m) => m.Auth),
    children: authRoutes,
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/profile/profile.component').then(
        (m) => m.ProfileComponent
      ),
    title: 'My Profile - PhantomGG',
  },
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () =>
      import('./features/admin/admin.component').then((m) => m.AdminComponent),
    title: 'Admin Dashboard - PhantomGG',
  },
  {
    path: 'unauthorized',
    component: UnauthorizedComponent,
    title: 'Unauthorized - PhantomGG',
  },
  {
    path: '**',
    redirectTo: '',
  },
];
