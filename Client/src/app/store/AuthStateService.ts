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
    console.log('AuthStateService initialized');
    // Check if we have a token in storage
    const token = TokenStorage.getAccessToken();
    const userData = TokenStorage.getUserData();

    if (userData) {
      // If we have user data in storage, set it immediately
      this.userSignal.set(userData);
      console.log('User restored from storage:', userData);
    }

    if (token) {
      // If token exists but is expired, try to refresh it
      if (TokenStorage.isTokenExpired()) {
        console.log('Token expired, attempting to refresh');
        this.refreshToken();
      } else {
        // If token is valid but we don't have user data, load it
        if (!userData) {
          console.log('Token valid but no user data, loading user');
          this.loadUser();
        }
      }
    } else {
      console.log('No token found, user not authenticated');
    }
  }

  login(credentials: LoginRequest) {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    return this.authService.login(credentials).pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);
        console.log('Login response:', res);

        if (res.success && res.accessToken) {
          // Store the token and its expiry
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }

          // Store user data if available
          if (res.user) {
            this.userSignal.set(res.user);
            TokenStorage.setUserData(res.user);
            console.log('User data stored:', res.user);
          } else {
            // If user is not in the response, load it
            this.loadUser();
          }
        } else {
          this.errorSignal.set(res.message ?? 'Login failed');
        }
      }),
      catchError((error) => {
        console.error('Login error:', error);
        this.loadingSignal.set(false);

        // Extract error message from response if available
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
          // Store the token and its expiry
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }

          // Store user data if available
          if (res.user) {
            this.userSignal.set(res.user);
            TokenStorage.setUserData(res.user);
          } else {
            this.loadUser();
          }
        } else {
          this.errorSignal.set(res.message ?? 'Signup failed');
        }
      }),
      catchError((error) => {
        console.error('Signup error:', error);
        this.loadingSignal.set(false);

        // Extract error message from response if available
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
        console.log('Logout successful');
        // Clear all storage and state
        TokenStorage.clear();
        this.userSignal.set(null);
      },
      error: (err) => {
        console.error('Logout error:', err);
        this.loadingSignal.set(false);
        // Even if server logout fails, clear local state
        TokenStorage.clear();
        this.userSignal.set(null);
      },
    });
  }

  refreshToken() {
    this.loadingSignal.set(true);
    console.log('Refreshing token...');

    return this.authService.refresh().pipe(
      tap((res: any) => {
        this.loadingSignal.set(false);
        console.log('Token refresh response:', res);

        if (res.success && res.accessToken) {
          // Update the token and its expiry
          TokenStorage.setAccessToken(res.accessToken);

          if (res.accessTokenExpires) {
            TokenStorage.setTokenExpiry(res.accessTokenExpires);
          }

          console.log('Token refreshed successfully');
        } else {
          console.warn('Token refresh failed, logging out');
          this.logout();
        }
      }),
      catchError((error) => {
        console.error('Token refresh error:', error);
        this.loadingSignal.set(false);
        this.logout();
        return throwError(() => error);
      })
    );
  }

  loadUser() {
    this.loadingSignal.set(true);
    console.log('Loading user profile...');

    this.authService.getMe().subscribe({
      next: (user: User) => {
        this.loadingSignal.set(false);
        console.log('User profile loaded:', user);

        if (user && user.id) {
          this.userSignal.set(user);
          TokenStorage.setUserData(user);
        } else {
          console.warn('Invalid user data received, logging out');
          this.logout();
        }
      },
      error: (err) => {
        console.error('Failed to load user profile:', err);
        this.loadingSignal.set(false);

        // If unauthorized, refresh token and try again
        if (err.status === 401) {
          this.refreshToken().subscribe({
            next: () => this.loadUser(),
            error: () => this.logout(),
          });
        } else {
          this.logout();
        }
      },
    });
  }
}
