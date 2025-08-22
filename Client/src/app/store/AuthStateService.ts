import { Injectable, computed, inject, signal } from '@angular/core';
import {
  LoginRequest,
  SignUpRequest,
} from '@/app/shared/models/Authentication';
import { catchError, tap, throwError, firstValueFrom } from 'rxjs';
import { AuthService } from '../core/services/auth.service';
import { TokenRefreshService } from '../core/services/tokenRefresh.service';
import { User } from '../shared/models/User';

@Injectable({
  providedIn: 'root',
})
export class AuthStateService {
  private authService = inject(AuthService);
  private tokenService = inject(TokenRefreshService);

  private userSignal = signal<User | null>(null);
  private loadingSignal = signal<boolean>(false);
  private errorSignal = signal<string | null>(null);

  readonly user = this.userSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly error = this.errorSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.userSignal());

  async initAuthState(): Promise<boolean> {
    const token = this.tokenService.getToken();
    if (!token) {
      return true;
    }

    try {
      if (this.tokenService.isTokenExpired()) {
        const refreshResponse: any = await firstValueFrom(this.authService.refresh());
        
        if (refreshResponse.success) {
          this.tokenService.setToken(refreshResponse.data.accessToken);
          this.tokenService.setTokenExpiry(refreshResponse.data.accessTokenExpiresAt);
          await firstValueFrom(this.loadUser());
        } else {
          throw new Error('Refresh failed');
        }
      } else {
        await firstValueFrom(this.loadUser());
      }
      
      return true;
    } catch (error) {
      this.tokenService.clearTokens();
      return false;
    }
  }

  login(credentials: LoginRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.login(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);

        if (res.success) {
          this.tokenService.setToken(res.data.accessToken);
          this.tokenService.setTokenExpiry(res.data.accessTokenExpiresAt);
          this.userSignal.set(res.data.user);
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
      tap((res: any) => {
        this.loadingSignal.set(false);

        if (res.success && res.data.accessToken) {
          this.tokenService.setToken(res.data.accessToken);
          this.tokenService.setTokenExpiry(res.data.accessTokenExpiresAt);
          this.userSignal.set(res.data.user);
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
        this.clearAuthState();
      },
      error: () => {
        // Clear state even if logout request fails
        this.clearAuthState();
      },
    });
  }

  private loadUser() {
    this.loadingSignal.set(true);
    return this.authService.getMe().pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);
        this.userSignal.set(res.data);
      }),
      catchError((err) => {
        this.loadingSignal.set(false);
        return throwError(() => err);
      })
    );
  }

  private clearAuthState() {
    this.loadingSignal.set(false);
    this.tokenService.clearTokens();
    this.userSignal.set(null);
    this.errorSignal.set(null);
  }
}
