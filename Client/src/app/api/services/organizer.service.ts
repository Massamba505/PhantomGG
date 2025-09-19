import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import {
  Tournament,
  CreateTournament,
  UpdateTournament,
  TournamentSearch
} from '../models/tournament.models';
import {
  Team,
  UpdateTeam,
  TeamSearch
} from '../models/team.models';
import {
  Player,
  CreatePlayer,
  UpdatePlayer,
  PlayerSearch
} from '../models/player.models';
import { PaginatedResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class OrganizerService {
  private apiClient = inject(ApiClient);

  // Tournament Management
  getMyTournaments(): Observable<Tournament[]> {
    return this.apiClient.get<Tournament[]>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.LIST);
  }

  searchTournaments(params: TournamentSearch): Observable<PaginatedResponse<Tournament>> {
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.LIST, params);
  }

  createTournament(tournament: CreateTournament): Observable<Tournament> {
    return this.apiClient.post<Tournament>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.CREATE, tournament);
  }

  updateTournament(id: string, updates: UpdateTournament): Observable<Tournament> {
    return this.apiClient.put<Tournament>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPDATE(id), updates);
  }

  deleteTournament(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.DELETE(id));
  }

  getTournamentDetails(id: string): Observable<Tournament> {
    return this.apiClient.get<Tournament>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.GET(id));
  }

  uploadTournamentBanner(tournamentId: string, file: File): Observable<{ bannerUrl: string }> {
    return this.apiClient.uploadFile<{ bannerUrl: string }>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPLOAD_BANNER(tournamentId), file);
  }

  uploadTournamentLogo(tournamentId: string, file: File): Observable<{ logoUrl: string }> {
    return this.apiClient.uploadFile<{ logoUrl: string }>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.UPLOAD_LOGO(tournamentId), file);
  }

  // Team Management
  getTournamentTeams(tournamentId: string): Observable<Team[]> {
    return this.apiClient.get<Team[]>(API_ENDPOINTS.ORGANIZERS.TEAMS.LIST(tournamentId));
  }

  getTeamDetails(tournamentId: string, teamId: string): Observable<Team> {
    return this.apiClient.get<Team>(API_ENDPOINTS.ORGANIZERS.TEAMS.GET(tournamentId, teamId));
  }

  approveTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.ORGANIZERS.TEAMS.APPROVE(tournamentId, teamId), {});
  }

  rejectTeam(tournamentId: string, teamId: string, reason?: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.ORGANIZERS.TEAMS.REJECT(tournamentId, teamId), { reason });
  }

  getTeamsByStatus(tournamentId: string, status: string): Observable<Team[]> {
    return this.apiClient.get<Team[]>(API_ENDPOINTS.ORGANIZERS.TEAMS.BY_STATUS(tournamentId, status));
  }

  removeTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.ORGANIZERS.TEAMS.REMOVE(tournamentId, teamId));
  }

  // Player Management
  getTournamentPlayers(tournamentId: string): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.ORGANIZERS.PLAYERS.LIST(tournamentId));
  }

  getTeamPlayers(tournamentId: string, teamId: string): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.ORGANIZERS.PLAYERS.BY_TEAM(tournamentId, teamId));
  }

  // Statistics & Analytics
  getTournamentStatistics(tournamentId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.STATISTICS(tournamentId));
  }

  getTopScorers(tournamentId: string, limit: number = 10): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.ORGANIZERS.PLAYERS.TOP_SCORERS(tournamentId) + `?limit=${limit}`);
  }

  getTopAssists(tournamentId: string, limit: number = 10): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.ORGANIZERS.PLAYERS.TOP_ASSISTS(tournamentId) + `?limit=${limit}`);
  }

  // Dashboard Data - Comprehensive tournament overview
  getDashboardData(tournamentId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.ORGANIZERS.TOURNAMENTS.DASHBOARD_DATA(tournamentId));
  }
}
