import { Routes } from '@angular/router';
import { authGuard } from '@/app/core/guards/auth.guard';
import { Roles } from '@/app/shared/constants/roles';

export const tournamentRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./tournaments').then((m) => m.Tournaments),
    title: 'Tournaments',
  },
  {
    path: 'details/:id',
    loadComponent: () =>
      import('../tournament-details/tournament-details').then(
        (m) => m.TournamentDetails
      ),
    title: 'Tournament Details',
  },
  {
    path: 'create',
    canActivate: [authGuard],
    data: { roles: [Roles.Organizer] },
    loadComponent: () =>
      import('../create-tournament/create-tournament').then(
        (m) => m.CreateTournament
      ),
    title: 'Create Tournament',
  },
  {
    path: 'edit/:id',
    canActivate: [authGuard],
    data: { roles: [Roles.Organizer] },
    loadComponent: () =>
      import('../edit-tournament/edit-tournament').then(
        (m) => m.EditTournament
      ),
    title: 'Edit Tournament',
  },
];
