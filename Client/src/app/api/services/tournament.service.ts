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

  getTournamentTeams(id: string, status?: string): Observable<TournamentTeam[]> {
    const url = status ? `${API_ENDPOINTS.TOURNAMENTS.TEAMS(id)}?status=${status}` : API_ENDPOINTS.TOURNAMENTS.TEAMS(id);
    return this.apiClient.get<TournamentTeam[]>(url);
  }

  getTournamentMatches(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.MATCHES(id));
  }

  getTournamentStandings(id: string): Observable<any[]> {
    return this.apiClient.get<any[]>(API_ENDPOINTS.TOURNAMENTS.STANDINGS(id));
  }

  manageTeam(tournamentId: string, teamId: string | null, action: 'register' | 'withdraw' | 'approve' | 'reject'): Observable<void> {
    const actionData = { action, teamId };
    const endpoint = teamId ? API_ENDPOINTS.TOURNAMENTS.MANAGE_TEAM(tournamentId, teamId) : API_ENDPOINTS.TOURNAMENTS.MANAGE_TEAM(tournamentId);
    return this.apiClient.put<void>(endpoint, actionData);
  }

  registerForTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeam(tournamentId, teamId, 'register');
  }

  withdrawFromTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeam(tournamentId, teamId, 'withdraw');
  }

  approveTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeam(tournamentId, teamId, 'approve');
  }

  rejectTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeam(tournamentId, teamId, 'reject');
  }

  createTournament(tournament: CreateTournament): Observable<Tournament> {
    const formData = new FormData();
    formData.append('Name', tournament.name);
    formData.append('Description', tournament.description);
    if (tournament.location) {
      formData.append('Location', tournament.location);
    }
    if (tournament.registrationStartDate) {
      formData.append('RegistrationStartDate', new Date(tournament.registrationStartDate).toISOString());
    }
    if (tournament.registrationDeadline) {
      formData.append('RegistrationDeadline', new Date(tournament.registrationDeadline).toISOString());
    }
    formData.append('StartDate', new Date(tournament.startDate).toISOString());
    formData.append('EndDate', new Date(tournament.endDate).toISOString());
    formData.append('MinTeams', tournament.minTeams.toString());
    formData.append('MaxTeams', tournament.maxTeams.toString());
    if (tournament.bannerUrl) {
      formData.append('BannerUrl', tournament.bannerUrl);
    }
    if (tournament.logoUrl) {
      formData.append('LogoUrl', tournament.logoUrl);
    }
    formData.append('IsPublic', tournament.isPublic.toString());
    
    // for (let [key, value] of formData.entries()) {
    //   console.log(`${key}:`, value);
    // }
    
    return this.apiClient.postFormData<Tournament>(API_ENDPOINTS.TOURNAMENTS.CREATE, formData);
  }

  updateTournament(id: string, updates: UpdateTournament): Observable<Tournament> {
    const formData = new FormData();
    if (updates.name) {
      formData.append('Name', updates.name);
    }
    if (updates.description) {
      formData.append('Description', updates.description);
    }
    if (updates.location) {
      formData.append('Location', updates.location);
    }
    if (updates.registrationStartDate) {
      formData.append('RegistrationStartDate', updates.registrationStartDate);
    }
    if (updates.registrationDeadline) {
      formData.append('RegistrationDeadline', updates.registrationDeadline);
    }
    if (updates.startDate) {
      formData.append('StartDate', updates.startDate);
    }
    if (updates.endDate) {
      formData.append('EndDate', updates.endDate);
    }
    if (updates.minTeams !== undefined) {
      formData.append('MinTeams', updates.minTeams.toString());
    }
    if (updates.maxTeams !== undefined) {
      formData.append('MaxTeams', updates.maxTeams.toString());
    }
    if (updates.bannerUrl) {
      formData.append('BannerUrl', updates.bannerUrl);
    }
    if (updates.logoUrl) {
      formData.append('LogoUrl', updates.logoUrl);
    }
    if (updates.isPublic !== undefined) {
      formData.append('IsPublic', updates.isPublic.toString());
    }
    
    return this.apiClient.putFormData<Tournament>(API_ENDPOINTS.TOURNAMENTS.UPDATE(id), formData);
  }

  deleteTournament(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TOURNAMENTS.DELETE(id));
  }

  createTournamentBracket(tournamentId: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.CREATE_BRACKET(tournamentId), {});
  }

  getTournamentFormats(): TournamentFormat[] {
    return getTournamentFormats();
  }
}
