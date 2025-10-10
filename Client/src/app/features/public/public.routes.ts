import { Routes } from '@angular/router';

export const publicRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('../../shared/components/layouts/public-layout/public-layout').then(m => m.PublicLayout),
    children: [
      {
        path: 'tournaments',
        loadComponent: () => import('./tournaments/browse/tournament-browse').then(m => m.TournamentBrowse),
        title: 'Browse Tournaments - PhantomGG'
      },
      {
        path: 'tournaments/:id',
        loadComponent: () => import('../../shared/components/pages/tournament-details/tournament-details').then(m => m.TournamentDetails),
        title: 'Tournament Details - PhantomGG'
      },
      {
        path: 'tournaments/:id/statistics',
        loadComponent: () => import('../../shared/components/pages/tournament-stats/tournament-stats').then(m => m.TournamentStatsComponent),
        title: 'Tournament Statistics - PhantomGG'
      },
    ]
  }
];
