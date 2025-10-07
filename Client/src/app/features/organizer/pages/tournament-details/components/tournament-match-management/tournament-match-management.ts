import { Component, input, signal, OnInit, inject, computed, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';

import { 
  Match, 
  CreateMatch, 
  UpdateMatch, 
  MatchResult, 
  MatchEvent, 
  CreateMatchEvent, 
  GenerateFixtures 
} from '@/app/api/models/match.models';
import { Team, Player } from '@/app/api/models/team.models';
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
  type MatchTab
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
    GenerateFixturesModalComponent
  ],
  templateUrl: './tournament-match-management.html',
  styleUrls: ['./tournament-match-management.css']
})
export class TournamentMatchManagementComponent implements OnInit {
  tournamentId = input.required<string>();
  
  private matchService = inject(MatchService);
  private tournamentService = inject(TournamentService);
  private toastService = inject(ToastService);
  
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
    
    return allMatches.filter(match => {
      const status = match.status?.toLowerCase();
      switch (tab) {
        case 'scheduled':
          return status === 'scheduled' || status === 'pending';
        case 'inprogress':
          return status === 'inprogress';
        case 'completed':
          return status === 'completed' || status === 'finished';
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
      error: (error) => {
        this.toastService.error('Failed to load tournament');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  loadMatches() {
    this.isLoading.set(true);
    
    this.matchService.getTournamentMatches(this.tournamentId()).subscribe({
      next: (matches) => {
        this.matches.set(matches);
      },
      error: (error) => {
        this.toastService.error('Failed to load matches');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  loadTournamentTeams() {
    this.tournamentService.getTournamentTeams(this.tournamentId(), 'Approved').subscribe({
      next: (teams) => {
        const convertedTeams: Team[] = teams.map(tt => ({
          id: tt.id,
          name: tt.name,
          shortName: tt.shortName,
          logoUrl: tt.logoUrl,
          userId: tt.managerId || '',
          createdAt: tt.registeredAt,
          players:tt.players,
          updatedAt: undefined
        }));
        this.tournamentTeams.set(convertedTeams);
      },
      error: (error) => {
        this.toastService.error('Failed to load tournament teams');
      }
    });
  }

  loadMatchEvents(matchId: string) {
    this.matchService.getMatchEvents(matchId).subscribe({
      next: (events) => {
        this.selectedMatchEvents.set(events);
      },
      error: (error) => {
        this.toastService.error('Failed to load match events');
      }
    });
  }

  loadPlayersForMatch(match: Match) {
    const mockPlayers = this.generateMockPlayersForTeams(match.homeTeamId, match.awayTeamId);
  }

  generateMockPlayersForTeams(homeTeamId: string, awayTeamId: string): { homeTeam: Player[], awayTeam: Player[] } {
    const positions = ['Goalkeeper', 'Defender', 'Midfielder', 'Forward'];
    
    const generatePlayersForTeam = (teamId: string, teamName: string): Player[] => {
      return Array.from({ length: 11 }, (_, i) => ({
        id: `${teamId}_player_${i + 1}`,
        firstName: `Player`,
        lastName: `${i + 1}`,
        position: positions[i % positions.length],
        email: `player${i + 1}@${teamName.toLowerCase().replace(' ', '')}.com`,
        photoUrl: undefined,
        teamId,
        teamName,
        joinedAt: new Date().toISOString()
      }));
    };

    const homeTeam = this.tournamentTeams().find(t => t.id === homeTeamId);
    const awayTeam = this.tournamentTeams().find(t => t.id === awayTeamId);

    return {
      homeTeam: homeTeam ? generatePlayersForTeam(homeTeamId, homeTeam.name) : [],
      awayTeam: awayTeam ? generatePlayersForTeam(awayTeamId, awayTeam.name) : []
    };
  }

  onTabChange(tab: MatchTab) {
    this.activeTab.set(tab);
  }

  onMatchView(match: Match) {
    this.selectedMatch.set(match);
    this.loadMatchEvents(match.id);
    console.log('View match:', match);
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
      error: (error) => {
        this.toastService.error('Failed to create match');
      }
    });
  }

  onEditMatch(data: { matchId: string; updateData: UpdateMatch }) {
    this.matchService.updateMatch(data.matchId, data.updateData).subscribe({
      next: (match) => {
        this.toastService.success('Match updated successfully');
        this.showEditMatchModal.set(false);
        this.loadMatches();
      },
      error: (error) => {
        this.toastService.error('Failed to update match');
      }
    });
  }

  onUpdateResult(data: { matchId: string; result: MatchResult }) {
    this.matchService.updateMatchResult(data.matchId, data.result).subscribe({
      next: (match) => {
        this.toastService.success('Match result updated successfully');
        this.showUpdateResultModal.set(false);
        this.loadMatches();
      },
      error: (error) => {
        this.toastService.error('Failed to update match result');
      }
    });
  }

  onAddEvent(eventData: CreateMatchEvent) {
    this.matchService.createMatchEvent(eventData).subscribe({
      next: (event) => {
        this.toastService.success('Match event added successfully');
        this.loadMatchEvents(this.selectedMatch()!.id);
      },
      error: (error) => {
        this.toastService.error('Failed to add match event');
      }
    });
  }

  onGenerateFixtures(fixtureData: GenerateFixtures) {
    this.matchService.generateFixtures(fixtureData).subscribe({
      next: (matches) => {
        this.toastService.success(`${matches.length} fixtures generated successfully`);
        this.showGenerateFixturesModal.set(false);
        this.loadMatches();
      },
      error: (error) => {
        this.toastService.error('Failed to generate fixtures');
      }
    });
  }
}