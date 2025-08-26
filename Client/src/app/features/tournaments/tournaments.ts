import { Tournament } from '@/app/shared/models/tournament';
import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { TournamentCard } from '@/app/shared/components/tournament-card/tournament-card';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastService } from '@/app/shared/services/toast.service';
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
  
  sidebarOpen = false;
  searchTerm = '';
  filterStatus = 'all';
  isDeleteModalOpen = signal(false);
  deletingTournament = signal<string | null>(null);
  readonly icons = LucideIcons;
  gridLayout = signal<boolean>(true);

  tournaments: Tournament[] = [
    {
      id: '1',
      name: 'Summer League 2024',
      description:
        'Annual summer soccer tournament for youth teams across the region',
      startDate: '2024-06-15',
      endDate: '2024-07-30',
      maxTeams: 16,
      status: 'active',
      createdAt: '2024-05-01',
      teams: [],
    },
    {
      id: '2',
      name: 'Champions Cup',
      description: 'Elite tournament for the best teams in the league',
      startDate: '2024-08-01',
      endDate: '2024-09-15',
      maxTeams: 8,
      status: 'draft',
      createdAt: '2024-05-15',
      teams: [],
    },
    {
      id: '3',
      name: 'Youth Championship',
      description: 'Under-18 championship tournament with regional qualifiers',
      startDate: '2024-04-01',
      endDate: '2024-05-20',
      maxTeams: 12,
      status: 'completed',
      createdAt: '2024-03-01',
      teams: [],
    },
    {
      id: '4',
      name: 'Winter Invitational',
      description: 'Exclusive winter tournament for invited teams only',
      startDate: '2024-12-01',
      endDate: '2024-12-20',
      maxTeams: 10,
      status: 'draft',
      createdAt: '2024-05-10',
      teams: [],
    },
    {
      id: '5',
      name: 'Regional Qualifier',
      description: 'Tournament to determine regional representatives',
      startDate: '2024-07-10',
      endDate: '2024-07-25',
      maxTeams: 16,
      status: 'active',
      createdAt: '2024-04-20',
      teams: [],
    },
  ];

  filteredTournaments: Tournament[] = [];

  ngOnInit() {
    this.filterTournaments();
  }

  filterTournaments() {
    this.filteredTournaments = this.tournaments.filter((tournament) => {
      const matchesSearch =
        this.searchTerm === '' ||
        tournament.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        tournament.description
          .toLowerCase()
          .includes(this.searchTerm.toLowerCase());
      const matchesFilter =
        this.filterStatus === 'all' || tournament.status === this.filterStatus;
      return matchesSearch && matchesFilter;
    });
  }

  handleEditTournament(tournament: Tournament) {
    // Navigate to edit page instead of opening modal
    this.router.navigate(['/edit-tournament', tournament.id]);
  }

  switchLayout(toGrid: boolean) {
    this.gridLayout.set(toGrid);
  }

  handleDeleteTournament(id: string) {
    this.deletingTournament.set(id);
    this.isDeleteModalOpen.set(true);
  }

  confirmDelete() {
    this.tournaments = this.tournaments.filter((t) => t.id !== this.deletingTournament());
    this.filterTournaments();
    this.toast.success('Tournament deleted successfully');
    this.isDeleteModalOpen.set(false);
    this.deletingTournament.set(null);
  }

  handleViewTournament(tournament: Tournament) {
    this.router.navigate(['/tournament-details', tournament.id]);
  }

  createNewTournament() {
    this.router.navigate(['/create-tournament']);
  }
}
