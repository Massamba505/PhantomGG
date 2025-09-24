import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PaginatedResponse } from '../models/api.models';
import {
  Team,
  CreateTeam,
  UpdateTeam,
  TeamSearch,
  Player,
  CreatePlayer,
  UpdatePlayer
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

  getTeamPlayers(id: string): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.TEAMS.PLAYERS(id));
  }

  getMyTeams(params?: TeamSearch): Observable<PaginatedResponse<Team>> {
    return this.apiClient.getPaginated<Team>(API_ENDPOINTS.TEAMS.MY_TEAMS, params);
  }

  createTeam(team: CreateTeam): Observable<Team> {
    const formData = new FormData();
    formData.append('Name', team.name);
    if (team.shortName) {
      formData.append('ShortName', team.shortName);
    }
    if (team.logoUrl) {
      formData.append('LogoUrl', team.logoUrl);
    }
    if (team.teamPhotoUrl) {
      formData.append('TeamPhotoUrl', team.teamPhotoUrl);
    }
    
    return this.apiClient.postFormData<Team>(API_ENDPOINTS.TEAMS.CREATE, formData);
  }

  updateTeam(id: string, updates: UpdateTeam): Observable<Team> {
    const formData = new FormData();
    formData.append('Name', updates.name);
    if (updates.shortName) {
      formData.append('ShortName', updates.shortName);
    }
    if (updates.logoUrl) {
      formData.append('LogoUrl', updates.logoUrl);
    }
    if (updates.teamPhotoUrl) {
      formData.append('TeamPhotoUrl', updates.teamPhotoUrl);
    }
    
    return this.apiClient.putFormData<Team>(API_ENDPOINTS.TEAMS.UPDATE(id), formData);
  }

  deleteTeam(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.DELETE(id));
  }

  uploadTeamLogo(teamId: string, file: File): Observable<{ logoUrl: string }> {
    return this.apiClient.uploadFile<{ logoUrl: string }>(API_ENDPOINTS.TEAMS.UPLOAD_LOGO(teamId), file);
  }

  addPlayerToTeam(teamId: string, playerData: CreatePlayer): Observable<Player> {
    const formData = new FormData();
    formData.append('FirstName', playerData.firstName);
    formData.append('LastName', playerData.lastName);
    if (playerData.position) {
      formData.append('Position', playerData.position);
    }
    if (playerData.email) {
      formData.append('Email', playerData.email);
    }
    if (playerData.photoUrl) {
      formData.append('PhotoUrl', playerData.photoUrl);
    }
    formData.append('TeamId', playerData.teamId);
    
    return this.apiClient.postFormData<Player>(API_ENDPOINTS.TEAMS.ADD_PLAYER(teamId), formData);
  }

  updateTeamPlayer(teamId: string, playerId: string, updates: UpdatePlayer): Observable<Player> {
    const formData = new FormData();
    formData.append('FirstName', updates.firstName);
    formData.append('LastName', updates.lastName);
    if (updates.position) {
      formData.append('Position', updates.position);
    }
    if (updates.email) {
      formData.append('Email', updates.email);
    }
    if (updates.photoUrl) {
      formData.append('PhotoUrl', updates.photoUrl);
    }
    
    return this.apiClient.putFormData<Player>(API_ENDPOINTS.TEAMS.UPDATE_PLAYER(teamId, playerId), formData);
  }

  removePlayerFromTeam(teamId: string, playerId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.REMOVE_PLAYER(teamId, playerId));
  }
}