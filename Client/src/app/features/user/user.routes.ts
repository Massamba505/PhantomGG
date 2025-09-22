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
        loadComponent: () => import('./pages/teams/create-team/create-team.component').then(m => m.CreateTeamComponent),
        title: 'Create Team - PhantomGG',
        data:{title:"Create Team"}
      },
      {
        path: 'teams/:id/edit',
        loadComponent: () => import('./pages/teams/edit-team/edit-team.component').then(m => m.EditTeamComponent),
        title: 'Edit Team - PhantomGG',
        data:{title:"Edit Team"}
      },
      {
        path: 'tournaments',
        loadComponent: () => import('./pages/tournaments/user-tournaments').then(m => m.UserTournaments),
        title: 'Tournaments - PhantomGG',
        data:{title:"Tournaments"}
      }
    ]
  }
];
