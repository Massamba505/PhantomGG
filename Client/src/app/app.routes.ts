import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard } from './core/guards/auth.guard';
import { NotFoundComponent } from './features/not-found/not-found';
import { userRoutes } from './features/user/user.routes';
import { publicRoutes } from './features/public/public.routes';
import { organizerRoutes } from './features/organizer/organizer.routes';
import { publicGuard } from './core/guards/public.guard';
import { UserRoles } from './api/models';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./shared/components/layouts/public-layout/public-layout').then(
        (m) => m.PublicLayout
      ),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/public/landing/landing').then((m) => m.Landing),
        title: 'PhantomGG - Esports Tournament Platform',
      },
    ],
  },
  {
    path: 'auth',
    canActivate: [publicGuard],
    loadComponent: () => import('./features/auth/auth').then((m) => m.Auth),
    children: authRoutes,
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import(
        './features/dashboard-selection/dashboard-selection.component'
      ).then((m) => m.DashboardSelectionComponent),
  },
  {
    path: 'public',
    children: publicRoutes,
  },
  {
    path: 'user',
    canActivate: [authGuard],
    data: { roles: [UserRoles.User] },
    children: userRoutes,
  },
  {
    path: 'organizer',
    canActivate: [authGuard],
    data: { roles: [UserRoles.Organizer] },
    children: organizerRoutes,
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/unauthorized/unauthorized').then(
        (m) => m.Unauthorized
      ),
    title: 'Unauthorized - PhantomGG',
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
