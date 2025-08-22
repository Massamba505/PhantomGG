import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';
import { authGuard } from './core/guards/auth.guard';
import { authenticatedGuard } from './core/guards/authenticated.guard';
import { UnauthorizedComponent } from './shared/components/unauthorized/unauthorized';
import { DashboardComponent } from './features/dashboard/dashboard';
import { TournamentDetailsComponent } from './features/tournament-details/tournament-details.component';

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
      import('./features/dashboard/dashboard').then(
        (m) => m.DashboardComponent
      ),
  },
  { path: 'tournament/:id', component: TournamentDetailsComponent },
  {
    path: 'tournaments',
    loadComponent: () =>
      import('./features/tournaments/tournaments').then(
        (m) => m.TournamentsComponent
      ),
  },
  {
    path: 'create-tournament',
    loadComponent: () =>
      import('./features/create-tournament/create-tournament.component').then(
        (m) => m.CreateTournamentComponent
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
    component: UnauthorizedComponent,
    title: 'Unauthorized - PhantomGG',
  },
  {
    path: '**',
    redirectTo: '',
  },
];
