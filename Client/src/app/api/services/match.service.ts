import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { 
  CreateMatch,
  UpdateMatch,
  Match 
} from '../models/match.models';
import { ApiResponse, PaginatedResponse } from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  constructor(private apiClient: ApiClient) {}

  // Public match viewing in tournament context
  getTournamentMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.TOURNAMENTS.MATCHES(tournamentId));
  }

  getTournamentMatchDetails(tournamentId: string, matchId: string): Observable<Match> {
    return this.apiClient.get<Match>(API_ENDPOINTS.TOURNAMENTS.MATCH_DETAILS(tournamentId, matchId));
  }

  getLiveMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.TOURNAMENTS.LIVE_MATCHES(tournamentId));
  }

  getUpcomingMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.TOURNAMENTS.UPCOMING_MATCHES(tournamentId));
  }

  getCompletedMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.TOURNAMENTS.COMPLETED_MATCHES(tournamentId));
  }

  getTournamentResults(tournamentId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.RESULTS(tournamentId));
  }

  getTournamentSchedule(tournamentId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.SCHEDULE(tournamentId));
  }

  getTeamMatches(tournamentId: string, teamId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.TOURNAMENTS.TEAM_MATCHES(tournamentId, teamId));
  }

  // User team match management
  getMyTeamMatches(teamId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.USERS.TEAMS.MATCHES(teamId));
  }

  getMyTeamUpcomingMatches(teamId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.USERS.TEAMS.UPCOMING_MATCHES(teamId));
  }

  getMyTeamMatchHistory(teamId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.USERS.TEAMS.MATCH_HISTORY(teamId));
  }
}
