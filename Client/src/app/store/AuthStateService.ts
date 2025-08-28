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

  readonly user = this.userSignal.asReadonly();
  readonly loading = this.loadingSignal.asReadonly();
  readonly isAuthenticated = computed(() => !!this.userSignal());

  async initAuthState(): Promise<boolean> {
    const token = this.tokenService.getToken();
    if (!token) {
      return true;
    }

    try {
      if (this.tokenService.isTokenExpired()) {
        const refreshResponse: any = await firstValueFrom(
          this.authService.refresh()
        );
        this.tokenService.setToken(refreshResponse.data.accessToken);
        
        await firstValueFrom(this.loadUser());
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

    return this.authService.login(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);
        this.tokenService.setToken(res.data.accessToken);
        this.userSignal.set(res.data.user);
      }),
      catchError((error) => {
        this.loadingSignal.set(false);
        return throwError(() => error);
      })
    );
  }

  signup(credentials: SignUpRequest) {
    this.loadingSignal.set(true);

    return this.authService.signup(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);
        this.tokenService.setToken(res.data.accessToken);
        this.userSignal.set(res.data.user);
      }),
      catchError((error) => {
        this.loadingSignal.set(false);
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
  }
}
