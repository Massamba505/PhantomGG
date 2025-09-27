import { authGuard } from '@/app/core/guards/auth.guard';
import { Routes } from '@angular/router';

export const organizerRoutes: Routes = [
  {
    path: '',
    loadComponent:()=>import("./organizer").then(m=>m.OrganizerComponent),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/organizer-dashboard.component').then(m => m.OrganizerDashboardComponent),
        title: 'Organizer Dashboard - PhantomGG',
        data:{title:"Dashboard"}
      },
      {
        path: 'tournaments',
        children: [
          {
            path: '',
            loadComponent: () => import('./pages/tournaments/tournament-list/tournament-list.component').then(m => m.TournamentListComponent),
            title: 'My Tournaments - PhantomGG',
            data:{title:"Tournaments"}
          },
          {
            path: 'create',
            loadComponent: () => import('./pages/tournaments/create-tournament/create-tournament.component').then(m => m.CreateTournamentComponent),
            title: 'Create Tournament - PhantomGG',
            data:{title:"Create Tournaments"}
          },
          {
            path: ':id',
            loadComponent: () => import('./pages/tournaments/tournament-details/tournament-details.component').then(m => m.TournamentDetailsComponent),
            title: 'Tournament Details - PhantomGG',
            data:{title:"Tournament Details"}
          },
          {
            path: ':id/edit',
            loadComponent: () => import('./pages/tournaments/edit-tournament/edit-tournament.component').then(m => m.EditTournamentComponent),
            title: 'Edit Tournament - PhantomGG',
            data:{title:"Edit Tournaments"}
          }
        ]
      }
    ]
  }
];
