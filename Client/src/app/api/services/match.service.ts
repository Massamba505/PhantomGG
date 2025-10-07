import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiClient } from '../base/api-client.service';
import { API_ENDPOINTS } from '../base/api-endpoints';
import { 
  CreateMatch,
  UpdateMatch,
  Match,
  MatchSearch,
  MatchResult,
  GenerateFixtures,
  FixtureGenerationStatus,
  MatchEvent,
  CreateMatchEvent,
  UpdateMatchEvent,
  PlayerEventsSummary,
  TeamEventsSummary
} from '../models/match.models';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private apiClient = inject(ApiClient);

  getMatch(matchId: string): Observable<Match> {
    return this.apiClient.get<Match>(API_ENDPOINTS.MATCHES.GET(matchId));
  }
  getTournamentMatches(tournamentId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.MATCHES.TOURNAMENT_MATCHES(tournamentId));
  }
  
  getTeamMatches(teamId: string): Observable<Match[]> {
    return this.apiClient.get<Match[]>(API_ENDPOINTS.MATCHES.TEAM_MATCHES(teamId));
  }
  
  searchMatches(searchCriteria: MatchSearch): Observable<Match[]> {
    const queryParams = new URLSearchParams();
    
    Object.entries(searchCriteria).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        queryParams.append(key, value.toString());
      }
    });

    const endpoint = queryParams.toString() 
      ? `${API_ENDPOINTS.MATCHES.SEARCH}?${queryParams.toString()}`
      : API_ENDPOINTS.MATCHES.SEARCH;

    return this.apiClient.get<Match[]>(endpoint);
  }
  
  createMatch(matchData: CreateMatch): Observable<Match> {
    return this.apiClient.post<Match>(API_ENDPOINTS.MATCHES.CREATE, matchData);
  }
  
  updateMatch(matchId: string, updates: UpdateMatch): Observable<Match> {
    return this.apiClient.put<Match>(API_ENDPOINTS.MATCHES.UPDATE(matchId), updates);
  }
  
  updateMatchResult(matchId: string, result: MatchResult): Observable<Match> {
    return this.apiClient.put<Match>(API_ENDPOINTS.MATCHES.UPDATE_RESULT(matchId), result);
  }
  
  deleteMatch(matchId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.MATCHES.DELETE(matchId));
  }

  generateFixtures(fixtureData: GenerateFixtures): Observable<Match[]> {
    return this.apiClient.post<Match[]>(API_ENDPOINTS.MATCHES.GENERATE_FIXTURES, fixtureData);
  }
  
  getFixtureGenerationStatus(tournamentId: string): Observable<FixtureGenerationStatus> {
    return this.apiClient.get<FixtureGenerationStatus>(API_ENDPOINTS.MATCHES.FIXTURE_STATUS(tournamentId));
  }
  
  getMatchEvents(matchId: string): Observable<MatchEvent[]> {
    return this.apiClient.get<MatchEvent[]>(API_ENDPOINTS.MATCHES.EVENTS.LIST_BY_MATCH(matchId));
  }
  
  getMatchEvent(eventId: string): Observable<MatchEvent> {
    return this.apiClient.get<MatchEvent>(API_ENDPOINTS.MATCHES.EVENTS.GET(eventId));
  }
  
  getPlayerEvents(playerId: string): Observable<MatchEvent[]> {
    return this.apiClient.get<MatchEvent[]>(API_ENDPOINTS.MATCHES.EVENTS.PLAYER_EVENTS(playerId));
  }
  
  getTeamEvents(teamId: string): Observable<MatchEvent[]> {
    return this.apiClient.get<MatchEvent[]>(API_ENDPOINTS.MATCHES.EVENTS.TEAM_EVENTS(teamId));
  }
  
  createMatchEvent(eventData: CreateMatchEvent): Observable<MatchEvent> {
    return this.apiClient.post<MatchEvent>(API_ENDPOINTS.MATCHES.EVENTS.CREATE, eventData);
  }
  
  updateMatchEvent(eventId: string, updates: UpdateMatchEvent): Observable<MatchEvent> {
    return this.apiClient.put<MatchEvent>(API_ENDPOINTS.MATCHES.EVENTS.UPDATE(eventId), updates);
  }
  
  deleteMatchEvent(eventId: string): Observable<void> {
    return this.apiClient.delete<void>(API_ENDPOINTS.MATCHES.EVENTS.DELETE(eventId));
  }
  
  getLiveMatches(tournamentId: string): Observable<Match[]> {
    return this.searchMatches({
      tournamentId,
      status: 'Live'
    });
  }
  
  getUpcomingMatches(tournamentId: string): Observable<Match[]> {
    return this.searchMatches({
      tournamentId,
      status: 'Scheduled'
    });
  }
  
  getCompletedMatches(tournamentId: string): Observable<Match[]> {
    return this.searchMatches({
      tournamentId,
      status: 'Completed'
    });
  }
  
  getPlayerStatistics(playerId: string): Observable<PlayerEventsSummary> {
    return new Observable(observer => {
      this.getPlayerEvents(playerId).subscribe({
        next: (events: MatchEvent[]) => {
          const summary: PlayerEventsSummary = {
            playerId,
            playerName: events[0]?.playerName || 'Unknown Player',
            teamId: events[0]?.teamId || '',
            teamName: events[0]?.teamName || 'Unknown Team',
            goals: events.filter((e: MatchEvent) => e.eventType === 'Goal').length,
            assists: events.filter((e: MatchEvent) => e.eventType === 'Assist').length,
            yellowCards: events.filter((e: MatchEvent) => e.eventType === 'YellowCard').length,
            redCards: events.filter((e: MatchEvent) => e.eventType === 'RedCard').length,
            fouls: events.filter((e: MatchEvent) => e.eventType === 'Foul').length,
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
        next: (events: MatchEvent[]) => {
          const playerGoals = events
            .filter((e: MatchEvent) => e.eventType === 'Goal')
            .reduce((acc: Record<string, number>, event: MatchEvent) => {
              acc[event.playerId] = (acc[event.playerId] || 0) + 1;
              return acc;
            }, {} as Record<string, number>);

          const topScorerEntry = Object.entries(playerGoals)
            .sort(([,a], [,b]) => (b as number) - (a as number))[0];

          const topScorer = topScorerEntry ? {
            playerId: topScorerEntry[0],
            playerName: events.find((e: MatchEvent) => e.playerId === topScorerEntry[0])?.playerName || 'Unknown',
            goals: topScorerEntry[1] as number
          } : undefined;

          const summary: TeamEventsSummary = {
            teamId,
            teamName: events[0]?.teamName || 'Unknown Team',
            totalGoals: events.filter((e: MatchEvent) => e.eventType === 'Goal').length,
            totalAssists: events.filter((e: MatchEvent) => e.eventType === 'Assist').length,
            totalYellowCards: events.filter((e: MatchEvent) => e.eventType === 'YellowCard').length,
            totalRedCards: events.filter((e: MatchEvent) => e.eventType === 'RedCard').length,
            totalFouls: events.filter((e: MatchEvent) => e.eventType === 'Foul').length,
            totalSubstitutions: events.filter((e: MatchEvent) => e.eventType === 'Substitution').length,
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
