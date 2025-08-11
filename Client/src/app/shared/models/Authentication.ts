import { User } from './User';

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface SignUpRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  success: boolean;
  message?: string;
  accessToken?: string;
  refreshToken?: string;
  accessTokenExpires?: Date;
  refreshTokenExpires?: Date;
  user?: User;
}
