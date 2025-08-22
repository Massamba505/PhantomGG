import { Injectable } from '@angular/core';
import { TokenStorage } from '@/app/shared/utils/tokenStorage';

@Injectable({
  providedIn: 'root',
})
export class TokenRefreshService {
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
