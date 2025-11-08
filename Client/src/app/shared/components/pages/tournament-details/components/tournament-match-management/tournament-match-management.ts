import {
  Component,
  input,
  signal,
  OnInit,
  inject,
  computed,
  output
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';

import { Match, MatchEvent } from '@/app/api/models/match.models';
import { Team } from '@/app/api/models/team.models';
import {
  MatchStatus,
  TeamRegistrationStatus,
} from '@/app/api/models/common.models';
import { MatchService } from '@/app/api/services/match.service';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Tournament } from '@/app/api/models';

import {
  MatchTabsComponent,
  MatchListComponent,
  UpdateResultModalComponent,
  type MatchTab,
} from './components';

@Component({
  selector: 'app-tournament-match-management',
  imports: [
    CommonModule,
    MatchTabsComponent,
    MatchListComponent,
    UpdateResultModalComponent,
  ],
  templateUrl: './tournament-match-management.html',
  styleUrls: ['./tournament-match-management.css'],
})
export class TournamentMatchManagementComponent implements OnInit {
  tournamentId = input.required<string>();

  private readonly matchService = inject(MatchService);
  private readonly tournamentService = inject(TournamentService);
  private readonly toastService = inject(ToastService);
  matchView = output<string>();

  matches = signal<Match[]>([]);
  tournament = signal<Tournament | null>(null);
  tournamentTeams = signal<Team[]>([]);

  activeTab = signal<MatchTab>('inprogress');
  isLoading = signal(false);

  showCreateMatchModal = signal(false);
  showEditMatchModal = signal(false);
  showUpdateResultModal = signal(false);
  showAddEventModal = signal(false);
  showGenerateFixturesModal = signal(false);

  selectedMatch = signal<Match | null>(null);
  selectedMatchEvents = signal<MatchEvent[]>([]);

  filteredMatches = computed(() => {
    const allMatches = this.matches();
    const tab = this.activeTab();
    return allMatches.filter((match) => {
      switch (tab) {
        case 'scheduled':
          return match.status === MatchStatus.Scheduled;
        case 'inprogress':
          return match.status === MatchStatus.InProgress;
        case 'completed':
          return match.status === MatchStatus.Completed;
        default:
          return true;
      }
    });
  });

  ngOnInit() {
    this.loadTournament();
    this.loadMatches();
    this.loadTournamentTeams();
  }

  loadTournament() {
    this.isLoading.set(true);

    this.tournamentService.getTournament(this.tournamentId()).subscribe({
      next: (tournament) => {
        this.tournament.set(tournament);
      },
      complete: () => {
        this.isLoading.set(false);
      },
    });
  }

  loadMatches() {
    this.isLoading.set(true);

    this.matchService.getTournamentMatches(this.tournamentId()).subscribe({
      next: (matches) => {
        this.matches.set(matches);
      },
      complete: () => {
        this.isLoading.set(false);
      },
    });
  }

  loadTournamentTeams() {
    this.tournamentService
      .getTournamentTeams(this.tournamentId(), TeamRegistrationStatus.Approved)
      .subscribe({
        next: (teams) => {
          const convertedTeams: Team[] = teams.map((tt) => ({
            id: tt.id,
            name: tt.name,
            shortName: tt.shortName || '',
            logoUrl: tt.logoUrl,
            userId: tt.managerId || '',
            createdAt: tt.registeredAt,
            players: tt.players,
            countPlayers: tt.players.length,
            updatedAt: undefined,
          }));
          this.tournamentTeams.set(convertedTeams);
        },
      });
  }

  loadMatchEvents(matchId: string) {
    this.matchService.getMatchEvents(matchId).subscribe({
      next: (events) => {
        this.selectedMatchEvents.set(events);
      },
    });
  }

  onTabChange(tab: MatchTab) {
    this.activeTab.set(tab);
  }

  onMatchView(match: Match) {
    this.matchView.emit(match.id);
  }
}
