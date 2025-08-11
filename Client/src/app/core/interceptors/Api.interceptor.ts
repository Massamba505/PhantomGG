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
  BehaviorSubject,
} from 'rxjs';
import { TokenStorage } from '@/app/shared/utils/tokenStorage';

const refreshTokenInProgress = new BehaviorSubject<boolean>(false);
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export function apiInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const authState = inject(AuthStateService);

  req = req.clone({
    withCredentials: true,
  });

  const isAuthEndpoint =
    req.url.includes('/auth/login') ||
    req.url.includes('/auth/register') ||
    req.url.includes('/auth/refresh');

  if (!isAuthEndpoint) {
    const token = TokenStorage.getAccessToken();

    if (token && !TokenStorage.isTokenExpired()) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      });
    } else if (token && TokenStorage.isTokenExpired()) {
      return handleExpiredToken(authState, req, next);
    }
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint) {
        return handleExpiredToken(authState, req, next);
      }

      return throwError(() => error);
    })
  );
}

function handleExpiredToken(
  authState: AuthStateService,
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

  return authState.refreshToken().pipe(
    switchMap(() => {
      refreshTokenInProgress.next(false);
      const newToken = TokenStorage.getAccessToken();
      refreshTokenSubject.next(newToken);

      if (newToken) {
        req = req.clone({
          setHeaders: { Authorization: `Bearer ${newToken}` },
        });
      }
      return next(req);
    }),
    catchError((refreshError) => {
      refreshTokenInProgress.next(false);
      refreshTokenSubject.next(null);
      authState.logout();
      return throwError(() => refreshError);
    })
  );
}
