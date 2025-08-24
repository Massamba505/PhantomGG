import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard } from './core/guards/auth.guard';
import { authenticatedGuard } from './core/guards/authenticated.guard';
import { Unauthorized } from './shared/components/unauthorized/unauthorized';
import { NotFoundComponent } from './features/not-found/not-found';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/landing').then((m) => m.Landing),
    title: 'PhantomGG',
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/dashboard').then((m) => m.Dashboard),
  },
  {
    path: 'tournament-details/:id',
    loadComponent: () =>
      import('./features/tournament-details/tournament-details').then(
        (m) => m.TournamentDetails
      ),
  },
  {
    path: 'tournament/:id',
    redirectTo: 'tournament-details/:id',
    pathMatch: 'full',
  },
  {
    path: 'tournaments',
    loadComponent: () =>
      import('./features/tournaments/tournaments').then((m) => m.Tournaments),
  },
  {
    path: 'create-tournament',
    loadComponent: () =>
      import('./features/create-tournament/create-tournament').then(
        (m) => m.CreateTournament
      ),
  },
  {
    path: 'auth',
    canActivate: [authenticatedGuard],
    loadComponent: () => import('./features/auth/auth').then((m) => m.Auth),
    children: authRoutes,
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
    data: { roles: ['Admin'] },
    loadComponent: () => import('./features/admin/admin').then((m) => m.Admin),
    title: 'Admin Dashboard - PhantomGG',
  },
  {
    path: 'unauthorized',
    component: Unauthorized,
    title: 'Unauthorized - PhantomGG',
  },
  {
    path: '**',
    component: NotFoundComponent,
  },
];
