import {
  HttpErrorResponse,
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { TokenRefreshService } from '@/app/core/services/tokenRefresh.service';

export function apiInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const tokenService = inject(TokenRefreshService);

  req = req.clone({ withCredentials: true });

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
    }
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint) {
        return tokenService.tryRefreshAndReplay(req, next);
      }
      return throwError(() => error);
    })
  );
}
