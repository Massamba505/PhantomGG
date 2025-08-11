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

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.env.apiUrl}/auth/login`,
      credentials,
      { withCredentials: true } // This is important for receiving and sending cookies
    );
  }

  signup(credentials: SignUpRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.env.apiUrl}/auth/register`,
      credentials,
      { withCredentials: true }
    );
  }

  logout(): Observable<any> {
    return this.http.post<any>(
      `${this.env.apiUrl}/auth/logout`,
      {},
      { withCredentials: true }
    );
  }

  refresh(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this.env.apiUrl}/auth/refresh`,
      {},
      { withCredentials: true }
    );
  }

  getMe(): Observable<User> {
    return this.http.get<User>(`${this.env.apiUrl}/auth/me`, {
      withCredentials: true,
    });
  }
}
