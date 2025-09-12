export interface Tournament {
  id: string;
  name: string;
  description: string;
  location?: string;
  formatId: string;
  formatName: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate: string;
  minTeams: number;
  maxTeams: number;
  maxPlayersPerTeam: number;
  minPlayersPerTeam: number;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
  bannerUrl?: string;
  logoUrl?: string;
  status: string;
  matchDuration: number;
  organizerId: string;
  organizerName: string;
  createdAt: string;
  updatedAt?: string;
  isActive: boolean;
  isPublic: boolean;
  teamCount: number;
  matchCount: number;
  completedMatches: number;
}

export interface CreateTournament {
  name: string;
  description: string;
  location?: string;
  formatId: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate: string;
  minTeams: number;
  maxTeams: number;
  maxPlayersPerTeam: number;
  minPlayersPerTeam: number;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
  bannerUrl?: string;
  logoUrl?: string;
  matchDuration: number;
  isPublic: boolean;
}

export interface UpdateTournament {
  name?: string;
  description?: string;
  location?: string;
  formatId?: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate?: string;
  minTeams?: number;
  maxTeams?: number;
  maxPlayersPerTeam?: number;
  minPlayersPerTeam?: number;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
  bannerUrl?: string;
  logoUrl?: string;
  matchDuration?: number;
  isPublic?: boolean;
}

export interface TournamentSearch {
  searchTerm?: string;
  status?: string;
  location?: string;
  formatId?: string;
  minPrizePool?: number;
  maxPrizePool?: number;
  startDateFrom?: string;
  startDateTo?: string;
  isPublic?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

export interface TournamentStatistics {
  tournamentId: string;
  totalTeams: number;
  totalMatches: number;
  completedMatches: number;
  totalGoals: number;
  averageGoalsPerMatch: number;
  topScorer?: {
    playerId: string;
    playerName: string;
    goals: number;
  };
  mostAssists?: {
    playerId: string;
    playerName: string;
    assists: number;
  };
}

export interface JoinTournament {
  tournamentId: string;
  teamId: string;
}

export interface LeaveTournament {
  tournamentId: string;
  teamId: string;
  reason?: string;
}

export interface TournamentFormat {
  id: string;
  name: string;
  description: string;
}
