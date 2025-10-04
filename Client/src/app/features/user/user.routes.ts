import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  {
    path: '',
    loadComponent:()=>import("./dashboard").then(m=>m.Dashboard),
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/user-dashboard').then(m => m.UserDashboard),
        title: 'Dashboard - PhantomGG',
        data:{title:"Dashboard"}
      },
      {
        path: 'teams',
        loadComponent: () => import('./pages/teams/user-teams').then(m => m.UserTeams),
        title: 'My Teams - PhantomGG',
        data:{title:"My Teams"}
      },
      {
        path: 'teams/create',
        loadComponent: () => import('./pages/create-team/create-team.component').then(m => m.CreateTeamComponent),
        title: 'Create Team - PhantomGG',
        data:{title:"Create Team"}
      },
      {
        path: 'teams/:id',
        loadComponent: () => import('./pages/team-details/team-details.component').then(m => m.TeamDetailsComponent),
        title: 'Team Details - PhantomGG',
        data:{title:"Team Details"}
      },
      {
        path: 'teams/:id/edit',
        loadComponent: () => import('./pages/edit-team/edit-team').then(m => m.EditTeamComponent),
        title: 'Edit Team - PhantomGG',
        data:{title:"Edit Team"}
      },
      {
        path: 'tournaments',
        loadComponent: () => import('./pages/tournaments/user-tournaments').then(m => m.UserTournaments),
        title: 'Tournaments - PhantomGG',
        data:{title:"Tournaments"}
      },
      {
        path: 'tournaments/:id',
        loadComponent: () => import('../public/tournaments/details/tournament-details').then(m => m.TournamentDetails),
        title: 'Tournaments - PhantomGG',
        data:{title:"Tournaments"}
      }
    ]
  }
];
