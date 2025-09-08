import { Match } from '../models/match.models';

export class MatchUtils {
  static getMatchResult(match: Match | null | undefined): string {
    if (!match) return 'N/A';
    if (match.homeScore === null || match.homeScore === undefined || 
        match.awayScore === null || match.awayScore === undefined) {
      return 'vs';
    }
    return `${match.homeScore} - ${match.awayScore}`;
  }

  static getWinner(match: Match | null | undefined): string | null {
    if (!match || match.homeScore === null || match.homeScore === undefined || 
        match.awayScore === null || match.awayScore === undefined) {
      return null;
    }
    if (match.homeScore > match.awayScore) return match.homeTeamName;
    if (match.awayScore > match.homeScore) return match.awayTeamName;
    return 'Draw';
  }

  static isDraw(match: Match | null | undefined): boolean {
    if (!match || match.homeScore === null || match.homeScore === undefined || 
        match.awayScore === null || match.awayScore === undefined) {
      return false;
    }
    return match.homeScore === match.awayScore;
  }

  static getStatusColor(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'scheduled': return 'blue';
      case 'inprogress': return 'green';
      case 'completed': return 'gray';
      case 'cancelled': return 'red';
      case 'postponed': return 'orange';
      default: return 'gray';
    }
  }

  static getStatusDisplay(status: string | undefined): string {
    switch (status?.toLowerCase()) {
      case 'inprogress': return 'In Progress';
      case 'scheduled': return 'Scheduled';
      case 'completed': return 'Completed';
      case 'cancelled': return 'Cancelled';
      case 'postponed': return 'Postponed';
      default: return status || 'Unknown';
    }
  }

  static isCompleted(match: Match | null | undefined): boolean {
    return match?.status?.toLowerCase() === 'completed';
  }

  static isInProgress(match: Match | null | undefined): boolean {
    return match?.status?.toLowerCase() === 'inprogress';
  }

  static isScheduled(match: Match | null | undefined): boolean {
    return match?.status?.toLowerCase() === 'scheduled';
  }

  static isUpcoming(match: Match | null | undefined): boolean {
    if (!match) return false;
    const matchDate = new Date(match.matchDate);
    const now = new Date();
    return match.status?.toLowerCase() === 'scheduled' && matchDate > now;
  }

  static getTimeUntilMatch(match: Match | null | undefined): string {
    if (!match) return 'Unknown';
    const matchDate = new Date(match.matchDate);
    const now = new Date();
    const diffMs = matchDate.getTime() - now.getTime();
    
    if (diffMs <= 0) return 'Started';
    
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const diffMinutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (diffDays > 0) return `${diffDays}d ${diffHours}h`;
    if (diffHours > 0) return `${diffHours}h ${diffMinutes}m`;
    return `${diffMinutes}m`;
  }

  static getMatchDateDisplay(match: Match | null | undefined): string {
    if (!match?.matchDate) return 'TBD';
    return new Date(match.matchDate).toLocaleDateString();
  }

  static getMatchTimeDisplay(match: Match | null | undefined): string {
    if (!match?.matchDate) return 'TBD';
    return new Date(match.matchDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  static getVsDisplay(match: Match | null | undefined): string {
    if (!match) return 'TBD vs TBD';
    return `${match.homeTeamName} vs ${match.awayTeamName}`;
  }

  static isHomeTeam(match: Match | null | undefined, teamId: string): boolean {
    return match?.homeTeamId === teamId;
  }

  static isAwayTeam(match: Match | null | undefined, teamId: string): boolean {
    return match?.awayTeamId === teamId;
  }

  static getOpponentName(match: Match | null | undefined, teamId: string): string {
    if (!match) return 'Unknown';
    if (match.homeTeamId === teamId) return match.awayTeamName;
    if (match.awayTeamId === teamId) return match.homeTeamName;
    return 'Unknown';
  }
}
