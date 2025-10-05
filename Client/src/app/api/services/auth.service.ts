import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import {
  Auth,
  LoginRequest,
  RegisterRequest,
  User,
  VerifyEmailRequest,
  ResendVerificationRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiClient = inject(ApiClient);

  register(request: RegisterRequest): Observable<Auth> {
    return this.apiClient.post<Auth>(API_ENDPOINTS.AUTH.REGISTER, request);
  }

  login(request: LoginRequest): Observable<Auth> {
    return this.apiClient.post<Auth>(API_ENDPOINTS.AUTH.LOGIN, request);
  }

  refreshToken(): Observable<{ accessToken: string }> {
    return this.apiClient.post<{ accessToken: string }>(API_ENDPOINTS.AUTH.REFRESH, {});
  }

  getCurrentUser(): Observable<User> {
    return this.apiClient.get<User>(API_ENDPOINTS.AUTH.ME);
  }

  logout(): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.AUTH.LOGOUT, {});
  }

  verifyEmail(request: VerifyEmailRequest): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.AUTH.VERIFY_EMAIL, request);
  }

  resendVerification(request: ResendVerificationRequest): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.AUTH.RESEND_VERIFICATION, request);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.AUTH.RESET_PASSWORD, request);
  }
}
