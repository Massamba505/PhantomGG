import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PaginatedResponse } from '../models/api.models';
import {
  Team,
  CreateTeam,
  UpdateTeam,
  TeamSearch
} from '../models/team.models';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  private apiClient = inject(ApiClient);

  // Public team discovery and viewing
  getPublicTeams(params?: TeamSearch): Observable<PaginatedResponse<Team>> {
    return this.apiClient.getPaginated<Team>(API_ENDPOINTS.TOURNAMENTS.LIST, params);
  }

  getTeam(id: string): Observable<Team> {
    return this.apiClient.get<Team>(API_ENDPOINTS.TOURNAMENTS.GET(id));
  }

  searchTeams(params: TeamSearch): Observable<PaginatedResponse<Team>> {
    return this.apiClient.getPaginated<Team>(API_ENDPOINTS.TOURNAMENTS.SEARCH, params);
  }

  getTeamsByTournament(tournamentId: string): Observable<Team[]> {
    return this.apiClient.get<Team[]>(API_ENDPOINTS.TOURNAMENTS.TEAMS(tournamentId));
  }

  getTeamStats(teamId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.TOURNAMENTS.TEAM_STATS(teamId));
  }

  getTeamPlayers(tournamentId: string, teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAM_ROSTER(tournamentId, teamId));
  }

  getTeamMatches(tournamentId: string, teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAM_MATCHES(tournamentId, teamId));
  }

  // User team management operations
  getMyTeams(): Observable<Team[]> {
    return this.apiClient.get<Team[]>(API_ENDPOINTS.USERS.TEAMS.LIST);
  }

  createTeam(team: CreateTeam): Observable<Team> {
    return this.apiClient.post<Team>(API_ENDPOINTS.USERS.TEAMS.CREATE, team);
  }

  updateMyTeam(id: string, updates: UpdateTeam): Observable<Team> {
    return this.apiClient.put<Team>(API_ENDPOINTS.USERS.TEAMS.UPDATE(id), updates);
  }

  deleteMyTeam(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.USERS.TEAMS.DELETE(id));
  }

  uploadTeamLogo(teamId: string, file: File): Observable<{ logoUrl: string }> {
    return this.apiClient.uploadFile<{ logoUrl: string }>(API_ENDPOINTS.USERS.TEAMS.UPLOAD_LOGO(teamId), file);
  }

  getMyTeamStatistics(teamId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.USERS.TEAMS.STATISTICS(teamId));
  }

  getMyTeamMatches(teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.USERS.TEAMS.MATCHES(teamId));
  }

  getMyTeamUpcomingMatches(teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.USERS.TEAMS.UPCOMING_MATCHES(teamId));
  }

  getMyTeamMatchHistory(teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.USERS.TEAMS.MATCH_HISTORY(teamId));
  }

  // Tournament registration
  registerForTournament(tournamentData: any): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.USERS.TOURNAMENTS.REGISTER, tournamentData);
  }

  withdrawFromTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.USERS.TOURNAMENTS.WITHDRAW(tournamentId, teamId));
  }

  getApplicationStatus(tournamentId: string, teamId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.USERS.TOURNAMENTS.APPLICATION_STATUS(tournamentId, teamId));
  }

  // Legacy compatibility methods for existing components
  updateTeam(id: string, updates: UpdateTeam): Observable<Team> {
    return this.updateMyTeam(id, updates);
  }

  deleteTeam(id: string): Observable<void> {
    return this.deleteMyTeam(id);
  }
}