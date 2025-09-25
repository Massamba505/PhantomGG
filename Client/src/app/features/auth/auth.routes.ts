import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
    title: 'PhantomGG - Login',
  },
  {
    path: 'signup',
    loadComponent: () => import('./pages/signup/signup').then((m) => m.Signup),
    title: 'PhantomGG - Sign Up',
  },
  {
    path: 'role-selection',
    loadComponent: () => import('./pages/role-selection/role-selection-page').then((m) => m.RoleSelectionPage),
    title: 'PhantomGG - Choose Your Role',
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];
