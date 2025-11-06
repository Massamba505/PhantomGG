import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import {
  Tournament,
  TournamentSearch,
} from '@/app/api/models/tournament.models';
import { PagedResult } from '@/app/api/models/api.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TournamentCard } from '@/app/shared/components/cards/tournament-card/tournament-card';
import { TournamentSearchComponent } from '@/app/shared/components/search';
import { TournamentStatus } from '@/app/api/models';

@Component({
  selector: 'app-tournament-browse',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TournamentCard,
    TournamentSearchComponent,
  ],
  templateUrl: './tournament-browse.html',
  styleUrls: ['./tournament-browse.css'],
})
export class TournamentBrowse implements OnInit {
  private tournamentService = inject(TournamentService);
  private toastService = inject(ToastService);
  private router = inject(Router);

  readonly icons = LucideIcons;

  tournaments = signal<Tournament[]>([]);
  isLoading = signal(false);

  searchCriteria = signal<TournamentSearch>({
    searchTerm: undefined,
    status: undefined,
    location: undefined,
    startFrom: undefined,
    startTo: undefined,
    isPublic: true,
    page: 1,
    pageSize: 6,
  });

  paginationData = signal<PagedResult<Tournament> | null>(null);

  totalRecords = computed(() => this.paginationData()?.meta.totalRecords ?? 0);
  totalPages = computed(() => this.paginationData()?.meta.totalPages ?? 0);
  currentPage = computed(() => this.paginationData()?.meta.page ?? 1);
  hasNextPage = computed(
    () => this.paginationData()?.meta.hasNextPage ?? false
  );
  hasPreviousPage = computed(
    () => this.paginationData()?.meta.hasPreviousPage ?? false
  );

  ngOnInit() {
    this.loadTournaments();
  }

  loadTournaments() {
    this.isLoading.set(true);

    this.tournamentService.getTournaments(this.searchCriteria()).subscribe({
      next: (response) => {
        this.tournaments.set(response.data);
        this.paginationData.set(response);
      },
      error: (error) => {
        this.toastService.error('Failed to load tournaments');
      },
      complete: () => {
        this.isLoading.set(false);
      },
    });
  }

  onSearchChange(searchCriteria: Partial<TournamentSearch>) {
    this.searchCriteria.update((current) => ({
      ...current,
      ...searchCriteria,
      isPublic: true,
      page: 1,
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
      isPublic: true,
      page: 1,
      pageSize: 6,
    });
    this.loadTournaments();
  }

  onPageChange(pageNumber: number) {
    this.searchCriteria.update((current) => ({
      ...current,
      page: pageNumber,
    }));
    this.loadTournaments();
  }

  onPageSizeChange(pageSize: number) {
    this.searchCriteria.update((current) => ({
      ...current,
      pageSize,
      page: 1,
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

  onTournamentView(tournament: Tournament) {
    this.router.navigate(['/public/tournaments', tournament.id]);
  }

  canJoinTournament(tournament: Tournament): boolean {
    return (
      tournament.status === TournamentStatus.RegistrationOpen &&
      tournament.teamCount < tournament.maxTeams
    );
  }
}
