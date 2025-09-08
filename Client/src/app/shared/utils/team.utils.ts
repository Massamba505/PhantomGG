import { Team } from '../models/team.models';

export class TeamUtils {
  static getDisplayName(team: Team | null | undefined): string {
    if (!team) return 'Unknown Team';
    return team.shortName || team.name || 'Unnamed Team';
  }

  static getFullName(team: Team | null | undefined): string {
    if (!team) return 'Unknown Team';
    return team.name || 'Unnamed Team';
  }

  static hasLogo(team: Team | null | undefined): boolean {
    return !!(team?.logoUrl && team.logoUrl.trim() !== '');
  }

  static hasPhoto(team: Team | null | undefined): boolean {
    return !!(team?.teamPhotoUrl && team.teamPhotoUrl.trim() !== '');
  }

  static getPlayerCountDisplay(team: Team | null | undefined): string {
    if (!team) return '0 players';
    const count = team.numberOfPlayers || 0;
    return `${count} player${count === 1 ? '' : 's'}`;
  }

  static getRegistrationStatusColor(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'approved': return 'green';
      case 'pending': return 'orange';
      case 'rejected': return 'red';
      case 'withdrawn': return 'gray';
      default: return 'gray';
    }
  }

  static getRegistrationStatusDisplay(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'approved': return 'Approved';
      case 'pending': return 'Pending';
      case 'rejected': return 'Rejected';
      case 'withdrawn': return 'Withdrawn';
      default: return status || 'Unknown';
    }
  }

  static isApproved(team: Team | null | undefined): boolean {
    return team?.registrationStatus?.toLowerCase() === 'approved';
  }

  static isPending(team: Team | null | undefined): boolean {
    return team?.registrationStatus?.toLowerCase() === 'pending';
  }

  static isRejected(team: Team | null | undefined): boolean {
    return team?.registrationStatus?.toLowerCase() === 'rejected';
  }

  static isActive(team: Team | null | undefined): boolean {
    return team?.isActive === true;
  }

  static getManagerContact(team: Team | null | undefined): string {
    if (!team) return 'No contact';
    if (team.managerPhone) {
      return `${team.managerEmail} | ${team.managerPhone}`;
    }
    return team.managerEmail || 'No contact';
  }

  static getRegistrationDateDisplay(team: Team | null | undefined): string {
    if (!team?.registrationDate) return 'Unknown';
    return new Date(team.registrationDate).toLocaleDateString();
  }
}
