import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import {
  Player,
  CreatePlayer,
  UpdatePlayer,
  PlayerSearch,
} from '../models/player.models';

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private apiClient = inject(ApiClient);

  // Public player viewing in tournament context
  getPublicPlayerDetails(tournamentId: string, playerId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.TOURNAMENTS.PLAYER_DETAILS(tournamentId, playerId));
  }

  getPublicPlayerStats(tournamentId: string, playerId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.TOURNAMENTS.PLAYER_STATS(tournamentId, playerId));
  }

  getTopScorers(tournamentId: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TOP_SCORERS(tournamentId) + `?limit=${limit}`);
  }

  getTopAssists(tournamentId: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TOP_ASSISTS(tournamentId) + `?limit=${limit}`);
  }

  getBestPlayers(tournamentId: string, limit: number = 10): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.BEST_PLAYERS(tournamentId) + `?limit=${limit}`);
  }

  getTeamRoster(tournamentId: string, teamId: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.TEAM_ROSTER(tournamentId, teamId));
  }

  // User team player management
  getMyTeamPlayers(teamId: string): Observable<Player[]> {
    return this.apiClient.get<Player[]>(API_ENDPOINTS.USERS.PLAYERS.LIST(teamId));
  }

  getMyTeamPlayer(teamId: string, playerId: string): Observable<Player> {
    return this.apiClient.get<Player>(API_ENDPOINTS.USERS.PLAYERS.GET(teamId, playerId));
  }

  addPlayerToMyTeam(teamId: string, player: CreatePlayer): Observable<Player> {
    return this.apiClient.post<Player>(API_ENDPOINTS.USERS.PLAYERS.ADD(teamId), player);
  }

  updateMyTeamPlayer(teamId: string, playerId: string, updates: UpdatePlayer): Observable<Player> {
    return this.apiClient.put<Player>(API_ENDPOINTS.USERS.PLAYERS.UPDATE(teamId, playerId), updates);
  }

  removePlayerFromMyTeam(teamId: string, playerId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.USERS.PLAYERS.REMOVE(teamId, playerId));
  }

  searchMyTeamPlayers(teamId: string, params?: PlayerSearch): Observable<Player[]> {
    const queryParams = params ? '?' + new URLSearchParams(params as any).toString() : '';
    return this.apiClient.get<Player[]>(API_ENDPOINTS.USERS.PLAYERS.SEARCH(teamId) + queryParams);
  }

  getPlayerProfile(playerId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.USERS.PLAYERS.PROFILE(playerId));
  }

  getPlayerStatistics(playerId: string): Observable<any> {
    return this.apiClient.get<any>(API_ENDPOINTS.USERS.PLAYERS.STATISTICS(playerId));
  }
}
