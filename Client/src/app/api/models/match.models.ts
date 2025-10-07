export interface Match {
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
  status: string;
  homeScore?: number;
  awayScore?: number;
}

export interface CreateMatch {
  tournamentId: string;
  homeTeamId: string;
  awayTeamId: string;
  matchDate: string;
  venue?: string;
}

export interface UpdateMatch {
  homeTeamId?: string;
  awayTeamId?: string;
  matchDate?: string;
  venue?: string;
}

export interface MatchSearch {
  searchTerm?: string;
  tournamentId?: string;
  teamId?: string;
  status?: string;
  matchDateFrom?: string;
  matchDateTo?: string;
  venue?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface MatchResult {
  homeScore: number;
  awayScore: number;
}

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

export enum MatchEventType {
  Goal = 'Goal',
  Assist = 'Assist',
  YellowCard = 'YellowCard',
  RedCard = 'RedCard',
  Foul = 'Foul',
  Substitution = 'Substitution'
}

export interface MatchEvent {
  id: string;
  matchId: string;
  eventType: MatchEventType;
  minute: number;
  teamId: string;
  teamName?: string;
  playerId: string;
  playerName?: string;
  description?: string;
  createdAt: string;
}

export interface CreateMatchEvent {
  matchId: string;
  eventType: MatchEventType;
  minute: number;
  teamId: string;
  playerId: string;
}

export interface UpdateMatchEvent {
  eventType?: MatchEventType;
  minute?: number;
  playerId?: string;
  description?: string;
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
