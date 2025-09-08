import { Routes } from '@angular/router';
import { guestGuard } from '@/app/core/guards/public.guard';

export const publicRoutes: Routes = [
  {
    path: '',
    canActivate: [guestGuard],
    children: [
      {
        path: 'tournaments',
        loadComponent: () =>
          import('./tournaments/browse-tournaments').then((m) => m.BrowseTournaments),
        title: 'Browse Tournaments - PhantomGG',
      },
      // TODO: Implement these components
      // {
      //   path: 'tournaments/:id',
      //   loadComponent: () =>
      //     import('./tournaments/tournament-details').then((m) => m.PublicTournamentDetails),
      //   title: 'Tournament Details - PhantomGG',
      // },
      // {
      //   path: 'teams',
      //   loadComponent: () =>
      //     import('./teams/browse-teams').then((m) => m.BrowseTeams),
      //   title: 'Browse Teams - PhantomGG',
      // },
      // {
      //   path: 'teams/:id',
      //   loadComponent: () =>
      //     import('./teams/team-details').then((m) => m.PublicTeamDetails),
      //   title: 'Team Details - PhantomGG',
      // },
      // {
      //   path: 'results',
      //   loadComponent: () =>
      //     import('./results/match-results').then((m) => m.MatchResults),
      //   title: 'Match Results - PhantomGG',
      // },
      // {
      //   path: 'players',
      //   loadComponent: () =>
      //     import('./players/browse-players').then((m) => m.BrowsePlayers),
      //   title: 'Browse Players - PhantomGG',
      // },
    ]
  }
];
