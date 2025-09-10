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
  matchId: string;
  homeScore: number;
  awayScore: number;
}

export interface AutoGenerateFixtures {
  tournamentId: string;
  startDate: string;
  matchDurationMinutes: number;
  timeBetweenMatches: number;
  venue?: string;
}

export interface GenerateFixtures {
  tournamentId: string;
  fixtures: {
    homeTeamId: string;
    awayTeamId: string;
    matchDate: string;
    venue?: string;
  }[];
}
