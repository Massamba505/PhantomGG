import { MatchEventType, MatchStatus } from './common.models';

export interface MatchDto {
  id: string;
  tournamentId: string;
  tournamentName: string;
  homeTeamId: string;
  homeTeamName: string;
  homeTeamLogo?: string;
  awayTeamId: string;
  awayTeamName: string;
  awayTeamLogo?: string;
  matchDate: string;
  venue?: string;
  status: MatchStatus;
  homeScore?: number;
  awayScore?: number;
}

export interface MatchQuery {
  q?: string;
  tournamentId?: string;
  teamId?: string;
  status?: string;
  from?: string;
  to?: string;
  page: number;
  pageSize: number;
}

export interface CreateMatchDto {
  tournamentId: string;
  homeTeamId: string;
  awayTeamId: string;
  matchDate: string;
  venue?: string;
}

export interface UpdateMatchDto {
  homeTeamId?: string;
  awayTeamId?: string;
  matchDate?: string;
  venue?: string;
  status?: string;
}

export interface MatchResultDto {
  homeScore: number;
  awayScore: number;
  status?: MatchStatus;
}

export interface MatchEventDto {
  id: string;
  matchId: string;
  eventType: string;
  minute: number;
  teamId: string;
  teamName: string;
  playerId: string;
  playerName: string;
}

export interface CreateMatchEventDto {
  matchId: string;
  eventType: MatchEventType;
  minute: number;
  teamId: string;
  playerId: string;
  description?: string;
}

export interface UpdateMatchEventDto {
  eventType?: MatchEventType;
  minute?: number;
  playerId?: string;
}

export type Match = MatchDto;
export type CreateMatch = CreateMatchDto;
export type UpdateMatch = UpdateMatchDto;
export type MatchSearch = MatchQuery;
export type MatchResult = MatchResultDto;
export type MatchEvent = MatchEventDto;
export type CreateMatchEvent = CreateMatchEventDto;
export type UpdateMatchEvent = UpdateMatchEventDto;

export { MatchEventType } from './common.models';

export interface AutoGenerateFixtures {
  tournamentId: string;
  tournamentFormat: string;
  startDate: string;
  daysBetweenRounds?: number;
  defaultVenue?: string;
  includeReturnMatches?: boolean;
  autoAdvanceTeams?: boolean;
  timeOfDay?: string;
}

export interface GenerateFixtures {
  tournamentId: string;
  startDate: string;
  daysBetweenMatches?: number;
  defaultVenue?: string;
  includeReturnMatches?: boolean;
}

export interface FixtureGenerationStatus {
  tournamentId: string;
  tournamentName: string;
  fixturesGeneratedAt?: string;
  fixturesGeneratedBy?: string;
  registeredTeams: number;
  requiredTeams: number;
  maxTeams: number;
  status: string;
  canGenerateFixtures: boolean;
  message?: string;
}

export interface PlayerEventsSummary {
  playerId: string;
  playerName: string;
  teamId: string;
  teamName: string;
  goals: number;
  assists: number;
  yellowCards: number;
  redCards: number;
  fouls: number;
  totalEvents: number;
}

export interface TeamEventsSummary {
  teamId: string;
  teamName: string;
  totalGoals: number;
  totalAssists: number;
  totalYellowCards: number;
  totalRedCards: number;
  totalFouls: number;
  totalSubstitutions: number;
  totalEvents: number;
  topScorer?: {
    playerId: string;
    playerName: string;
    goals: number;
  };
}
