import { Injectable, computed, inject, signal } from '@angular/core';
import {
  LoginRequest,
  SignUpRequest,
  AuthResponse,
} from '@/app/shared/models/Authentication';
import {
  catchError,
  tap,
  throwError,
  firstValueFrom,
  finalize,
  Observable,
} from 'rxjs';
import { AuthService } from '../core/services/auth.service';
import { TokenRefreshService } from '../core/services/tokenRefresh.service';
import { User } from '../shared/models/User';
import { ApiResponse } from '../shared/models/ApiResponse';

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

  private withLoading<T>(obs$: Observable<T>): Observable<T> {
    this.loadingSignal.set(true);
    return obs$.pipe(finalize(() => this.loadingSignal.set(false)));
  }

  async initAuthState(): Promise<boolean> {
    const token = this.tokenService.getToken();
    if (!token) return true;

    try {
      await firstValueFrom(this.loadUser());
      return true;
    } catch {
      this.clearAuthState();
      return false;
    }
  }

  login(credentials: LoginRequest) {
    return this.withLoading(
      this.authService.login(credentials).pipe(
        tap((res) => this.handleAuthSuccess(res)),
        catchError((error) => throwError(() => error))
      )
    );
  }

  signup(credentials: SignUpRequest) {
    return this.withLoading(
      this.authService.signup(credentials).pipe(
        tap((res) => this.handleAuthSuccess(res)),
        catchError((error) => throwError(() => error))
      )
    );
  }

  logout(): Observable<ApiResponse<any>> {
    return this.withLoading(
      this.authService.logout().pipe(
        tap(() => this.clearAuthState()),
        catchError((error) => {
          this.clearAuthState();
          return throwError(() => error);
        })
      )
    );
  }

  private loadUser() {
    return this.withLoading(
      this.authService.getMe().pipe(
        tap((res) => this.userSignal.set(res.data!)),
        catchError((err) => throwError(() => err))
      )
    );
  }

  private handleAuthSuccess(res: ApiResponse<AuthResponse>) {
    const { accessToken, user } = res.data!;
    this.tokenService.setToken(accessToken!);
    this.userSignal.set(user!);
  }

  private clearAuthState() {
    this.tokenService.clearTokens();
    this.userSignal.set(null);
  }

  updateUser(user: User) {
    this.userSignal.set(user);
  }
}
