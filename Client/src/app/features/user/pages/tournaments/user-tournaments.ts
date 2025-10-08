import { Component, signal, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentService } from '@/app/api/services/tournament.service';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Tournament, TournamentSearch } from '@/app/api/models/tournament.models';
import { Team } from '@/app/api/models/team.models';
import { PagedResult } from '@/app/api/models/api.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamSelectionModalComponent } from './components/team-selection-modal/team-selection-modal.component';
import { TournamentCard } from '@/app/shared/components/cards/tournament-card/tournament-card';
import { TournamentSearchComponent } from './components/tournament-search/tournament-search.component';

@Component({
  selector: 'app-user-tournaments',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TeamSelectionModalComponent,
    TournamentCard,
    TournamentSearchComponent
  ],
  templateUrl: './user-tournaments.html',
  styleUrl: './user-tournaments.css'
})
export class UserTournaments implements OnInit {
  private tournamentService = inject(TournamentService);
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  
  readonly icons = LucideIcons;
  
  tournaments = signal<Tournament[]>([]);
  myTeams = signal<Team[]>([]);
  isLoading = signal(false);
  isJoiningTournament = signal(false);
  
  searchCriteria = signal<TournamentSearch>({
    searchTerm: undefined,
    status: undefined,
    location: undefined,
    startFrom: undefined,
    startTo: undefined,
    isPublic: undefined,
    page: 1,
    pageSize: 6
  });

  paginationData = signal<PagedResult<Tournament> | null>(null);
  

  totalRecords = computed(() => this.paginationData()?.meta.totalRecords ?? 0);
  totalPages = computed(() => this.paginationData()?.meta.totalPages ?? 0);
  currentPage = computed(() => this.paginationData()?.meta.page ?? 1);
  hasNextPage = computed(() => this.paginationData()?.meta.hasNextPage ?? false);
  hasPreviousPage = computed(() => this.paginationData()?.meta.hasPreviousPage ?? false);
  
  showTeamSelectionModal = signal(false);
  selectedTournament = signal<Tournament | null>(null);

  ngOnInit() {
    this.loadTournaments();
    this.loadMyTeams();
  }

  loadTournaments() {
    this.isLoading.set(true);
    
    this.tournamentService.getTournaments(this.searchCriteria()).subscribe({
      next: (response) => {
        this.tournaments.set(response.data);
        this.paginationData.set(response);
      },
      error: (error) => {
        console.error('Failed to load tournaments:', error);
        this.toastService.error('Failed to load tournaments');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  loadMyTeams() {
    this.teamService.getTeams().subscribe({
      next: (response: any) => {
        this.myTeams.set(response.data);
      },
      error: (error: any) => {
        console.error('Failed to load teams:', error);
      }
    });
  }

  onSearchChange(searchCriteria: Partial<TournamentSearch>) {
    this.searchCriteria.update(current => ({
      ...current,
      ...searchCriteria,
      pageNumber: 1
    }));
    this.loadTournaments();
  }

  onClearSearch() {
    this.searchCriteria.set({
      searchTerm: undefined,
      status: undefined,
      location: undefined,
      startFrom: undefined,
      startTo: undefined,
      isPublic: undefined,
      page: 1,
      pageSize: 6
    });
    this.loadTournaments();
  }

  onPageChange(pageNumber: number) {
    this.searchCriteria.update(current => ({
      ...current,
      page: pageNumber
    }));
    this.loadTournaments();
  }

  onPageSizeChange(pageSize: number) {
    this.searchCriteria.update(current => ({
      ...current,
      pageSize,
      pageNumber: 1
    }));
    this.loadTournaments();
  }

  onPageSizeSelectChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.onPageSizeChange(+target.value);
  }

  getPageNumbers(): number[] {
    const current = this.currentPage();
    const total = this.totalPages();
    const delta = 2;
    
    const range: number[] = [];
    const start = Math.max(1, current - delta);
    const end = Math.min(total, current + delta);
    
    for (let i = start; i <= end; i++) {
      range.push(i);
    }
    
    return range;
  }

  joinTournament(tournament: Tournament) {
    const teams = this.myTeams();
    if (teams.length === 0) {
      this.toastService.error('You need to create a team first before joining tournaments');
      return;
    }

    this.selectedTournament.set(tournament);
    this.showTeamSelectionModal.set(true);
  }

  onTeamSelected(team: Team) {
    const tournament = this.selectedTournament();
    if (!tournament) return;
    
    this.isJoiningTournament.set(true);
    
    this.tournamentService.registerForTournament(tournament.id, team.id).subscribe({
      next: () => {
        this.toastService.success(`Successfully registered ${team.name} for ${tournament.name}`);
        this.loadTournaments();
        this.closeTeamSelectionModal();
      },
      error: (error: any) => {
        this.isJoiningTournament.set(false);
      },
      complete: () => {
        this.isJoiningTournament.set(false);
      }
    });
  }

  onTeamSelectionModalClosed() {
    this.closeTeamSelectionModal();
  }

  private closeTeamSelectionModal() {
    this.showTeamSelectionModal.set(false);
    this.selectedTournament.set(null);
    this.isJoiningTournament.set(false);
  }

  onCreateTeamFromModal() {
    this.router.navigate(['/user/teams/create']);
  }

  canJoinTournament(tournament: Tournament): boolean {
    return (
      tournament.status === 'RegistrationOpen' &&
      tournament.teamCount < tournament.maxTeams &&
      this.myTeams().length > 0
    );
  }

  getRegistrationStatus(tournament: Tournament): string {
    return tournament.status;
  }


  onTournamentJoin(tournament: Tournament) {
    this.joinTournament(tournament);
  }

  onTournamentView(tournament: Tournament) {
    this.router.navigate(['/user/tournaments', tournament.id]);
  }

  onTournamentLeave(tournament: Tournament) {

    console.log('Leave tournament:', tournament);
  }
}