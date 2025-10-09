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
  UpdateMatchEventDto,
  PlayerEventsSummary,
  TeamEventsSummary,
  MatchEventType
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
    return new Observable(observer => {
      observer.next([]);
      observer.complete();
    });
  }

  getLiveMatches(tournamentId?: string): Observable<PagedResult<MatchDto>> {
    return this.getMatches({
      tournamentId,
      status: 'InProgress'
    });
  }
  
  getUpcomingMatches(tournamentId?: string): Observable<PagedResult<MatchDto>> {
    return this.getMatches({
      tournamentId,
      status: 'Scheduled'
    });
  }
  
  getCompletedMatches(tournamentId?: string): Observable<PagedResult<MatchDto>> {
    return this.getMatches({
      tournamentId,
      status: 'Completed'
    });
  }
  
  getPlayerStatistics(playerId: string): Observable<PlayerEventsSummary> {
    return new Observable(observer => {
      this.getPlayerEvents(playerId).subscribe({
        next: (events: MatchEventDto[]) => {
          const summary: PlayerEventsSummary = {
            playerId,
            playerName: events[0]?.playerName || 'Unknown Player',
            teamId: events[0]?.teamId || '',
            teamName: events[0]?.teamName || 'Unknown Team',
            goals: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Goal).length,
            assists: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Assist).length,
            yellowCards: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.YellowCard).length,
            redCards: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.RedCard).length,
            fouls: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Foul).length,
            totalEvents: events.length
          };
          observer.next(summary);
          observer.complete();
        },
        error: (error) => observer.error(error)
      });
    });
  }
  
  getTeamStatistics(teamId: string): Observable<TeamEventsSummary> {
    return new Observable(observer => {
      this.getTeamEvents(teamId).subscribe({
        next: (events: MatchEventDto[]) => {
          const playerGoals = events
            .filter((e: MatchEventDto) => e.eventType === MatchEventType.Goal)
            .reduce((acc: Record<string, number>, event: MatchEventDto) => {
              acc[event.playerId] = (acc[event.playerId] || 0) + 1;
              return acc;
            }, {} as Record<string, number>);

          const topScorerEntry = Object.entries(playerGoals)
            .sort(([,a], [,b]) => (b as number) - (a as number))[0];

          const topScorer = topScorerEntry ? {
            playerId: topScorerEntry[0],
            playerName: events.find((e: MatchEventDto) => e.playerId === topScorerEntry[0])?.playerName || 'Unknown',
            goals: topScorerEntry[1] as number
          } : undefined;

          const summary: TeamEventsSummary = {
            teamId,
            teamName: events[0]?.teamName || 'Unknown Team',
            totalGoals: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Goal).length,
            totalAssists: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Assist).length,
            totalYellowCards: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.YellowCard).length,
            totalRedCards: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.RedCard).length,
            totalFouls: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Foul).length,
            totalSubstitutions: events.filter((e: MatchEventDto) => e.eventType === MatchEventType.Substitution).length,
            totalEvents: events.length,
            topScorer
          };
          observer.next(summary);
          observer.complete();
        },
        error: (error) => observer.error(error)
      });
    });
  }
}
