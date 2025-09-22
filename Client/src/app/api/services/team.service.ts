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

  getTeams(params?: TeamSearch): Observable<PaginatedResponse<Team>> {
    return this.apiClient.getPaginated<Team>(API_ENDPOINTS.TEAMS.LIST, params);
  }

  getTeam(id: string): Observable<Team> {
    return this.apiClient.get<Team>(API_ENDPOINTS.TEAMS.GET(id));
  }

  getTeamPlayers(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TEAMS.PLAYERS(id));
  }

  getMyTeams(params?: TeamSearch): Observable<PaginatedResponse<Team>> {
    return this.apiClient.getPaginated<Team>(API_ENDPOINTS.TEAMS.MY_TEAMS, params);
  }

  createTeam(team: CreateTeam): Observable<Team> {
    return this.apiClient.post<Team>(API_ENDPOINTS.TEAMS.CREATE, team);
  }

  updateTeam(id: string, updates: UpdateTeam): Observable<Team> {
    return this.apiClient.put<Team>(API_ENDPOINTS.TEAMS.UPDATE(id), updates);
  }

  deleteTeam(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.DELETE(id));
  }

  uploadTeamLogo(teamId: string, file: File): Observable<{ logoUrl: string }> {
    return this.apiClient.uploadFile<{ logoUrl: string }>(API_ENDPOINTS.TEAMS.UPLOAD_LOGO(teamId), file);
  }

  addPlayerToTeam(teamId: string, playerData: any): Observable<any> {
    return this.apiClient.post<any>(API_ENDPOINTS.TEAMS.ADD_PLAYER(teamId), playerData);
  }

  updateTeamPlayer(teamId: string, playerId: string, updates: any): Observable<any> {
    return this.apiClient.put<any>(API_ENDPOINTS.TEAMS.UPDATE_PLAYER(teamId, playerId), updates);
  }

  removePlayerFromTeam(teamId: string, playerId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.REMOVE_PLAYER(teamId, playerId));
  }
}