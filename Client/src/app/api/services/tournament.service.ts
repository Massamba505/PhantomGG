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
  TournamentFormat,
  getTournamentFormats
} from '../models/tournament.models';
import { TournamentTeam } from '../models/team.models';

@Injectable({
  providedIn: 'root'
})
export class TournamentService {
  private apiClient = inject(ApiClient);

  getTournaments(params?: TournamentSearch): Observable<PaginatedResponse<Tournament>> {
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.LIST, params);
  }

  getTournament(id: string): Observable<Tournament> {
    return this.apiClient.get<Tournament>(API_ENDPOINTS.TOURNAMENTS.GET(id));
  }

  getTournamentTeams(id: string): Observable<TournamentTeam[]> {
    return this.apiClient.get<TournamentTeam[]>(API_ENDPOINTS.TOURNAMENTS.TEAMS(id));
  }

  getMyTournaments(params?: TournamentSearch): Observable<PaginatedResponse<Tournament>> {
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.MY_TOURNAMENTS, params);
  }

  registerForTournament(tournamentId: string, registrationData: any): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.REGISTER(tournamentId), registrationData);
  }

  withdrawFromTournament(tournamentId: string, withdrawData: any): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.WITHDRAW(tournamentId), withdrawData);
  }

  createTournament(tournament: CreateTournament): Observable<Tournament> {
    return this.apiClient.post<Tournament>(API_ENDPOINTS.TOURNAMENTS.CREATE, tournament);
  }

  updateTournament(id: string, updates: UpdateTournament): Observable<Tournament> {
    return this.apiClient.put<Tournament>(API_ENDPOINTS.TOURNAMENTS.UPDATE(id), updates);
  }

  deleteTournament(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TOURNAMENTS.DELETE(id));
  }

  uploadTournamentBanner(tournamentId: string, file: File): Observable<{ imageUrl: string }> {
    return this.apiClient.uploadFile<{ imageUrl: string }>(API_ENDPOINTS.TOURNAMENTS.UPLOAD_BANNER(tournamentId), file);
  }

  uploadTournamentLogo(tournamentId: string, file: File): Observable<{ imageUrl: string }> {
    return this.apiClient.uploadFile<{ imageUrl: string }>(API_ENDPOINTS.TOURNAMENTS.UPLOAD_LOGO(tournamentId), file);
  }

  getPendingTeams(tournamentId: string): Observable<TournamentTeam[]> {
    return this.apiClient.get<TournamentTeam[]>(API_ENDPOINTS.TOURNAMENTS.PENDING_TEAMS(tournamentId));
  }

  approveTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.APPROVE_TEAM(tournamentId, teamId), {});
  }

  rejectTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.REJECT_TEAM(tournamentId, teamId), {});
  }

  removeTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TOURNAMENTS.REMOVE_TEAM(tournamentId, teamId));
  }

  getTournamentFormats(): TournamentFormat[] {
    return getTournamentFormats();
  }

}
