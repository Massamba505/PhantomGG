export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  profilePictureUrl?: string;
  role: string;
}

export interface Auth {
  accessToken: string;
  user: User;
}

export interface AccessToken {
  token: string;
  expiresAt: Date;
}

export interface RefreshToken {
  token: string;
  expiresAt: Date;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  profilePictureUrl: string;
}

export interface CurrentUser {
  id: string;
  email: string;
  role: string;
}

export interface UpdateUserProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface ProfilePictureUpload {
  profilePictureUrl: string;
}
