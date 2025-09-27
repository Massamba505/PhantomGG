import { Injectable, inject } from '@angular/core';
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
  private apiClient = inject(ApiClient);

  // Note: Match-specific endpoints are not yet implemented in the backend controllers
  // This service is prepared for future match management features
  
  // Tournament-related operations using available endpoints
  getTournamentTeams(tournamentId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAMS(tournamentId));
  }

  // Placeholder methods for future match functionality
  // These will need to be implemented when match endpoints are added to the backend

  /*
  // Future match endpoints when implemented:
  
  getTournamentMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(`tournaments/${tournamentId}/matches`);
  }

  getMatch(tournamentId: string, matchId: string): Observable<Match> {
    return this.apiClient.get<Match>(`tournaments/${tournamentId}/matches/${matchId}`);
  }

  createMatch(tournamentId: string, matchData: CreateMatch): Observable<Match> {
    return this.apiClient.post<Match>(`tournaments/${tournamentId}/matches`, matchData);
  }

  updateMatch(tournamentId: string, matchId: string, updates: UpdateMatch): Observable<Match> {
    return this.apiClient.put<Match>(`tournaments/${tournamentId}/matches/${matchId}`, updates);
  }

  deleteMatch(tournamentId: string, matchId: string): Observable<void> {
    return this.apiClient.delete<void>(`tournaments/${tournamentId}/matches/${matchId}`);
  }

  updateMatchResult(tournamentId: string, matchId: string, result: any): Observable<Match> {
    return this.apiClient.put<Match>(`tournaments/${tournamentId}/matches/${matchId}/result`, result);
  }
  */

  // Legacy methods that return empty data until match endpoints are implemented
  getTournamentMatches(tournamentId: string): Observable<Match[]> {
    // Return empty array until match endpoints are implemented
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }

  getLiveMatches(tournamentId: string): Observable<Match[]> {
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }

  getUpcomingMatches(tournamentId: string): Observable<Match[]> {
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }

  getCompletedMatches(tournamentId: string): Observable<Match[]> {
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }
}
