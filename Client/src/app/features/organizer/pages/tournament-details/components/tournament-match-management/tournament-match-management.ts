import {
  Component,
  input,
  signal,
  OnInit,
  inject,
  computed,
  viewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';

import {
  Match,
  CreateMatch,
  UpdateMatch,
  MatchResult,
  MatchEvent,
  CreateMatchEvent,
  GenerateFixtures,
} from '@/app/api/models/match.models';
import { Team } from '@/app/api/models/team.models';
import {
  MatchStatus,
  TeamRegistrationStatus,
} from '@/app/api/models/common.models';
import { MatchService } from '@/app/api/services/match.service';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Tournament } from '@/app/api/models';

import {
  MatchHeaderComponent,
  MatchTabsComponent,
  MatchListComponent,
  CreateMatchModalComponent,
  EditMatchModalComponent,
  UpdateResultModalComponent,
  AddEventModalComponent,
  GenerateFixturesModalComponent,
  type MatchTab,
} from './components';

@Component({
  selector: 'app-tournament-match-management',
  imports: [
    CommonModule,
    MatchHeaderComponent,
    MatchTabsComponent,
    MatchListComponent,
    CreateMatchModalComponent,
    EditMatchModalComponent,
    UpdateResultModalComponent,
    AddEventModalComponent,
    GenerateFixturesModalComponent,
  ],
  templateUrl: './tournament-match-management.html',
  styleUrls: ['./tournament-match-management.css'],
})
export class TournamentMatchManagementComponent implements OnInit {
  tournamentId = input.required<string>();

  private readonly matchService = inject(MatchService);
  private readonly tournamentService = inject(TournamentService);
  private readonly toastService = inject(ToastService);

  matches = signal<Match[]>([]);
  tournament = signal<Tournament | null>(null);
  tournamentTeams = signal<Team[]>([]);

  activeTab = signal<MatchTab>('all');
  isLoading = signal(false);

  showCreateMatchModal = signal(false);
  showEditMatchModal = signal(false);
  showUpdateResultModal = signal(false);
  showAddEventModal = signal(false);
  showGenerateFixturesModal = signal(false);

  selectedMatch = signal<Match | null>(null);
  selectedMatchEvents = signal<MatchEvent[]>([]);

  createMatchModal = viewChild(CreateMatchModalComponent);
  addEventModal = viewChild(AddEventModalComponent);
  generateFixturesModal = viewChild(GenerateFixturesModalComponent);

  filteredMatches = computed(() => {
    const allMatches = this.matches();
    const tab = this.activeTab();

    if (tab === 'all') return allMatches;

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
    this.selectedMatch.set(match);
    this.loadMatchEvents(match.id);
  }

  onMatchEdit(match: Match) {
    this.selectedMatch.set(match);
    this.showEditMatchModal.set(true);
  }

  onMatchUpdateResult(match: Match) {
    this.selectedMatch.set(match);
    this.loadMatchEvents(match.id);
    this.showUpdateResultModal.set(true);
  }

  openCreateMatchModal() {
    this.createMatchModal()?.reset();
    this.showCreateMatchModal.set(true);
  }

  openGenerateFixturesModal() {
    this.generateFixturesModal()?.reset();
    this.showGenerateFixturesModal.set(true);
  }

  openAddEventModal() {
    if (!this.selectedMatch()) return;

    this.addEventModal()?.reset();
    this.showAddEventModal.set(true);
  }

  closeModals() {
    this.showCreateMatchModal.set(false);
    this.showEditMatchModal.set(false);
    this.showUpdateResultModal.set(false);
    this.showAddEventModal.set(false);
    this.showGenerateFixturesModal.set(false);
    this.selectedMatch.set(null);
  }

  onCreateMatch(createData: CreateMatch) {
    this.matchService.createMatch(createData).subscribe({
      next: (match) => {
        this.toastService.success('Match created successfully');
        this.showCreateMatchModal.set(false);
        this.loadMatches();
      },
    });
  }

  onEditMatch(data: { matchId: string; updateData: UpdateMatch }) {
    this.matchService.updateMatch(data.matchId, data.updateData).subscribe({
      next: (match) => {
        this.toastService.success('Match updated successfully');
        this.showEditMatchModal.set(false);
        this.loadMatches();
      },
    });
  }

  onUpdateResult(data: { matchId: string; result: MatchResult }) {
    this.matchService.updateMatchResult(data.matchId, data.result).subscribe({
      next: (match) => {
        this.toastService.success('Match result updated successfully');
        this.showUpdateResultModal.set(false);
        this.loadMatches();
      },
    });
  }

  onAddEvent(eventData: CreateMatchEvent) {
    const matchId = this.selectedMatch()?.id;
    if (!matchId) {
      this.toastService.error('No match selected');
      return;
    }

    this.matchService.createMatchEvent(matchId, eventData).subscribe({
      next: (event) => {
        this.toastService.success('Match event added successfully');
        this.showAddEventModal.set(false);
        this.loadMatchEvents(this.selectedMatch()!.id);
      },
    });
  }

  onGenerateFixtures(fixtureData: GenerateFixtures) {
    this.matchService.generateFixtures(fixtureData).subscribe({
      next: (matches) => {
        this.toastService.success(
          `${matches.length} fixtures generated successfully`
        );
        this.showGenerateFixturesModal.set(false);
        this.loadMatches();
      },
    });
  }
}
