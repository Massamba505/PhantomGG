import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Tournament, TournamentSearch } from '@/app/api/models/tournament.models';
import { PagedResult } from '@/app/api/models/api.models';
import { TournamentCard } from '@/app/shared/components/cards';
import { TournamentSearchComponent } from './components/tournament-search/tournament-search';
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentService, UserRoles } from '@/app/api/services';
import { AuthStateService } from '@/app/store/AuthStateService';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-tournament-list',
  imports: [
    CommonModule,
    LucideAngularModule,
    TournamentCard,
    TournamentSearchComponent,
    ConfirmDeleteModal
],
  templateUrl: './tournament-list.html',
  styleUrl: './tournament-list.css'
})
export class TournamentListComponent implements OnInit {
  private tournamentService = inject(TournamentService);
  private router = inject(Router);
  private toastService = inject(ToastService);
  private authStateStore = inject(AuthStateService);

  readonly icons = LucideIcons;

  tournaments = signal<Tournament[]>([]);
  isLoading = signal(false);
  isDeleteModalOpen = signal(false);
  tournamentToDelete = signal<Tournament | null>(null);
  isDeleting = signal(false);

  isOrganizer = computed(()=>{
    if(!this.authStateStore.isAuthenticated()){
      return false;
    }
    return this.authStateStore.user()!.role == UserRoles.Organizer;
  })
  
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

  ngOnInit() {
    this.loadTournaments();
  }

  loadTournaments() {
    this.isLoading.set(true);

    this.tournamentService.getTournaments({ ...this.searchCriteria(), scope: 'my' }).subscribe({
      next: (response: any) => {
        this.tournaments.set(response.data);
        this.paginationData.set(response);
      },
      complete:()=>{
        this.isLoading.set(false);
      },
    });
  }

  onSearchChange(searchCriteria: Partial<TournamentSearch>) {
    this.searchCriteria.update(current => ({
      ...current,
      ...searchCriteria
    }));

    this.loadTournaments();
  }

  onClearSearh(){
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
        this.deleteTournament(action.tournament.id);
        break;
    }
  }

  deleteTournament(tournamentId: string) {
    const tournament = this.tournaments().find(t => t.id === tournamentId);
    if (!tournament) return;
    
    this.tournamentToDelete.set(tournament);
    this.isDeleteModalOpen.set(true);
  }

  confirmDelete() {
    const tournament = this.tournamentToDelete();
    if (!tournament) return;

    this.isDeleting.set(true);
    this.tournamentService.deleteTournament(tournament.id).subscribe({
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
