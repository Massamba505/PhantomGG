import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@/environments/environment.development';
import { ApiResponse } from '@/app/shared/models/ApiResponse';
import {
  Tournament,
  CreateTournamentRequest,
  UpdateTournamentRequest,
  TournamentSearchRequest,
} from '@/app/shared/models/tournament';

@Injectable({
  providedIn: 'root',
})
export class TournamentService {
  private readonly http = inject(HttpClient);
  private readonly env = environment;

  getAllTournaments(): Observable<ApiResponse<Tournament[]>> {
    return this.http.get<ApiResponse<Tournament[]>>(
      `${this.env.apiUrl}/tournaments`
    );
  }

  getTournamentById(id: string): Observable<ApiResponse<Tournament>> {
    return this.http.get<ApiResponse<Tournament>>(
      `${this.env.apiUrl}/tournaments/${id}`
    );
  }

  searchTournaments(searchRequest: TournamentSearchRequest): Observable<ApiResponse<Tournament[]>> {
    return this.http.post<ApiResponse<Tournament[]>>(
      `${this.env.apiUrl}/tournaments/search`,
      searchRequest
    );
  }

  getMyTournaments(): Observable<ApiResponse<Tournament[]>> {
    return this.http.get<ApiResponse<Tournament[]>>(
      `${this.env.apiUrl}/tournaments/my-tournaments`
    );
  }

  createTournament(tournament: CreateTournamentRequest): Observable<ApiResponse<Tournament>> {
    return this.http.post<ApiResponse<Tournament>>(
      `${this.env.apiUrl}/tournaments`,
      tournament
    );
  }

  updateTournament(id: string, tournament: UpdateTournamentRequest): Observable<ApiResponse<Tournament>> {
    return this.http.put<ApiResponse<Tournament>>(
      `${this.env.apiUrl}/tournaments/${id}`,
      tournament
    );
  }

  deleteTournament(id: string): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(
      `${this.env.apiUrl}/tournaments/${id}`
    );
  }
}
