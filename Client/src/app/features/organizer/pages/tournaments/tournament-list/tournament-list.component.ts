import { Component, OnInit, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Tournament, TournamentSearch } from '@/app/api/models/tournament.models';
import { PaginatedResponse } from '@/app/api/models/api.models';
import { TournamentCardComponent } from '../components/tournament-card/tournament-card.component';
import { TournamentSearchComponent } from '../components/tournament-search/tournament-search.component';
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";
import { ToastService } from '@/app/shared/services/toast.service';
import { OrganizerService } from '@/app/api/services';

@Component({
  selector: 'app-tournament-list',
  standalone: true,
  imports: [
    CommonModule,
    TournamentCardComponent,
    TournamentSearchComponent,
    ConfirmDeleteModal
],
  templateUrl: './tournament-list.component.html',
  styleUrl: './tournament-list.component.css'
})
export class TournamentListComponent implements OnInit {
  private organizerService = inject(OrganizerService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  tournaments = signal<Tournament[]>([]);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isDeleteModalOpen = signal(false);
  tournamentToDelete = signal<Tournament | null>(null);
  isDeleting = signal(false);
  
  searchCriteria = signal<TournamentSearch>({
    searchTerm: undefined,
    status: undefined,
    location: undefined,
    formatId: undefined,
    minPrizePool: undefined,
    maxPrizePool: undefined,
    startDateFrom: undefined,
    startDateTo: undefined,
    isPublic: undefined,
    pageNumber: 1,
    pageSize: 6
  });

  paginationData = signal<PaginatedResponse<Tournament> | null>(null);
  
  totalRecords = computed(() => this.paginationData()?.totalRecords ?? 0);
  totalPages = computed(() => this.paginationData()?.totalPages ?? 0);
  currentPage = computed(() => this.paginationData()?.pageNumber ?? 1);
  hasNextPage = computed(() => this.paginationData()?.hasNextPage ?? false);
  hasPreviousPage = computed(() => this.paginationData()?.hasPreviousPage ?? false);

  ngOnInit() {
    this.loadTournaments();
  }

  loadTournaments() {
    this.isLoading.set(true);
    this.error.set(null);

    this.organizerService.searchTournaments(this.searchCriteria()).subscribe({
      next: (response) => {
        this.tournaments.set(response.data);
        this.paginationData.set(response);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.error.set('Failed to load tournaments');
        this.isLoading.set(false);
      }
    });
  }

  onSearchChange(searchCriteria: Partial<TournamentSearch>) {
    this.searchCriteria.update(current => ({
      ...current,
      ...searchCriteria
    }));

    this.loadTournaments();
  }

  onPageChange(pageNumber: number) {
    this.searchCriteria.update(current => ({
      ...current,
      pageNumber
    }));

    this.loadTournaments();
  }

  onPageSizeChange(pageSize: number) {
    this.searchCriteria.update(current => ({
      ...current,
      pageSize
    }));

    this.loadTournaments();
  }

  onPageSizeSelectChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.onPageSizeChange(+target.value);

    this.loadTournaments();
  }

  onTournamentAction(action: { type: string, tournament: Tournament }) {
    switch (action.type) {
      case 'edit':
        this.router.navigate(['/organizer/tournaments/edit', action.tournament.id]);
        break;
      case 'view':
        this.router.navigate(['/organizer/tournaments', action.tournament.id]);
        break;
      case 'delete':
        this.deleteTournament(action.tournament);
        break;
    }
  }

  deleteTournament(tournament: Tournament) {
    this.tournamentToDelete.set(tournament);
    this.isDeleteModalOpen.set(true);
  }

  confirmDelete() {
    const tournament = this.tournamentToDelete();
    if (!tournament) return;

    this.isDeleting.set(true);
    this.organizerService.deleteTournament(tournament.id).subscribe({
      next: () => {
        this.toastService.success('Tournament deleted successfully');
        this.loadTournaments();
        this.closeDeleteModal();
      },
      error: (error) => {
        this.closeDeleteModal()
        this.isDeleting.set(false);
      }
    });
  }

  closeDeleteModal() {
    this.isDeleteModalOpen.set(false);
    this.tournamentToDelete.set(null);
    this.isDeleting.set(false);
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

  createTournament() {
    this.router.navigate(['/organizer/tournaments/create']);
  }

  editTournament(tournament: Tournament) {
    this.router.navigate(['/organizer/tournaments', tournament.id, 'edit']);
  }

  viewTournament(tournament: Tournament) {
    this.router.navigate(['/organizer/tournaments', tournament.id]);
  }
}
