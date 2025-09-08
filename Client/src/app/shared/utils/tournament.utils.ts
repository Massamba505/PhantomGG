import { Tournament } from '../models/tournament.models';

export class TournamentUtils {
  static getDisplayName(tournament: Tournament | null | undefined): string {
    return tournament?.name || 'Unknown Tournament';
  }

  static getStatusColor(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'draft': return 'gray';
      case 'registrationopen': return 'blue';
      case 'registrationclosed': return 'orange';
      case 'inprogress': return 'green';
      case 'completed': return 'purple';
      case 'cancelled': return 'red';
      case 'postponed': return 'yellow';
      default: return 'gray';
    }
  }

  static getStatusDisplay(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'registrationopen': return 'Registration Open';
      case 'registrationclosed': return 'Registration Closed';
      case 'inprogress': return 'In Progress';
      default: return status || 'Unknown';
    }
  }

  static isRegistrationOpen(tournament: Tournament | null | undefined): boolean {
    if (!tournament) return false;
    return tournament.status?.toLowerCase() === 'registrationopen';
  }

  static hasSpots(tournament: Tournament | null | undefined): boolean {
    if (!tournament) return false;
    return tournament.teamCount < tournament.maxTeams;
  }

  static getProgressPercentage(tournament: Tournament | null | undefined): number {
    if (!tournament || tournament.matchCount === 0) return 0;
    return Math.round((tournament.completedMatches / tournament.matchCount) * 100);
  }

  static formatPrize(amount: number | null | undefined): string {
    if (!amount || amount === 0) return 'No prize';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  static formatEntryFee(amount: number | null | undefined): string {
    if (!amount || amount === 0) return 'Free';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  static isCompleted(tournament: Tournament | null | undefined): boolean {
    return tournament?.status?.toLowerCase() === 'completed';
  }

  static isInProgress(tournament: Tournament | null | undefined): boolean {
    return tournament?.status?.toLowerCase() === 'inprogress';
  }

  static isUpcoming(tournament: Tournament | null | undefined): boolean {
    if (!tournament) return false;
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    return startDate > now && (tournament.status?.toLowerCase() === 'registrationopen' || tournament.status?.toLowerCase() === 'registrationclosed');
  }

  static getDaysUntilStart(tournament: Tournament | null | undefined): number {
    if (!tournament) return 0;
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    const diffTime = startDate.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
