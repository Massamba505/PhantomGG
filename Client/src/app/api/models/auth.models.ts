import { UserRole } from "./common.models";

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRole;
  profilePictureUrl: string;
}

export interface Auth {
  accessToken: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  profilePictureUrl?: string;
  role: string;
}

export interface UpdateUserProfile {
  firstName?: string;
  lastName?: string;
  email?: string;
  profilePictureUrl?: string;
}

export interface ChangePassword {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ProfilePictureUpload {
  imageUrl: string;
}