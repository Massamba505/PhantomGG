import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard } from './core/guards/auth.guard';
import { NotFoundComponent } from './features/not-found/not-found';
import { Roles } from './shared/constants/roles';
import { userRoutes } from './features/user/user.routes';
import { publicRoutes } from './features/public/public.routes';
import { organizerRoutes } from './features/organizer/organizer.routes';
import { publicGuard } from './core/guards/public.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/public/landing/landing').then((m) => m.Landing),
    title: 'PhantomGG - Esports Tournament Platform',
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
      import('./features/dashboard-selection/dashboard-selection.component').then((m) => m.DashboardSelectionComponent),
    title: 'Dashboard',
  },
  {
    path: 'public',
    children: publicRoutes,
  },
  {
    path: 'user',
    canActivate: [authGuard],
    children: userRoutes,
  },
  {
    path: 'organizer',
    canActivate: [authGuard],
    data: { roles: [Roles.Organizer, Roles.Admin] },
    children: organizerRoutes,
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/unauthorized/unauthorized').then((m) => m.Unauthorized),
    title: 'Unauthorized - PhantomGG',
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
