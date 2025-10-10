import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { PagedResult } from '../models/api.models';
import { 
  CreateMatchDto,
  UpdateMatchDto,
  MatchDto,
  MatchQuery,
  MatchResultDto,
  MatchEventDto,
  CreateMatchEventDto,
  UpdateMatchEventDto
} from '../models/match.models';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private apiClient = inject(ApiClient);

  getMatches(params?: Partial<MatchQuery>): Observable<PagedResult<MatchDto>> {
    const query: MatchQuery = {
      page: 1,
      pageSize: 20,
      ...params
    };
    return this.apiClient.getPaged<MatchDto>(API_ENDPOINTS.MATCHES.LIST, query);
  }

  getMatch(matchId: string): Observable<MatchDto> {
    return this.apiClient.get<MatchDto>(API_ENDPOINTS.MATCHES.GET(matchId));
  }
  
  createMatch(matchData: CreateMatchDto): Observable<MatchDto> {
    return this.apiClient.post<MatchDto>(API_ENDPOINTS.MATCHES.CREATE, matchData);
  }
  
  updateMatch(matchId: string, updates: UpdateMatchDto): Observable<MatchDto> {
    return this.apiClient.patch<MatchDto>(API_ENDPOINTS.MATCHES.UPDATE(matchId), updates);
  }
  
  updateMatchResult(matchId: string, result: MatchResultDto): Observable<MatchDto> {
    return this.apiClient.patch<MatchDto>(API_ENDPOINTS.MATCHES.UPDATE_RESULT(matchId), result);
  }
  
  deleteMatch(matchId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.MATCHES.DELETE(matchId));
  }
  
  getMatchEvents(matchId: string, type?: string): Observable<MatchEventDto[]> {
    const url = type ? `${API_ENDPOINTS.MATCHES.EVENTS.LIST_BY_MATCH(matchId)}?type=${type}` : API_ENDPOINTS.MATCHES.EVENTS.LIST_BY_MATCH(matchId);
    return this.apiClient.get<MatchEventDto[]>(url);
  }
  
  getMatchEvent(matchId: string, eventId: string): Observable<MatchEventDto> {
    return this.apiClient.get<MatchEventDto>(API_ENDPOINTS.MATCHES.EVENTS.GET(matchId, eventId));
  }
  
  createMatchEvent(matchId: string, eventData: CreateMatchEventDto): Observable<MatchEventDto> {
    const eventDataWithMatch = { ...eventData, matchId };
    return this.apiClient.post<MatchEventDto>(API_ENDPOINTS.MATCHES.EVENTS.CREATE(matchId), eventDataWithMatch);
  }
  
  updateMatchEvent(matchId: string, eventId: string, updates: UpdateMatchEventDto): Observable<MatchEventDto> {
    return this.apiClient.patch<MatchEventDto>(API_ENDPOINTS.MATCHES.EVENTS.UPDATE(matchId, eventId), updates);
  }
  
  deleteMatchEvent(matchId: string, eventId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.MATCHES.EVENTS.DELETE(matchId, eventId));
  }
  
  getPlayerEvents(playerId: string): Observable<MatchEventDto[]> {
    return this.apiClient.get<MatchEventDto[]>(API_ENDPOINTS.MATCHES.EVENTS.PLAYER_EVENTS(playerId));
  }
  
  getTeamEvents(teamId: string): Observable<MatchEventDto[]> {
    return this.apiClient.get<MatchEventDto[]>(API_ENDPOINTS.MATCHES.EVENTS.TEAM_EVENTS(teamId));
  }
  
  getTournamentMatches(tournamentId: string): Observable<MatchDto[]> {
    return new Observable(observer => {
      this.getMatches({ tournamentId }).subscribe({
        next: (pagedResult) => observer.next(pagedResult.data),
        error: (error) => observer.error(error),
        complete: () => observer.complete()
      });
    });
  }

  generateFixtures(fixtureData: any): Observable<MatchDto[]> {
    const tournamentId = fixtureData.tournamentId;
    return this.apiClient.post<MatchDto[]>(API_ENDPOINTS.TOURNAMENTS.GENERATE_FIXTURES(tournamentId), {});
  }
}
