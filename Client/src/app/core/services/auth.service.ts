import { ApiResponse } from '@/app/shared/models/ApiResponse';
import {
  AuthResponse,
  LoginRequest,
  SignUpRequest,
} from '@/app/shared/models/Authentication';
import { User } from '@/app/shared/models/User';
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

  login(credentials: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(
      `${this.env.apiUrl}/auth/login`,
      credentials
    );
  }

  signup(credentials: SignUpRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(
      `${this.env.apiUrl}/auth/register`,
      credentials
    );
  }

  logout(): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<any>(`${this.env.apiUrl}/auth/logout`, {});
  }

  refresh(): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.env.apiUrl}/auth/refresh`, {});
  }

  getMe(): Observable<ApiResponse<User>> {
    return this.http.get<ApiResponse<User>>(`${this.env.apiUrl}/auth/me`);
  }
}
