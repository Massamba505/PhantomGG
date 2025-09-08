import { Routes } from '@angular/router';
import { authGuard } from '@/app/core/guards/auth.guard';
import { roleGuard } from '@/app/core/guards/role.guard';
import { Roles } from '@/app/shared/constants/roles';

export const userRoutes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./dashboard/user-dashboard').then((m) => m.UserDashboard),
    title: 'My Dashboard - PhantomGG',
  },
  {
    path: 'tournaments',
    loadComponent: () =>
      import('./my-tournaments/my-tournaments').then((m) => m.MyTournaments),
    title: 'My Tournaments - PhantomGG',
  },
  {
    path: 'teams',
    loadComponent: () =>
      import('./my-teams/my-teams').then((m) => m.MyTeams),
    title: 'My Teams - PhantomGG',
  },
  {
    path: 'schedule',
    loadComponent: () =>
      import('./my-schedule/my-schedule').then((m) => m.MySchedule),
    title: 'My Schedule - PhantomGG',
  },
  {
    path: 'join-tournament',
    loadComponent: () =>
      import('./join-tournament/join-tournament').then((m) => m.JoinTournament),
    title: 'Join Tournament - PhantomGG',
  },
  {
    path: 'join-tournament/:id',
    loadComponent: () =>
      import('./join-tournament/join-tournament').then((m) => m.JoinTournament),
    title: 'Join Tournament - PhantomGG',
  },
];
