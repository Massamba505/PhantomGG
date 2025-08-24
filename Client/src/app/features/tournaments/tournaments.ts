import { Tournament, TournamentFormData } from '@/app/shared/models/tournament';
import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Modal } from '@/app/shared/components/modal/modal.component';
import { TournamentFormComponent } from '@/app/shared/components/tournament-form/tournament-form';
import { TournamentCard } from '@/app/shared/components/tournament-card/tournament-card';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-tournaments',
  templateUrl: './tournaments.html',
  styleUrls: ['./tournaments.css'],
  standalone: true,
  imports: [
    Modal,
    TournamentFormComponent,
    DashboardLayout,
    CommonModule,
    FormsModule,
  ],
})
export class Tournaments implements OnInit {
  sidebarOpen = false;
  searchTerm = '';
  filterStatus = 'all';
  isEditModalOpen = signal(false);
  editingTournament = signal<Tournament | null>(null);

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

  constructor(private router: Router, private toast: ToastService) {}

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
    this.editingTournament.set(tournament);
    this.isEditModalOpen.set(true);
  }

  handleDeleteTournament(id: string) {
    if (confirm('Are you sure you want to delete this tournament?')) {
      this.tournaments = this.tournaments.filter((t) => t.id !== id);
      this.filterTournaments();
      this.toast.success('Tournament deleted successfully');
    }
  }

  handleViewTournament(tournament: Tournament) {
    this.router.navigate(['/tournament-details', tournament.id]);
  }

  handleSaveTournament(formData: TournamentFormData) {
    const currentTournament = this.editingTournament();
    if (currentTournament) {
      this.tournaments = this.tournaments.map((t) =>
        t.id === currentTournament.id ? { ...t, ...formData } : t
      );
      this.filterTournaments();
      this.toast.success('Tournament updated successfully');
    }
    this.isEditModalOpen.set(false);
    this.editingTournament.set(null);
  }

  createNewTournament() {
    this.router.navigate(['/create-tournament']);
  }
}
