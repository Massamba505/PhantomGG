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
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];
