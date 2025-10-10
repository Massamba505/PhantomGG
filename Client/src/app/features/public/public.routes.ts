import { Routes } from '@angular/router';

export const publicRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./landing/landing').then(m => m.Landing),
    title: 'PhantomGG'
  },
  {
    path: 'tournaments',
    loadComponent: () => import('./tournaments/browse/tournament-browse').then(m => m.TournamentBrowse),
    title: 'Browse Tournaments - PhantomGG'
  },
  {
    path: 'tournaments/:id',
    loadComponent: () => import('./tournaments/details/tournament-details').then(m => m.TournamentDetails),
    title: 'Tournament Details - PhantomGG'
  },
  {
    path: 'tournaments/:id/statistics',
    loadComponent: () => import('../../shared/components/pages/tournament-stats/tournament-stats.component').then(m => m.TournamentStatsComponent),
    title: 'Tournament Statistics - PhantomGG'
  },
];
