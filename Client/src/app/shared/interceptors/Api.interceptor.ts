import { AuthStateService } from '@/app/store/AuthStateService';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { TokenStorage } from '../utils/tokenStorage';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  private authState = inject(AuthStateService);

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = TokenStorage.getAccessToken();
    let clonedReq = req;
    if (token) {
      clonedReq = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }

    return next.handle(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !req.url.includes('/auth/login')) {
          return this.authState.refresh().pipe(
            switchMap(() => {
              const newToken = TokenStorage.getAccessToken();
              const retryReq = req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` },
              });
              return next.handle(retryReq);
            }),
            catchError((refreshError) => {
              this.authState.logout();
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

        this.authState.setError(message);

        return throwError(() => error);
      })
    );
  }
}
