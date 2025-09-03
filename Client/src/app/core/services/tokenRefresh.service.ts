import { Injectable, inject } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  switchMap,
  catchError,
  throwError,
  filter,
  take,
} from 'rxjs';
import { HttpRequest, HttpHandlerFn, HttpEvent } from '@angular/common/http';
import { AuthService } from './auth.service';
import { TokenStorage } from '@/app/shared/utils/tokenStorage';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshService {
  private refreshInProgress = new BehaviorSubject<boolean>(false);
  private refreshTokenSubject = new BehaviorSubject<string | null>(null);

  private authService = inject(AuthService);

  getToken(): string | null {
    return TokenStorage.getAccessToken();
  }

  isTokenExpired(): boolean {
    return TokenStorage.isTokenExpired();
  }

  setToken(token: string): void {
    TokenStorage.setAccessToken(token);
  }

  clearTokens(): void {
    TokenStorage.clear();
  }

  tryRefreshAndReplay(
    req: HttpRequest<any>,
    next: HttpHandlerFn
  ): Observable<HttpEvent<any>> {
    if (this.refreshInProgress.value) {
      return this.refreshTokenSubject.pipe(
        filter((token) => token !== null),
        take(1),
        switchMap((token) => {
          const cloned = req.clone({
            setHeaders: { Authorization: `Bearer ${token}` },
          });
          return next(cloned);
        })
      );
    }

    this.refreshInProgress.next(true);
    this.refreshTokenSubject.next(null);

    return this.authService.refresh().pipe(
      switchMap((res: any) => {
        this.refreshInProgress.next(false);
        const token = res.data.accessToken;
        this.setToken(token);

        this.refreshTokenSubject.next(token);

        const cloned = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        });

        return next(cloned);
      }),
      catchError((err) => {
        this.refreshInProgress.next(false);
        this.refreshTokenSubject.next(null);
        this.clearTokens();
        return throwError(() => err);
      })
    );
  }
}
