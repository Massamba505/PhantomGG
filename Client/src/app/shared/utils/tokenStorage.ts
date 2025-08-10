export class TokenStorage {
  private static readonly ACCESS_TOKEN_KEY = 'access_token';

  static setAccessToken(token: string) {
    localStorage.setItem(this.ACCESS_TOKEN_KEY, token);
  }

  static getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  static clear() {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
  }
}
