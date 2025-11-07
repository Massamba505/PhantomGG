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
    loadComponent: () =>
      import('./pages/role-selection/role-selection-page').then(
        (m) => m.RoleSelectionPage
      ),
    title: 'PhantomGG - Choose Your Role',
  },
  {
    path: 'verify-email',
    loadComponent: () =>
      import('./pages/verify-email/verify-email').then((m) => m.VerifyEmail),
    title: 'PhantomGG - Verify Email',
  },
  {
    path: 'verify-email-sent',
    loadComponent: () =>
      import('./pages/verify-email-sent/verify-email-sent').then(
        (m) => m.VerifyEmailSent
      ),
    title: 'PhantomGG - Email Sent',
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/forgot-password/forgot-password').then(
        (m) => m.ForgotPassword
      ),
    title: 'PhantomGG - Forgot Password',
  },
  {
    path: 'reset-password',
    loadComponent: () =>
      import('./pages/reset-password/reset-password').then(
        (m) => m.ResetPassword
      ),
    title: 'PhantomGG - Reset Password',
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
];
