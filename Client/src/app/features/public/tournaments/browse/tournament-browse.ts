import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';

import { TournamentCard } from '@/app/shared/components/cards/tournament-card/tournament-card';
import { Tournament, TournamentSearch } from '@/app/api/models/tournament.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-tournament-browse',
  templateUrl: './tournament-browse.html',
  styleUrls: ['./tournament-browse.css'],
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    TournamentCard,
    // TournamentFilter
  ],
})
export class TournamentBrowse implements OnInit {
  tournaments = signal<Tournament[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  searchTerm = signal('');
  
  // Pagination
  currentPage = signal(1);
  pageSize = signal(12);
  totalCount = signal(0);
  totalPages = computed(() => Math.ceil(this.totalCount() / this.pageSize()));
  
  // Search and filters
  searchFilters = signal<TournamentSearch>({
    isPublic: true,
    pageNumber: 1,
    pageSize: 12
  });

  readonly icons = LucideIcons;

  constructor(
    private tournamentService: TournamentService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadTournaments();
  }

  async loadTournaments() {
    this.loading.set(true);
    this.error.set(null);
    
    try {
      const filters = {
        ...this.searchFilters(),
        pageNumber: this.currentPage(),
        pageSize: this.pageSize(),
        scope: 'public' as const
      };
      
      const result = await this.tournamentService.getTournaments(filters).toPromise();
      this.tournaments.set(result?.data || []);
      this.totalCount.set(result?.totalRecords || 0);
    } catch (error) {
      console.error('Error loading tournaments:', error);
      this.error.set('Failed to load tournaments. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  onFiltersChanged(filters: TournamentSearch) {
    this.searchFilters.set({ ...filters, isPublic: true });
    this.currentPage.set(1);
    this.loadTournaments();
  }

  onTournamentView(tournament: Tournament) {
    this.router.navigate(['/public/tournaments', tournament.id]);
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadTournaments();
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'RegistrationOpen':
        return 'badge-success';
      case 'InProgress':
        return 'badge-warning';
      case 'Completed':
        return 'badge-secondary';
      default:
        return 'badge-primary';
    }
  }

  trackByTournamentId(index: number, tournament: Tournament): string {
    return tournament.id;
  }

  onSearchChange() {
    this.searchFilters.set({
      ...this.searchFilters(),
      searchTerm: this.searchTerm() || undefined
    });
    this.currentPage.set(1);
    this.loadTournaments();
  }
}