export interface Tournament {
  id: string;
  name: string;
  description: string;
  location?: string;
  formatId: string;
  formatName: string;
  registrationStartDate?: Date;
  registrationDeadline?: Date;
  startDate: Date;
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
  createdAt: Date;
  updatedAt?: Date;
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
  registrationStartDate?: Date;
  registrationDeadline?: Date;
  startDate: Date;
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
  name: string;
  description: string;
  location?: string;
  formatId: string;
  registrationStartDate?: Date;
  registrationDeadline?: Date;
  startDate: Date;
  minTeams: number;
  maxTeams: number;
  maxPlayersPerTeam: number;
  minPlayersPerTeam: number;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
  bannerUrl?: string;
  logoUrl?: string;
  status?: string;
  matchDuration: number;
  isPublic: boolean;
}

export interface TournamentSearch {
  searchTerm?: string;
  status?: string;
  organizerId?: string;
  formatId?: string;
  location?: string;
  pageNumber?: number;
  pageSize?: number;
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

export interface TournamentStatistics {
  totalTournaments: number;
  activeTournaments: number;
  completedTournaments: number;
  totalTeams: number;
  totalMatches: number;
  totalPrizePool: number;
  averageTeamsPerTournament: number;
  mostPopularFormat: string;
}
