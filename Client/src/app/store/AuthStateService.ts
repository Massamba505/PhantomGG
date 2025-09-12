import { Injectable, computed, inject, signal } from '@angular/core';
import {
  catchError,
  tap,
  throwError,
  firstValueFrom,
  finalize,
  Observable,
} from 'rxjs';
import { TokenRefreshService } from '../core/services/tokenRefresh.service';
import { ApiResponse, Auth, LoginRequest, RegisterRequest, User } from '../api/models';
import { AuthService } from '../api/services';

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

  signup(credentials: RegisterRequest) {
    return this.withLoading(
      this.authService.signup(credentials).pipe(
        tap((res) => this.handleAuthSuccess(res)),
        catchError((error) => throwError(() => error))
      )
    );
  }

  logout(): Observable<void> {
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
      this.authService.getCurrentUser().pipe(
        tap((data) => this.userSignal.set(data)),
        catchError((err) => throwError(() => err))
      )
    );
  }

  private handleAuthSuccess(res: Auth) {
    const { accessToken, user } = res;
    this.tokenService.setToken(accessToken);
    this.userSignal.set(user);
  }

  private clearAuthState() {
    this.tokenService.clearTokens();
    this.userSignal.set(null);
  }

  updateUser(user: User) {
    this.userSignal.set(user);
  }
}
