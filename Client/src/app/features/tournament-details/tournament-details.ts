import { Team, TeamFormData, Tournament } from '@/app/shared/models/tournament';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';
import { TeamModal } from '@/app/shared/components/team-modal/team-modal';
import { ConfirmationDialog } from '@/app/shared/components/confirmation-dialog/confirmation-dialog';
import { TeamCard } from '@/app/shared/components/team-card/team-card';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";
import { Modal } from "@/app/shared/components/modal/modal";
import { ConfirmDeleteModal } from "@/app/shared/components/ConfirmDeleteModal/ConfirmDeleteModal";

@Component({
  selector: 'app-tournament-details',
  templateUrl: './tournament-details.html',
  styleUrls: ['./tournament-details.css'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TeamModal,
    TeamCard,
    DatePipe,
    TitleCasePipe,
    LucideAngularModule,
    ConfirmDeleteModal
],
  providers: [ToastService],
})
export class TournamentDetails implements OnInit {
  // Modal states
  isAddTeamModalOpen = false;
  isEditTeamModalOpen = false;
  isDeleteTeamConfirmOpen = false;

  readonly icons = LucideIcons;

  // Team management
  editingTeam: Team | null = null;
  teamToDelete: string | null = null;

  // Active tab
  activeTab: 'teams' | 'schedule' | 'bracket' | 'results' = 'teams';

  // Tournament data
  tournament: Tournament | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toast: ToastService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.tournament = {
      id: id || '1',
      name: 'Summer League 2024',
      description:
        'Annual summer soccer tournament for youth teams across the region. This prestigious competition brings together the best young talent from across the country to compete in a month-long championship.',
      startDate: '2024-06-15',
      endDate: '2024-07-30',
      maxTeams: 16,
      status: 'active',
      createdAt: '2024-05-01',
      teams: [
        {
          id: '1',
          name: 'FC Barcelona Academy',
          city: 'Barcelona',
          coach: 'Carlos Rodriguez',
          players: 22,
          tournamentId: id || '1',
          createdAt: '2024-05-15',
        },
        {
          id: '2',
          name: 'Real Madrid Youth',
          city: 'Madrid',
          coach: 'Miguel Santos',
          players: 20,
          tournamentId: id || '1',
          createdAt: '2024-05-16',
        },
        {
          id: '3',
          name: 'Manchester City U18',
          city: 'Manchester',
          coach: 'James Wilson',
          players: 21,
          tournamentId: id || '1',
          createdAt: '2024-05-17',
        },
      ],
    };
  }

  // Tab management
  setActiveTab(tab: 'teams' | 'schedule' | 'bracket' | 'results') {
    this.activeTab = tab;
  }

  // Tournament utils
  getTournamentDuration(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
  }

  getTeamRegistrationProgress(): number {
    if (!this.tournament || !this.tournament.teams) return 0;
    return (this.tournament.teams.length / this.tournament.maxTeams) * 100;
  }

  // Team management methods
  openAddTeamModal() {
    this.isAddTeamModalOpen = true;
  }

  closeAddTeamModal() {
    this.isAddTeamModalOpen = false;
  }

  handleAddTeam(team: Team) {
    if (!this.tournament) return;

    this.tournament = {
      ...this.tournament,
      teams: [...this.tournament.teams, team],
    };
    this.toast.success('Team added successfully');
  }

  handleEditTeam(team: Team) {
    this.editingTeam = team;
    this.isEditTeamModalOpen = true;
  }

  closeEditTeamModal() {
    this.isEditTeamModalOpen = false;
    this.editingTeam = null;
  }

  handleUpdateTeam(team: Team) {
    if (!this.tournament) return;

    this.tournament = {
      ...this.tournament,
      teams: this.tournament.teams.map((t) => (t.id === team.id ? team : t)),
    };
    this.toast.success('Team updated successfully');
  }

  openDeleteConfirmation(teamId: string) {
    this.teamToDelete = teamId;
    this.isDeleteTeamConfirmOpen = true;
  }

  closeDeleteConfirmation() {
    this.isDeleteTeamConfirmOpen = false;
    this.teamToDelete = null;
  }

  confirmDeleteTeam() {
    if (!this.tournament || !this.teamToDelete) return;

    this.tournament = {
      ...this.tournament,
      teams: this.tournament.teams.filter(
        (team) => team.id !== this.teamToDelete
      ),
    };
    this.toast.success('Team removed from tournament');
    this.closeDeleteConfirmation();
  }

  // Tournament actions
  generateSchedule() {
    this.toast.info('Schedule generation is not implemented yet');
  }

  createBracket() {
    this.toast.info('Bracket creation is not implemented yet');
  }

  cancelTournament() {
    // Would open a confirmation dialog in a real app
    this.toast.warn('Tournament cancellation is not implemented yet');
  }
}
