import { Injectable, computed, inject, signal } from '@angular/core';
import {
  LoginRequest,
  SignUpRequest,
} from '@/app/shared/models/Authentication';
import { catchError, tap, throwError } from 'rxjs';
import { AuthService } from '../core/services/authService';
import { TokenStorage } from '../shared/utils/tokenStorage';
import { User } from '../shared/models/User';

@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private authService = inject(AuthService);

  private userSignal = signal<User | null>(null);
  private loadingSignal = signal<boolean>(false);
  private errorSignal = signal<string | null>(null);

  readonly user = this.userSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.userSignal());

  constructor() {
    this.initAuthState();
  }

  initAuthState(): Promise<boolean> {
    const token = TokenStorage.getAccessToken();
    if (token && !TokenStorage.isTokenExpired() && this.userSignal() == null) {
      return new Promise<boolean>((resolve) => {
        this.loadUser().subscribe({
          next: () => resolve(true),
          error: () => resolve(false),
        });
      });
    }
    return Promise.resolve(true);
  }

  login(credentials: LoginRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.login(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);

        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }

          if (res.user) {
            this.userSignal.set(res.user);
          } else {
            this.loadUser();
          }
        } else {
          this.errorSignal.set(res.message ?? 'Login failed');
        }
      }),
      catchError((error) => {
        this.loadingSignal.set(false);

        let errorMsg = 'Login failed';
        if (error.error && error.error.message) {
          errorMsg = error.error.message;
        } else if (error.status === 0) {
          errorMsg = 'Cannot connect to server';
        } else if (error.status === 401) {
          errorMsg = 'Invalid email or password';
        }

        this.errorSignal.set(errorMsg);
        return throwError(() => error);
      })
    );
  }

  signup(credentials: SignUpRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.signup(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);

        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }

          if (res.user) {
            this.userSignal.set(res.user);
          } else {
            this.loadUser();
          }
        } else {
          this.errorSignal.set(res.message ?? 'Signup failed');
        }
      }),
      catchError((error) => {
        this.loadingSignal.set(false);

        let errorMsg = 'Signup failed';
        if (error.error && error.error.message) {
          errorMsg = error.error.message;
        }

        this.errorSignal.set(errorMsg);
        return throwError(() => error);
      })
    );
  }

  logout() {
    this.loadingSignal.set(true);

    this.authService.logout().subscribe({
      next: () => {
        this.loadingSignal.set(false);
        TokenStorage.clear();
        this.userSignal.set(null);
      },
      error: (err) => {
        this.loadingSignal.set(false);
        TokenStorage.clear();
        this.userSignal.set(null);
      },
    });
  }

  refreshToken() {
    this.loadingSignal.set(true);

    return this.authService.refresh().pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);

        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }
        } else {
          this.logout();
        }
      }),
      catchError((error) => {
        this.loadingSignal.set(false);
        this.logout();
        return throwError(() => error);
      })
    );
  }

  loadUser() {
    this.loadingSignal.set(true);
    return this.authService.getMe().pipe(
      tap((user: User) => {
        this.loadingSignal.set(false);
        this.userSignal.set(user);
      }),
      catchError((err) => {
        this.loadingSignal.set(false);
        this.logout();
        return throwError(() => err);
      })
    );
  }
}
