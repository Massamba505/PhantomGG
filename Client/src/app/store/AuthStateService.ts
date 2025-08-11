import { Injectable, computed, inject, signal } from '@angular/core';
import {
  AuthResponse,
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
    const token = TokenStorage.getAccessToken();
    if (token) {
      this.loadUser();
    }
  }

  login(credentials: LoginRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.login(credentials).pipe(
      tap((res: AuthResponse) => {
        this.loadingSignal.set(false);
        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);
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
        this.errorSignal.set(error.error.message ?? 'Login failed');
        return throwError(() => error);
      })
    );
  }

  signup(credentials: SignUpRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.signup(credentials).pipe(
      tap((res: AuthResponse) => {
        this.loadingSignal.set(false);
        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);
          if (res.user) this.userSignal.set(res.user);
        } else {
          this.errorSignal.set(res.message ?? 'Signup failed');
        }
      }),
      catchError((error) => {
        this.loadingSignal.set(false);
        this.errorSignal.set('Signup failed');
        return throwError(() => error);
      })
    );
  }

  logout() {
    this.authService.logout().subscribe();
    TokenStorage.clear();
    this.userSignal.set(null);
  }

  refresh() {
    return this.authService.refresh().pipe(
      tap((res: AuthResponse) => {
        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);
        } else {
          this.logout();
        }
      }),
      catchError((error) => {
        this.loadingSignal.set(false);
        this.errorSignal.set('Refresh failed');
        return throwError(() => error);
      })
    );
  }

  loadUser() {
    this.loadingSignal.set(true);
    this.authService.getMe().subscribe({
      next: (res: any) => {
        this.loadingSignal.set(false);
        if (res && res.id) {
          this.userSignal.set(res);
        } else {
          this.logout();
        }
      },
      error: (err) => {
        this.loadingSignal.set(false);
        this.logout();
      },
    });
  }
}
