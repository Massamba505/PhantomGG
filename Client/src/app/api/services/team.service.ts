import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PagedResult } from '../models/api.models';
import {
  TeamDto,
  CreateTeamDto,
  UpdateTeamDto,
  TeamQuery,
  PlayerDto,
  CreatePlayerDto,
  UpdatePlayerDto
} from '../models/team.models';

@Injectable({
  providedIn: 'root'
})
export class TeamService {
  private readonly apiClient = inject(ApiClient);

  getTeams(params?: Partial<TeamQuery>): Observable<PagedResult<TeamDto>> {
    const query: TeamQuery = {
      page: 1,
      pageSize: 10,
      ...params
    };
    return this.apiClient.getPaged<TeamDto>(API_ENDPOINTS.TEAMS.LIST, query);
  }

  getTeam(id: string): Observable<TeamDto> {
    return this.apiClient.get<TeamDto>(API_ENDPOINTS.TEAMS.GET(id));
  }

  getTeamPlayers(id: string): Observable<PlayerDto[]> {
    return this.apiClient.get<PlayerDto[]>(API_ENDPOINTS.TEAMS.PLAYERS(id));
  }

  createTeam(team: CreateTeamDto): Observable<TeamDto> {
    const formData = new FormData();
    formData.append('name', team.name);
    if (team.shortName) {
      formData.append('shortName', team.shortName);
    }
    if (team.logoUrl) {
      formData.append('logoUrl', team.logoUrl);
    }
    if (team.teamPhotoUrl) {
      formData.append('teamPhotoUrl', team.teamPhotoUrl);
    }
    
    return this.apiClient.postFormData<TeamDto>(API_ENDPOINTS.TEAMS.CREATE, formData);
  }

  updateTeam(id: string, updates: UpdateTeamDto): Observable<TeamDto> {
    const formData = new FormData();
    formData.append('name', updates.name);
    if (updates.shortName) {
      formData.append('shortName', updates.shortName);
    }
    if (updates.logoUrl) {
      formData.append('logoUrl', updates.logoUrl);
    }
    if (updates.teamPhotoUrl) {
      formData.append('teamPhotoUrl', updates.teamPhotoUrl);
    }
    
    return this.apiClient.patchFormData<TeamDto>(API_ENDPOINTS.TEAMS.UPDATE(id), formData);
  }

  deleteTeam(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.DELETE(id));
  }

  addPlayerToTeam(teamId: string, playerData: CreatePlayerDto): Observable<PlayerDto> {
    const formData = new FormData();
    formData.append('firstName', playerData.firstName);
    formData.append('lastName', playerData.lastName);
    if (playerData.position) {
      formData.append('position', playerData.position);
    }
    if (playerData.email) {
      formData.append('email', playerData.email);
    }
    if (playerData.photoUrl) {
      formData.append('photoUrl', playerData.photoUrl);
    }
    formData.append('teamId', playerData.teamId);
    
    return this.apiClient.postFormData<PlayerDto>(API_ENDPOINTS.TEAMS.ADD_PLAYER(teamId), formData);
  }

  updateTeamPlayer(teamId: string, playerId: string, updates: UpdatePlayerDto): Observable<PlayerDto> {
    const formData = new FormData();
    formData.append('firstName', updates.firstName);
    formData.append('lastName', updates.lastName);
    if (updates.position) {
      formData.append('position', updates.position.toString());
    }
    if (updates.email) {
      formData.append('email', updates.email);
    }
    if (updates.photoUrl) {
      formData.append('photoUrl', updates.photoUrl);
    }
    
    return this.apiClient.patchFormData<PlayerDto>(API_ENDPOINTS.TEAMS.UPDATE_PLAYER(teamId, playerId), formData);
  }

  removePlayerFromTeam(teamId: string, playerId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TEAMS.REMOVE_PLAYER(teamId, playerId));
  }
}