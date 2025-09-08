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
  matchDate: Date;
  venue?: string;
  status: string;
  homeScore?: number;
  awayScore?: number;
}

export interface CreateMatch {
  tournamentId: string;
  homeTeamId: string;
  awayTeamId: string;
  matchDate: Date;
  venue?: string;
}

export interface UpdateMatch {
  matchDate: Date;
  venue?: string;
  status: string;
  homeScore?: number;
  awayScore?: number;
}

export interface MatchSearch {
  searchTerm?: string;
  tournamentId?: string;
  teamId?: string;
  status?: string;
  venue?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface MatchResult {
  matchId: string;
  homeScore: number;
  awayScore: number;
  winnerTeamId?: string;
  isDraw: boolean;
}

export interface GenerateFixtures {
  tournamentId: string;
  startDate: Date;
  matchDuration: number;
  daysBetweenRounds: number;
  venues?: string[];
}

export interface AutoGenerateFixtures {
  tournamentId: string;
}
