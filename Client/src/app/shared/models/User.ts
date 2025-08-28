import { Roles } from "../constants/roles";

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  profilePictureUrl: string;
  role: Roles;
}
