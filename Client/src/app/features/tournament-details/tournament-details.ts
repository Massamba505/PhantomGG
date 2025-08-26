import { Team, Tournament } from '@/app/shared/models/tournament';
import { Component, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';
import { TeamModal } from './components/modals/team-modal';
import { TeamCard } from '@/app/shared/components/team-card/team-card';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";

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
  // Modal states - converted to signals
  isAddTeamModalOpen = signal(false);
  isEditTeamModalOpen = signal(false);
  isDeleteTeamConfirmOpen = signal(false);

  readonly icons = LucideIcons;

  // Team management - converted to signals
  editingTeam = signal<Team | null>(null);
  teamToDelete = signal<Team | null>(null);

  // Active tab - converted to signal
  activeTab = signal<'teams' | 'schedule' | 'bracket' | 'results'>('teams');

  // Tournament data - converted to signal
  tournament = signal<Tournament | null>(null);

  // Computed signals for derived values
  tournamentDuration = computed(() => {
    const t = this.tournament();
    if (!t) return 0;
    return this.calculateDuration(t.startDate, t.endDate);
  });

  teamRegistrationProgress = computed(() => {
    const t = this.tournament();
    if (!t || !t.teams) return 0;
    return (t.teams.length / t.maxTeams) * 100;
  });

  teamsCount = computed(() => {
    const t = this.tournament();
    return t?.teams?.length || 0;
  });

  maxTeams = computed(() => {
    const t = this.tournament();
    return t?.maxTeams || 0;
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toast: ToastService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.tournament.set({
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
    });
  }

  // Tab management
  setActiveTab(tab: 'teams' | 'schedule' | 'bracket' | 'results') {
    this.activeTab.set(tab);
  }

  // Tournament utils
  private calculateDuration(startDate: string, endDate: string): number {
    const start = new Date(startDate);
    const end = new Date(endDate);
    return Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
  }

  getTournamentDuration(startDate: string, endDate: string): number {
    return this.calculateDuration(startDate, endDate);
  }

  getTeamRegistrationProgress(): number {
    return this.teamRegistrationProgress();
  }

  // Team management methods
  openAddTeamModal() {
    this.isAddTeamModalOpen.set(true);
  }

  closeAddTeamModal() {
    this.isAddTeamModalOpen.set(false);
  }

  handleAddTeam(team: Team) {
    const currentTournament = this.tournament();
    if (!currentTournament) return;

    this.tournament.update(t => t ? {
      ...t,
      teams: [...t.teams, team],
    } : null);
    this.toast.success('Team added successfully');
    this.closeAddTeamModal();
  }

  handleEditTeam(team: Team) {
    this.editingTeam.set(team);
    console.log(this.editingTeam());
    this.isEditTeamModalOpen.set(true);
  }

  closeEditTeamModal() {
    this.isEditTeamModalOpen.set(false);
    this.editingTeam.set(null);
  }

  handleUpdateTeam(team: Team) {
    const currentTournament = this.tournament();
    if (!currentTournament) return;

    this.tournament.update(t => t ? {
      ...t,
      teams: t.teams.map((existingTeam: Team) => 
        existingTeam.id === team.id ? team : existingTeam
      ),
    } : null);
    this.toast.success('Team updated successfully');
    this.closeEditTeamModal();
  }

  openDeleteConfirmation(teamId: string) {
    const currentTournament = this.tournament();
    const team = currentTournament?.teams.find((t: Team) => t.id === teamId);
    this.teamToDelete.set(team || null);
    this.isDeleteTeamConfirmOpen.set(true);
  }

  closeDeleteConfirmation() {
    this.isDeleteTeamConfirmOpen.set(false);
    this.teamToDelete.set(null);
  }

  confirmDeleteTeam() {
    const currentTournament = this.tournament();
    const teamToDelete = this.teamToDelete();
    if (!currentTournament || !teamToDelete) return;

    this.tournament.update(t => t ? {
      ...t,
      teams: t.teams.filter((team: Team) => team.id !== teamToDelete.id),
    } : null);
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
