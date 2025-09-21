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
  tournamentFormat: string; // "RoundRobin" or "SingleElimination"
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
