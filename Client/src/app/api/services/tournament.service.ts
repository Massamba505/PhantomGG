import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PaginatedResponse } from '../models/api.models';
import {
  Tournament,
  CreateTournament,
  UpdateTournament,
  TournamentSearch,
  TournamentStatistics,
  TournamentFormat
} from '../models/tournament.models';

@Injectable({
  providedIn: 'root'
})
export class TournamentService {
  private apiClient = inject(ApiClient);

  // Public tournament discovery and viewing
  getPublicTournaments(params?: TournamentSearch): Observable<PaginatedResponse<Tournament>> {
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.LIST, params);
  }

  getTournament(id: string): Observable<Tournament> {
    return this.apiClient.get<Tournament>(API_ENDPOINTS.TOURNAMENTS.GET(id));
  }

  searchTournaments(params: TournamentSearch): Observable<PaginatedResponse<Tournament>> {
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.SEARCH, params);
  }

  getFeaturedTournaments(): Observable<Tournament[]> {
    return this.apiClient.get<Tournament[]>(API_ENDPOINTS.TOURNAMENTS.FEATURED);
  }

  getUpcomingTournaments(): Observable<Tournament[]> {
    return this.apiClient.get<Tournament[]>(API_ENDPOINTS.TOURNAMENTS.UPCOMING);
  }

  getLiveTournaments(): Observable<Tournament[]> {
    return this.apiClient.get<Tournament[]>(API_ENDPOINTS.TOURNAMENTS.LIVE);
  }

  getRecentTournaments(): Observable<Tournament[]> {
    return this.apiClient.get<Tournament[]>(API_ENDPOINTS.TOURNAMENTS.RECENT);
  }

  getTournamentFormats(): Observable<TournamentFormat[]> {
    return this.apiClient.get<TournamentFormat[]>(API_ENDPOINTS.TOURNAMENTS.FORMATS);
  }

  // Public tournament information
  getTournamentDetails(id: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.TOURNAMENTS.DETAILS(id));
  }

  getTournamentRules(id: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.TOURNAMENTS.RULES(id));
  }

  getTournamentStatistics(id: string): Observable<TournamentStatistics> {
    return this.apiClient.get<TournamentStatistics>(API_ENDPOINTS.TOURNAMENTS.STATISTICS(id));
  }

  getTournamentTeams(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAMS(id));
  }

  getTournamentStandings(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.STANDINGS(id));
  }

  getTournamentSchedule(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.SCHEDULE(id));
  }

  getTournamentMatches(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.MATCHES(id));
  }

  getTournamentResults(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.RESULTS(id));
  }

  getLiveMatches(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.LIVE_MATCHES(id));
  }

  getUpcomingMatches(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.UPCOMING_MATCHES(id));
  }

  getCompletedMatches(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.COMPLETED_MATCHES(id));
  }

  // Tournament statistics
  getTopScorers(id: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TOP_SCORERS(id) + `?limit=${limit}`);
  }

  getTopAssists(id: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TOP_ASSISTS(id) + `?limit=${limit}`);
  }

  getBestPlayers(id: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.BEST_PLAYERS(id) + `?limit=${limit}`);
  }

  getTeamStats(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAM_STATS(id));
  }

  // Organizer-specific methods for managing tournaments
  createTournament(tournament: any): Observable<any> {
    return this.apiClient.post<any>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.CREATE, tournament);
  }

  updateTournament(id: string, updates: any): Observable<any> {
    return this.apiClient.put<any>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPDATE(id), updates);
  }

  deleteTournament(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.DELETE(id));
  }

  uploadTournamentBanner(tournamentId: string, file: File): Observable<{ bannerUrl: string }> {
    return this.apiClient.uploadFile<{ bannerUrl: string }>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPLOAD_BANNER(tournamentId), file);
  }

  uploadTournamentLogo(tournamentId: string, file: File): Observable<{ logoUrl: string }> {
    return this.apiClient.uploadFile<{ logoUrl: string }>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPLOAD_LOGO(tournamentId), file);
  }

  uploadOrganizerTournamentBanner(tournamentId: string, file: File): Observable<{ bannerUrl: string }> {
    return this.apiClient.uploadFile<{ bannerUrl: string }>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPLOAD_BANNER(tournamentId), file);
  }

  // User-specific methods for tournament participation
  getMyTournaments(): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.USERS.TOURNAMENTS.CURRENT);
  }
}
