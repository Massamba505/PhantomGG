import { User, CurrentUser } from '../models/auth.models';

export class UserUtils {
  static getFullName(user: User | CurrentUser | null | undefined): string {
    if (!user) return 'Unknown User';
    if ('firstName' in user && 'lastName' in user) {
      return `${user.firstName} ${user.lastName}`.trim();
    }
    return user.email || 'Unknown User';
  }

  static getInitials(user: User | null | undefined): string {
    if (!user || !('firstName' in user) || !('lastName' in user)) return 'U';
    const firstInitial = user.firstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = user.lastName?.charAt(0)?.toUpperCase() || '';
    return `${firstInitial}${lastInitial}` || 'U';
  }

  static hasProfilePicture(user: User | null | undefined): boolean {
    return !!(user?.profilePictureUrl && user.profilePictureUrl.trim() !== '');
  }

  static getDisplayEmail(user: User | CurrentUser | null | undefined): string {
    return user?.email || 'No email';
  }

  static hasRole(user: User | CurrentUser | null | undefined, role: string): boolean {
    return user?.role?.toLowerCase() === role.toLowerCase();
  }

  static isAdmin(user: User | CurrentUser | null | undefined): boolean {
    return this.hasRole(user, 'Admin');
  }

  static isOrganizer(user: User | CurrentUser | null | undefined): boolean {
    return this.hasRole(user, 'Organizer');
  }

  static isManager(user: User | CurrentUser | null | undefined): boolean {
    return this.hasRole(user, 'Manager');
  }

  static isPlayer(user: User | CurrentUser | null | undefined): boolean {
    return this.hasRole(user, 'Player');
  }
}
