import {
  ApplicationConfig,
  APP_INITIALIZER,
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { ThemeService } from './shared/services/theme.service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { apiInterceptor } from './core/interceptors/Api.interceptor';
import { MessageService } from 'primeng/api';
import { AuthStateService } from './store/AuthStateService';
import { errorInterceptor } from './core/interceptors/error.interceptor';

function initializeTheme(themeService: ThemeService) {
  return () => {
    return Promise.resolve();
  };
}

function initializeAuthState(authStateService: AuthStateService) {
  return () => {
    return authStateService.initAuthState();
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    MessageService,
    provideHttpClient(withInterceptors([apiInterceptor, errorInterceptor])),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideAnimationsAsync(),
    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: '.phantomGG-dark',
        },
      },
    }),
    {
      provide: APP_INITIALIZER,
      useFactory: initializeTheme,
      deps: [ThemeService],
      multi: true,
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeAuthState,
      deps: [AuthStateService],
      multi: true,
    },
  ],
};
