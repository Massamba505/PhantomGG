// src/app/shared/interceptors/api.interceptor.ts
import { AuthStateService } from '@/app/store/AuthStateService';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { TokenStorage } from '../utils/tokenStorage';

export function apiInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const authState = inject(AuthStateService);

  const token = TokenStorage.getAccessToken();
  let clonedReq = req;
  if (token) {
    clonedReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(clonedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/auth/login')) {
        return authState.refresh().pipe(
          switchMap(() => {
            const newToken = TokenStorage.getAccessToken();
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${newToken}` },
            });
            return next(retryReq);
          }),
          catchError((refreshError) => {
            authState.logout();
            return throwError(() => refreshError);
          })
        );
      }

      let message = 'Something went wrong';
      if (error.error?.message) {
        message = error.error.message;
      } else if (error.status === 0) {
        message = 'Cannot connect to server';
      }
      authState.setError(message);

      return throwError(() => error);
    })
  );
}
