import {
  AuthResponse,
  LoginRequest,
  SignUpRequest,
} from '@/app/shared/models/Authentication';
import { environment } from '@/environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly env = environment;

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.env.apiUrl}/auth/login`,
      credentials
    );
  }

  signup(credentials: SignUpRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.env.apiUrl}/auth/register`,
      credentials
    );
  }

  logout(): Observable<any> {
    return this.http.post<any>(`${this.env.apiUrl}/auth/logout`, {});
  }

  refresh(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.env.apiUrl}/auth/refresh`, {});
  }

  getMe(): Observable<AuthResponse> {
    return this.http.get<AuthResponse>(`${this.env.apiUrl}/auth/me`);
  }
}
