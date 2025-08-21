import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@/environments/environment.development';
import { AuthResponse } from '@/app/shared/models/Authentication';
import { TokenStorage } from '@/app/shared/utils/tokenStorage';
import { ApiResponse } from '@/app/shared/models/ApiResponse';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshService {
  private readonly env = environment;

  constructor(private http: HttpClient) {}

  refreshToken(): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.env.apiUrl}/auth/refresh`, {});
  }

  getToken(): string | null {
    return TokenStorage.getAccessToken();
  }

  isTokenExpired(): boolean {
    return TokenStorage.isTokenExpired();
  }

  setToken(token: string): void {
    TokenStorage.setAccessToken(token);
  }

  setTokenExpiry(expiry: Date | string): void {
    TokenStorage.setTokenExpiry(expiry);
  }

  clearTokens(): void {
    TokenStorage.clear();
  }
}
