import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard } from './core/guards/auth.guard';
import { authenticatedGuard } from './core/guards/authenticated.guard';
import { roleGuard } from './core/guards/role.guard';
import { publicGuard } from './core/guards/public.guard';
import { NotFoundComponent } from './features/not-found/not-found';
import { Roles } from './shared/constants/roles';
import { tournamentRoutes } from './features/tournaments/tournament.routes';
import { userRoutes } from './features/user/user.routes';
import { publicRoutes } from './features/public/public.routes';

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
    canActivate: [authenticatedGuard],
    loadComponent: () => import('./features/auth/auth').then((m) => m.Auth),
    children: authRoutes,
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
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/profile/profile').then((m) => m.Profile),
    title: 'My Profile - PhantomGG',
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    data: { roles: [Roles.Admin] },
    loadComponent: () => import('./features/admin/admin').then((m) => m.Admin),
    title: 'Admin Dashboard - PhantomGG',
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'tournaments',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Organizer, Roles.Admin] },
    children: tournamentRoutes,
  },
  {
    path: 'teams',
    canActivate: [authGuard, roleGuard],
    data: { roles: [Roles.Organizer, Roles.Admin] },
    loadComponent: () =>
      import('./features/teams/teams').then((m) => m.Teams),
    title: 'Teams Management - PhantomGG',
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
