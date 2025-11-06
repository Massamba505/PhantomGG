import { Tournament } from '@/app/api/models';

export class TournamentUtils {
  static getDisplayName(tournament: Tournament | null | undefined): string {
    return tournament?.name || 'Unknown Tournament';
  }

  static getStatusDisplay(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'registrationopen':
        return 'Registration Open';
      case 'registrationclosed':
        return 'Registration Closed';
      case 'inprogress':
        return 'In Progress';
      default:
        return status || 'Unknown';
    }
  }

  static getDaysUntilStart(tournament: Tournament | null | undefined): number {
    if (!tournament) return 0;
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    const diffTime = startDate.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
