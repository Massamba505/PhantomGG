import { User } from './auth.models';

export interface Tournament {
  id: string;
  name: string;
  description: string;
  location?: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate: string;
  endDate: string;
  minTeams: number;
  maxTeams: number;
  bannerUrl?: string;
  logoUrl?: string;
  status: string;
  organizerId: string;
  organizer?: User;
  createdAt: string;
  updatedAt?: string;
  isPublic: boolean;
  teamCount: number;
  pendingTeamCount: number;
  matchCount: number;
}

export interface CreateTournament {
  name: string;
  description: string;
  location?: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate: string;
  endDate: string;
  minTeams: number;
  maxTeams: number;
  contactEmail?: string;
  bannerUrl?: File;
  logoUrl?: File;
  isPublic: boolean;
}

export interface UpdateTournament {
  name?: string;
  description?: string;
  location?: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate?: string;
  endDate?: string;
  minTeams?: number;
  maxTeams?: number;
  contactEmail?: string;
  bannerUrl?: File;
  logoUrl?: File;
  isPublic?: boolean;
}

export interface TournamentSearch {
  searchTerm?: string;
  status?: string;
  location?: string;
  startDateFrom?: string;
  startDateTo?: string;
  isPublic?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

export interface JoinTournament {
  tournamentId: string;
  teamId: string;
}

export interface TournamentFormat {
  id: string;
  name: string;
  description: string;
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

export function getTournamentFormats(): TournamentFormat[] {
  return [
    {
      id: '1',
      name: 'Round Robin',
      description: 'Every team plays every other team once'
    },
    {
      id: '2', 
      name: 'Single Elimination',
      description: 'Teams are eliminated after losing one match'
    }
  ];
}
