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
        title: 'Organizer Dashboard - PhantomGG',
        data:{title:"Dashboard"}
      }
    ]
  }
];
