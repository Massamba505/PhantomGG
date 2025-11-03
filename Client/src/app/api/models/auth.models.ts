import { UserRoles } from "./common.models";

export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: UserRoles;
  profilePictureUrl: string;
}

export interface AuthDto {
  accessToken: string;
  user: UserDto;
}

export interface RefreshTokenResponse {
  accessToken: string;
}

export interface LoginRequestDto {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequestDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  profilePictureUrl?: string;
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

export interface ProfilePictureUploadDto {
  profilePictureUrl: string;
}

export interface VerifyEmailRequest {
  token: string;
}

export interface ResendVerificationRequest {
  email: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

export type User = UserDto;
export type Auth = AuthDto;
export type LoginRequest = LoginRequestDto;
export type RegisterRequest = RegisterRequestDto;
export type UpdateUserProfile = UpdateUserProfileRequest;
export type ChangePassword = ChangePasswordRequest;
export type ProfilePictureUpload = ProfilePictureUploadDto;