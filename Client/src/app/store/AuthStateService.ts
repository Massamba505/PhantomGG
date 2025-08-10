import { Injectable, computed, inject, signal } from '@angular/core';
import {
  User,
  AuthResponse,
  LoginRequest,
  SignUpRequest,
} from '@/app/shared/models/Authentication';
import { tap } from 'rxjs';
import { AuthService } from '../features/auth/services/authService';
import { TokenStorage } from '../shared/utils/tokenStorage';

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

  private globalErrorSignal = signal<string | null>(null);
  readonly globalError = this.globalErrorSignal.asReadonly();

  setError(message: string) {
    this.errorSignal.set(message);
  }

  login(credentials: LoginRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.login(credentials).pipe(
      tap((res: AuthResponse) => {
        this.loadingSignal.set(false);
        if (res.success && res.accessToken) {
          TokenStorage.setAccessToken(res.accessToken);
          if (res.user) this.userSignal.set(res.user);
        } else {
          this.errorSignal.set(res.message ?? 'Login failed');
        }
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
      })
    );
  }

  loadUser() {
    this.authService.getMe().subscribe({
      next: (res) => {
        if (res.success && res.user) {
          this.userSignal.set(res.user);
        } else {
          this.logout();
        }
      },
      error: () => this.logout(),
    });
  }
}
