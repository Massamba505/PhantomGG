export class TokenStorage {
  private static readonly ACCESS_TOKEN_KEY = 'access_token';

  static setAccessToken(token: string): void {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  static getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  static clear(): void {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
  }

  private static decodePayload(token: string){
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch {
      return null;
    }
  }
  
  static getTokenExpiry(): Date | null {
    const token = this.getAccessToken();
    if (!token) return null;

    const payload = this.decodePayload(token);
    if (!payload?.exp) return null;
    
    return new Date(payload.exp * 1000);
  }

  static isTokenExpired(): boolean {
    const expiry = this.getTokenExpiry();
    if (!expiry) return true;

    return expiry.getTime() <= Date.now();
  }
}
