import { AuthStateService } from '@/app/store/AuthStateService';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import {
  Observable,
  catchError,
  switchMap,
  throwError,
  of,
  lastValueFrom,
} from 'rxjs';
import { TokenStorage } from '../../shared/utils/tokenStorage';

export function apiInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const authState = inject(AuthStateService);

  req = req.clone({
    withCredentials: true,
  });

  // Skip adding token for auth endpoints that don't need it
  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/refresh');

  // For non-auth endpoints, add the token if available
  if (!isAuthEndpoint) {
    const token = TokenStorage.getAccessToken();

    // Check if token exists and is not expired
    if (token && !TokenStorage.isTokenExpired()) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    } else if (token && TokenStorage.isTokenExpired()) {
      // Token exists but is expired, try to refresh it first
      return refreshTokenAndRetry(authState, req, next);
    }
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // If we get a 401 Unauthorized error and we're not already trying to refresh
      if (error.status === 401 && !req.url.includes('/auth/refresh')) {
        return refreshTokenAndRetry(authState, req, next);
      }

      return throwError(() => error);
    })
  );
}

function refreshTokenAndRetry(
  authState: AuthStateService,
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  console.log('Token expired or invalid, attempting to refresh...');

  return authState.refreshToken().pipe(
    switchMap((response) => {
      console.log('Token refreshed successfully');

      // Clone the request with the new token
      const token = TokenStorage.getAccessToken();
      if (token) {
        req = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        });
      }

      // Retry the request with the new token
      return next(req);
    }),
    catchError((refreshError) => {
      console.error('Token refresh failed, logging out:', refreshError);
      authState.logout();
      return throwError(() => refreshError);
    })
  );
}
