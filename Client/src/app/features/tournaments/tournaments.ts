import { Tournament, TournamentSearchRequest } from '@/app/shared/models/tournament';
import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TournamentCard } from '@/app/shared/components/tournament-card/tournament-card';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentService } from '@/app/core/services/tournament.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";

@Component({
  selector: 'app-tournaments',
  templateUrl: './tournaments.html',
  styleUrls: ['./tournaments.css'],
  standalone: true,
  imports: [
    DashboardLayout,
    CommonModule,
    FormsModule,
    LucideAngularModule,
    TournamentCard,
    ConfirmDeleteModal
],
})

export class Tournaments implements OnInit {
  private router = inject(Router);
  private toast = inject(ToastService);
  private tournamentService = inject(TournamentService);
  
  sidebarOpen = false;
  searchTerm = '';
  filterStatus = 'All';
  isDeleteModalOpen = signal(false);
  deletingTournament = signal<string | null>(null);
  readonly icons = LucideIcons;
  gridLayout = signal<boolean>(true);
  loading = signal<boolean>(false);

  tournaments = signal<Tournament[]>([]);
  filteredTournaments = signal<Tournament[]>([]);

  ngOnInit() {
    this.loadTournaments();
  }

  async loadTournaments() {
    try {
      this.loading.set(true);
      const searchRequest: TournamentSearchRequest = {
        searchTerm: this.searchTerm || undefined,
        status: this.filterStatus === 'All' ? undefined : this.filterStatus
      };
      
      const response = await this.tournamentService.searchTournaments(searchRequest).toPromise();
      this.tournaments.set(response?.data || []);
      this.filterTournaments();
    } catch (error) {
      console.error('Failed to load tournaments:', error);
      this.toast.error('Failed to load tournaments');
      this.tournaments.set([]);
      this.filteredTournaments.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  filterTournaments() {
    const tournaments = this.tournaments();
    const filtered = tournaments.filter((tournament: Tournament) => {
      const matchesSearch =
        this.searchTerm === '' ||
        tournament.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        tournament.description
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase());
      const matchesFilter =
        this.filterStatus === 'All' || tournament.status === this.filterStatus;
      return matchesSearch && matchesFilter;
    });
    this.filteredTournaments.set(filtered);
  }

  onSearchChange() {
    this.loadTournaments();
  }

  onFilterChange() {
    this.loadTournaments();
  }

  handleEditTournament(tournament: Tournament) {
    this.router.navigate(['/tournaments','edit', tournament.id]);
  }

  switchLayout(toGrid: boolean) {
    this.gridLayout.set(toGrid);
  }

  handleDeleteTournament(id: string) {
    this.deletingTournament.set(id);
    this.isDeleteModalOpen.set(true);
  }

  async confirmDelete() {
    const tournamentId = this.deletingTournament();
    if (!tournamentId) return;

    try {
      await this.tournamentService.deleteTournament(tournamentId).toPromise();
      
      const currentTournaments = this.tournaments();
      const updatedTournaments = currentTournaments.filter((t: Tournament) => t.id !== tournamentId);
      this.tournaments.set(updatedTournaments);
      this.filterTournaments();
      
      this.toast.success('Tournament deleted successfully');
    } catch (error) {
      console.error('Failed to delete tournament:', error);
      this.toast.error('Failed to delete tournament');
    } finally {
      this.isDeleteModalOpen.set(false);
      this.deletingTournament.set(null);
    }
  }

  handleViewTournament(tournament: Tournament) {
    this.router.navigate(['/tournaments','details', tournament.id]);
  }

  createNewTournament() {
    this.router.navigate(['/tournaments','create']);
  }
}
