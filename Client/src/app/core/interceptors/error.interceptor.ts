import {
  HttpErrorResponse,
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { ToastService } from '@/app/shared/services/toast.service';

export function errorInterceptor(
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthEndpoint = req.url.includes('/auth/login') || 
                            req.url.includes('/auth/register') || 
                            req.url.includes('/auth/refresh');
      
      if (error.status === 401) {
        if (isAuthEndpoint) {
          if (req.url.includes('/refresh')) {
            toast.error('Your session has expired. Please log in again.');
          } else {
            toast.error(error.error?.message);
          }
        }
        return throwError(() => error);
      }
      
      if (error.status === 0) {
        toast.warn('Network error. Please check your internet.');
      } else if (error.status >= 500) {
        toast.error('Server error. Please try again later.');
      } else if (error.status >= 400) {
        toast.error(error.error?.message || 'Something went wrong.');
      }

      return throwError(() => error);
    })
  );
}
