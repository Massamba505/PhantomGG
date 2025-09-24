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
    console.log('Creating tournament with data:', tournament);
    
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
    if (tournament.contactEmail) {
      formData.append('ContactEmail', tournament.contactEmail);
    }
    if (tournament.bannerUrl) {
      formData.append('BannerUrl', tournament.bannerUrl);
    }
    if (tournament.logoUrl) {
      formData.append('LogoUrl', tournament.logoUrl);
    }
    formData.append('IsPublic', tournament.isPublic.toString());
    
    // Debug: Log FormData entries
    console.log('FormData entries:');
    for (let [key, value] of formData.entries()) {
      console.log(`${key}:`, value);
    }
    
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
    if (updates.contactEmail) {
      formData.append('ContactEmail', updates.contactEmail);
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

  // Public methods for browsing tournaments
  getPublicTournaments(params?: TournamentSearch): Promise<PaginatedResponse<Tournament>> {
    const searchParams = { ...params, isPublic: true };
    return this.apiClient.getPaginated<Tournament>(API_ENDPOINTS.TOURNAMENTS.PUBLIC, searchParams).toPromise() as Promise<PaginatedResponse<Tournament>>;
  }

  getPublicTournament(id: string): Promise<Tournament> {
    return this.apiClient.get<Tournament>(API_ENDPOINTS.TOURNAMENTS.PUBLIC_DETAILS(id)).toPromise() as Promise<Tournament>;
  }

  getPublicTournamentTeams(id: string): Promise<TournamentTeam[]> {
    return this.apiClient.get<TournamentTeam[]>(API_ENDPOINTS.TOURNAMENTS.PUBLIC_TEAMS(id)).toPromise() as Promise<TournamentTeam[]>;
  }

  getPublicTournamentStatistics(id: string): Promise<TournamentStatistics> {
    return this.apiClient.get<TournamentStatistics>(API_ENDPOINTS.TOURNAMENTS.PUBLIC_STATISTICS(id)).toPromise() as Promise<TournamentStatistics>;
  }

}
