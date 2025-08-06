import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () =>
      import('./features/landing/landing').then((m) => m.Landing),
    title: 'PhantomGG - Home',
  },
  {
    path: 'auth',
    loadComponent: () => import('./features/auth/auth').then((m) => m.Auth),
    children: [
      {
        path: 'login',
        loadComponent: () => 
          import('./features/auth/pages/login/login').then((m) => m.Login),
        title: 'PhantomGG - Login'
      },
      {
        path: 'signup',
        loadComponent: () => 
          import('./features/auth/pages/signup/signup').then((m) => m.Signup),
        title: 'PhantomGG - Sign Up'
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  }
];
