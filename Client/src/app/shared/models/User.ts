import { Roles } from "../constants/roles";

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  profilePictureUrl: string;
  role: Roles;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
