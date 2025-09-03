import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@/environments/environment.development';
import { ApiResponse } from '@/app/shared/models/ApiResponse';
import {
  Team,
  CreateTeamRequest,
  UpdateTeamRequest,
  TeamSearchRequest,
} from '@/app/shared/models/tournament';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private readonly http = inject(HttpClient);
  private readonly env = environment;

  getAllTeams(): Observable<ApiResponse<Team[]>> {
    return this.http.get<ApiResponse<Team[]>>(
      `${this.env.apiUrl}/teams`
    );
  }

  getTeamById(id: string): Observable<ApiResponse<Team>> {
    return this.http.get<ApiResponse<Team>>(
      `${this.env.apiUrl}/teams/${id}`
    );
  }

  getTeamsByTournament(tournamentId: string): Observable<ApiResponse<Team[]>> {
    return this.http.get<ApiResponse<Team[]>>(
      `${this.env.apiUrl}/teams/tournament/${tournamentId}`
    );
  }

  searchTeams(searchRequest: TeamSearchRequest): Observable<ApiResponse<Team[]>> {
    let params = new HttpParams();
    
    if (searchRequest.searchTerm) {
      params = params.set('searchTerm', searchRequest.searchTerm);
    }
    if (searchRequest.tournamentId) {
      params = params.set('tournamentId', searchRequest.tournamentId);
    }
    if (searchRequest.pageNumber) {
      params = params.set('pageNumber', searchRequest.pageNumber.toString());
    }
    if (searchRequest.pageSize) {
      params = params.set('pageSize', searchRequest.pageSize.toString());
    }

    return this.http.get<ApiResponse<Team[]>>(
      `${this.env.apiUrl}/teams/search`,
      { params }
    );
  }

  getMyTeams(): Observable<ApiResponse<Team[]>> {
    return this.http.get<ApiResponse<Team[]>>(
      `${this.env.apiUrl}/teams/my-teams`
    );
  }

  createTeam(team: CreateTeamRequest): Observable<ApiResponse<Team>> {
    return this.http.post<ApiResponse<Team>>(
      `${this.env.apiUrl}/teams`,
      team
    );
  }

  updateTeam(id: string, team: UpdateTeamRequest): Observable<ApiResponse<Team>> {
    return this.http.put<ApiResponse<Team>>(
      `${this.env.apiUrl}/teams/${id}`,
      team
    );
  }

  deleteTeam(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(
      `${this.env.apiUrl}/teams/${id}`
    );
  }
}
