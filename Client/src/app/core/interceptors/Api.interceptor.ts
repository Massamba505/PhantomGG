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
  BehaviorSubject,
} from 'rxjs';
import { TokenRefreshService } from '@/app/core/services/tokenRefresh.service';
import { AuthService } from '@/app/core/services/auth.service';

const refreshTokenInProgress = new BehaviorSubject<boolean>(false);
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export function apiInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const tokenService = inject(TokenRefreshService);

  req = req.clone({
    withCredentials: true,
  });

  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/refresh');

  if (!isAuthEndpoint) {
    const token = tokenService.getToken();

    if (token && !tokenService.isTokenExpired()) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    } else if (token && tokenService.isTokenExpired()) {
      return handleExpiredToken(tokenService, req, next);
    }
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint) {
        return handleExpiredToken(tokenService, req, next);
      }

      return throwError(() => error);
    })
  );
}

function handleExpiredToken(
  tokenService: TokenRefreshService,
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  if (refreshTokenInProgress.value) {
    return refreshTokenSubject.pipe(
      switchMap((token) => {
        if (token) {
          req = req.clone({
            setHeaders: { Authorization: `Bearer ${token}` },
          });
        }
        return next(req);
      })
    );
  }

  refreshTokenInProgress.next(true);
  refreshTokenSubject.next(null);

  const authService = inject(AuthService);

  return authService.refresh().pipe(
    switchMap((response: any) => {
      refreshTokenInProgress.next(false);

      if (response.success) {
        tokenService.setToken(response.data.accessToken);
        tokenService.setTokenExpiry(response.data.accessTokenExpiresAt);
        refreshTokenSubject.next(response.data.accessToken);

        req = req.clone({
          setHeaders: { Authorization: `Bearer ${response.data.accessToken}` },
        });
      }
      return next(req);
    }),
    catchError((refreshError) => {
      refreshTokenInProgress.next(false);
      refreshTokenSubject.next(null);
      tokenService.clearTokens();
      return throwError(() => refreshError);
    })
  );
}
