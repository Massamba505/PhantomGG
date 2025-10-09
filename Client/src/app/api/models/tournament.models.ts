import { UserDto } from './auth.models';
import { TournamentFormats, TournamentStatus, TeamAction } from './common.models';

export interface TournamentDto {
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
  status: TournamentStatus;
  organizerId: string;
  organizer?: UserDto;
  createdAt: string;
  updatedAt?: string;
  isPublic: boolean;
  teamCount: number;
  pendingTeamCount: number;
  matchCount: number;
}

export interface TournamentQuery {
  q?: string;
  searchTerm?: string;
  status?: string;
  location?: string;
  startFrom?: string;
  startTo?: string;
  page: number;
  pageNumber?: number;
  pageSize: number;
  isPublic?: boolean;
  scope?: string;
}

export interface CreateTournamentDto {
  name: string;
  description: string;
  location?: string;
  registrationStartDate: string;
  registrationDeadline: string;
  startDate: string;
  endDate: string;
  minTeams: number;
  maxTeams: number;
  bannerUrl?: File;
  logoUrl?: File;
  isPublic: boolean;
}

export interface UpdateTournamentDto {
  name?: string;
  description?: string;
  location?: string;
  registrationStartDate?: string;
  registrationDeadline?: string;
  startDate?: string;
  endDate?: string;
  minTeams?: number;
  maxTeams?: number;
  bannerUrl?: File;
  logoUrl?: File;
  isPublic?: boolean;
}

export interface TeamRegistrationRequest {
  teamId: string;
}

export interface TournamentGenerateFixturesRequest {
  format: TournamentFormats;
}

export interface FixtureStatusResponse {
  status: string;
}

export interface TeamManagementRequest {
  action: TeamAction;
}

export interface TournamentStandingDto {
  id: string;
  tournamentId: string;
  tournamentName: string;
  teamId: string;
  teamName: string;
  teamLogo?: string;
  matchesPlayed: number;
  wins: number;
  draws: number;
  losses: number;
  goalsFor: number;
  goalsAgainst: number;
  goalDifference: number;
  points: number;
  position?: number;
}

export interface PlayerGoalStandingDto {
  playerId: string;
  playerName: string;
  teamId: string;
  teamName: string;
  teamLogo?: string;
  playerPhoto?: string;
  goals: number;
  matchesPlayed: number;
  position?: number;
}

export interface PlayerAssistStandingDto {
  playerId: string;
  playerName: string;
  teamId: string;
  teamName: string;
  teamLogo?: string;
  playerPhoto?: string;
  assists: number;
  matchesPlayed: number;
  assistsPerMatch: number;
  position?: number;
}

export type Tournament = TournamentDto;
export type CreateTournament = CreateTournamentDto;
export type UpdateTournament = UpdateTournamentDto;
export type TournamentSearch = TournamentQuery;

export interface TournamentFormat {
  id: string;
  name: string;
  description: string;
}

export function getTournamentFormats(): TournamentFormat[] {
  return [
    {
      id: TournamentFormats.RoundRobin.toString(),
      name: 'Round Robin',
      description: 'Every team plays every other team once'
    },
    {
      id: TournamentFormats.SingleElimination.toString(), 
      name: 'Single Elimination',
      description: 'Teams are eliminated after losing one match'
    }
  ];
}
