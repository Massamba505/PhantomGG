export class TokenStorage {
  private static readonly ACCESS_TOKEN_KEY = 'access_token';
  private static readonly TOKEN_EXPIRY_KEY = 'token_expiry';
  private static readonly USER_KEY = 'user_data';

  // Store access token
  static setAccessToken(token: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  // Get access token
  static getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  // Store token expiry
  static setTokenExpiry(expiry: Date | string): void {
    const expiryStr =
      typeof expiry === 'string' ? expiry : expiry.toISOString();
    localStorage.setItem(this.TOKEN_EXPIRY_KEY, expiryStr);
  }

  // Get token expiry
  static getTokenExpiry(): Date | null {
    const expiry = localStorage.getItem(this.TOKEN_EXPIRY_KEY);
    return expiry ? new Date(expiry) : null;
  }

  // Check if token is expired
  static isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry();
    if (!expiry) return true;

    // Return true if token is expired (with 10 second buffer)
    return expiry.getTime() <= new Date().getTime() + 10000;
  }

  // Store user data
  static setUserData(user: any): void {
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  // Get user data
  static getUserData(): any | null {
    const userData = localStorage.getItem(this.USER_KEY);
    if (!userData) return null;

    try {
      return JSON.parse(userData);
    } catch (e) {
      console.error('Error parsing user data:', e);
      return null;
    }
  }

  // Clear all auth data
  static clear(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    localStorage.removeItem(this.USER_KEY);
  }
}
