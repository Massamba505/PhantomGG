export class TokenStorage {
  private static readonly ACCESS_TOKEN_KEY = 'access_token';
  private static readonly TOKEN_EXPIRY_KEY = 'token_expiry';

  static setAccessToken(token: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  static getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  static setTokenExpiry(expiry: Date | string): void {
    const expiryStr =
      typeof expiry === 'string' ? expiry : expiry.toISOString();
    localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiryStr);
  }

  static getTokenExpiry(): Date | null {
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    return expiry ? new Date(expiry) : null;
  }

  static isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry();
    if (!expiry) return true;

    return expiry.getTime() <= new Date().getTime() + 30000;
  }

  static clear(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
  }
}
