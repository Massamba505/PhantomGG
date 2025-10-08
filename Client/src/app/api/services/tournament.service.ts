import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PagedResult } from '../models/api.models';
import { TeamRegistrationStatus, TeamAction } from '../models/common.models';
import {
  TournamentDto,
  CreateTournamentDto,
  UpdateTournamentDto,
  TournamentQuery,
  TournamentFormat,
  getTournamentFormats,
  TeamManagementRequest,
  TournamentGenerateFixturesRequest,
  TournamentStandingDto,
  PlayerGoalStandingDto,
  PlayerAssistStandingDto
} from '../models/tournament.models';
import { TournamentTeamDto } from '../models/team.models';
import { MatchDto } from '../models/match.models';

@Injectable({
  providedIn: 'root'
})
export class TournamentService {
  private apiClient = inject(ApiClient);

  getTournaments(params?: Partial<TournamentQuery>): Observable<PagedResult<TournamentDto>> {
    const query: TournamentQuery = {
      page: 1,
      pageSize: 10,
      ...params
    };
    return this.apiClient.getPaged<TournamentDto>(API_ENDPOINTS.TOURNAMENTS.LIST, query);
  }

  getTournament(id: string): Observable<TournamentDto> {
    return this.apiClient.get<TournamentDto>(API_ENDPOINTS.TOURNAMENTS.GET(id));
  }

  getTournamentTeams(id: string, status: TeamRegistrationStatus = TeamRegistrationStatus.Approved): Observable<TournamentTeamDto[]> {
    const url = `${API_ENDPOINTS.TOURNAMENTS.TEAMS(id)}?status=${status}`;
    return this.apiClient.get<TournamentTeamDto[]>(url);
  }

  getTournamentMatches(id: string, status?: string): Observable<MatchDto[]> {
    const url = status ? `${API_ENDPOINTS.TOURNAMENTS.MATCHES(id)}?status=${status}` : API_ENDPOINTS.TOURNAMENTS.MATCHES(id);
    return this.apiClient.get<MatchDto[]>(url);
  }

  getTournamentStandings(id: string): Observable<TournamentStandingDto[]> {
    return this.apiClient.get<TournamentStandingDto[]>(API_ENDPOINTS.TOURNAMENTS.STANDINGS(id));
  }

  getPlayerGoalStandings(id: string): Observable<PlayerGoalStandingDto[]> {
    return this.apiClient.get<PlayerGoalStandingDto[]>(API_ENDPOINTS.TOURNAMENTS.GOAL_STANDINGS(id));
  }

  getPlayerAssistStandings(id: string): Observable<PlayerAssistStandingDto[]> {
    return this.apiClient.get<PlayerAssistStandingDto[]>(API_ENDPOINTS.TOURNAMENTS.ASSIST_STANDINGS(id));
  }

  registerTeamForTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.REGISTER_TEAM(tournamentId, teamId), {});
  }

  manageTeamParticipation(tournamentId: string, teamId: string, request: TeamManagementRequest): Observable<void> {
    return this.apiClient.patch<void>(API_ENDPOINTS.TOURNAMENTS.MANAGE_TEAM(tournamentId, teamId), request);
  }

  createTournament(tournament: CreateTournamentDto): Observable<TournamentDto> {
    const formData = new FormData();
    formData.append('name', tournament.name);
    formData.append('description', tournament.description);
    if (tournament.location) {
      formData.append('location', tournament.location);
    }
    formData.append('registrationStartDate', new Date(tournament.registrationStartDate).toISOString());
    formData.append('registrationDeadline', new Date(tournament.registrationDeadline).toISOString());
    formData.append('startDate', new Date(tournament.startDate).toISOString());
    formData.append('endDate', new Date(tournament.endDate).toISOString());
    formData.append('minTeams', tournament.minTeams.toString());
    formData.append('maxTeams', tournament.maxTeams.toString());
    if (tournament.bannerUrl) {
      formData.append('bannerUrl', tournament.bannerUrl);
    }
    if (tournament.logoUrl) {
      formData.append('logoUrl', tournament.logoUrl);
    }
    formData.append('isPublic', tournament.isPublic.toString());
    
    return this.apiClient.postFormData<TournamentDto>(API_ENDPOINTS.TOURNAMENTS.CREATE, formData);
  }

  updateTournament(id: string, updates: UpdateTournamentDto): Observable<TournamentDto> {
    const formData = new FormData();
    if (updates.name) {
      formData.append('name', updates.name);
    }
    if (updates.description) {
      formData.append('description', updates.description);
    }
    if (updates.location) {
      formData.append('location', updates.location);
    }
    if (updates.registrationStartDate) {
      formData.append('registrationStartDate', new Date(updates.registrationStartDate).toISOString());
    }
    if (updates.registrationDeadline) {
      formData.append('registrationDeadline', new Date(updates.registrationDeadline).toISOString());
    }
    if (updates.startDate) {
      formData.append('startDate', new Date(updates.startDate).toISOString());
    }
    if (updates.endDate) {
      formData.append('endDate', new Date(updates.endDate).toISOString());
    }
    if (updates.minTeams !== undefined) {
      formData.append('minTeams', updates.minTeams.toString());
    }
    if (updates.maxTeams !== undefined) {
      formData.append('maxTeams', updates.maxTeams.toString());
    }
    if (updates.bannerUrl) {
      formData.append('bannerUrl', updates.bannerUrl);
    }
    if (updates.logoUrl) {
      formData.append('logoUrl', updates.logoUrl);
    }
    if (updates.isPublic !== undefined) {
      formData.append('isPublic', updates.isPublic.toString());
    }
    
    return this.apiClient.patchFormData<TournamentDto>(API_ENDPOINTS.TOURNAMENTS.UPDATE(id), formData);
  }

  deleteTournament(id: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.TOURNAMENTS.DELETE(id));
  }

  generateFixtures(tournamentId: string, request: TournamentGenerateFixturesRequest): Observable<void> {
    return this.apiClient.post<void>(API_ENDPOINTS.TOURNAMENTS.GENERATE_FIXTURES(tournamentId), request);
  }

  approveTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeamParticipation(tournamentId, teamId, { action: TeamAction.Approve });
  }

  rejectTeam(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeamParticipation(tournamentId, teamId, { action: TeamAction.Reject });
  }

  registerForTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.registerTeamForTournament(tournamentId, teamId);
  }

  withdrawFromTournament(tournamentId: string, teamId: string): Observable<void> {
    return this.manageTeamParticipation(tournamentId, teamId, { action: TeamAction.Withdraw });
  }

  getTournamentFormats(): TournamentFormat[] {
    return getTournamentFormats();
  }
}
