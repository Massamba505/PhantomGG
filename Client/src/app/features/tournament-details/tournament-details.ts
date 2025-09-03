import { Team, Tournament } from '@/app/shared/models/tournament';
import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule, DatePipe, TitleCasePipe } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentService } from '@/app/core/services/tournament.service';
import { TeamService } from '@/app/core/services/team.service';
import { TeamModal } from './components/modals/team-modal';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";
import { ConfirmDeleteModal } from "@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal";
import {
  TournamentTabs,
  TeamsSection,
  ScheduleSection,
  BracketSection,
  ResultsSection
} from './components';
import type { TabType, TabItem } from './components';

@Component({
  selector: 'app-tournament-details',
  templateUrl: './tournament-details.html',
  styleUrls: ['./tournament-details.css'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    TeamModal,
    DatePipe,
    TitleCasePipe,
    LucideAngularModule,
    ConfirmDeleteModal,
    TournamentTabs,
    TeamsSection,
    ScheduleSection,
    BracketSection,
    ResultsSection
],
  providers: [ToastService],
})
export class TournamentDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);
  private tournamentService = inject(TournamentService);
  private teamService = inject(TeamService);

  isAddTeamModalOpen = signal(false);
  isEditTeamModalOpen = signal(false);
  isDeleteTeamConfirmOpen = signal(false);

  readonly icons = LucideIcons;

  editingTeam = signal<Team | null>(null);
  teamToDelete = signal<Team | null>(null);

  activeTab = signal<TabType>('teams');
  
  tabs: TabItem[] = [
    { key: 'teams' as TabType, label: 'Teams' },
    { key: 'schedule' as TabType, label: 'Schedule' },
    { key: 'bracket' as TabType, label: 'Bracket' },
    { key: 'results' as TabType, label: 'Results' }
  ];

  tournament = signal<Tournament | null>(null);
  teams = signal<Team[]>([]);
  loading = signal<boolean>(false);
  teamsLoading = signal<boolean>(false);
  scheduleLoading = signal<boolean>(false);
  bracketLoading = signal<boolean>(false);
  resultsLoading = signal<boolean>(false);

  tournamentDuration = computed(() => {
    const t = this.tournament();
    if (!t) return 0;
    return this.calculateDuration(t.startDate, t.endDate);
  });

  teamRegistrationProgress = computed(() => {
    const t = this.tournament();
    const teamsCount = this.teams().length;
    if (!t || teamsCount === 0) return 0;
    return (teamsCount / t.maxTeams) * 100;
  });

  teamsCount = computed(() => {
    return this.teams().length;
  });

  maxTeams = computed(() => {
    const t = this.tournament();
    return t?.maxTeams || 0;
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadTournamentData(id);
    }
  }

  private async loadTournamentData(tournamentId: string) {
    try {
      this.loading.set(true);
      this.teamsLoading.set(true);
      
      const [tournamentResponse, teamsResponse] = await Promise.all([
        this.tournamentService.getTournamentById(tournamentId).toPromise(),
        this.teamService.searchTeams({ tournamentId }).toPromise()
      ]);
      
      if (tournamentResponse?.success && tournamentResponse.data) {
        this.tournament.set(tournamentResponse.data);
      }
      
      if (teamsResponse?.success && teamsResponse.data) {
        this.teams.set(teamsResponse.data);
      }
      
    } catch (error) {
      console.error('Failed to load tournament data:', error);
      this.toast.error('Failed to load tournament details');
      this.router.navigate(['/tournaments']);
    } finally {
      this.loading.set(false);
      this.teamsLoading.set(false);
    }
  }

  onTabChange(tab: TabType) {
    this.activeTab.set(tab);
  }

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

  openAddTeamModal() {
    this.isAddTeamModalOpen.set(true);
  }

  closeAddTeamModal() {
    this.isAddTeamModalOpen.set(false);
  }

  async handleAddTeam(team: Team) {
    try {
      this.teamsLoading.set(true);
      const tournamentId = this.tournament()?.id;
      if (!tournamentId) return;

      const createRequest = {
        name: team.name,
        manager: team.manager,
        numberOfPlayers: team.numberOfPlayers,
        logoUrl: team.logoUrl,
        tournamentId: tournamentId
      };

      const response = await this.teamService.createTeam(createRequest).toPromise();
      
      if (response?.success && response.data) {
        const currentTeams = this.teams();
        this.teams.set([...currentTeams, response.data]);
        this.toast.success('Team added successfully');
      } else {
        this.toast.error('Failed to add team');
      }
    } catch (error) {
      console.error('Failed to add team:', error);
      this.toast.error('Failed to add team');
    } finally {
      this.teamsLoading.set(false);
    }
    
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

  async handleUpdateTeam(team: Team) {
    try {
      const updateRequest = {
        name: team.name,
        manager: team.manager,
        numberOfPlayers: team.numberOfPlayers,
        logoUrl: team.logoUrl
      };

      const response = await this.teamService.updateTeam(team.id, updateRequest).toPromise();
      
      if (response?.success && response.data) {
        const currentTeams = this.teams();
        const updatedTeams = currentTeams.map((existingTeam: Team) => 
          existingTeam.id === team.id ? response.data! : existingTeam
        );
        this.teams.set(updatedTeams);
        this.toast.success('Team updated successfully');
      } else {
        this.toast.error('Failed to update team');
      }
    } catch (error) {
      console.error('Failed to update team:', error);
      this.toast.error('Failed to update team');
    }

    this.closeEditTeamModal();
  }

  openDeleteConfirmation(teamId: string) {
    const teams = this.teams();
    const team = teams.find((t: Team) => t.id === teamId);
    this.teamToDelete.set(team || null);
    this.isDeleteTeamConfirmOpen.set(true);
  }

  closeDeleteConfirmation() {
    this.isDeleteTeamConfirmOpen.set(false);
    this.teamToDelete.set(null);
  }

  async confirmDeleteTeam() {
    const teamToDelete = this.teamToDelete();
    if (!teamToDelete) return;

    try {
      const response = await this.teamService.deleteTeam(teamToDelete.id).toPromise();
      
      if (response?.success) {
        const currentTeams = this.teams();
        const updatedTeams = currentTeams.filter((team: Team) => team.id !== teamToDelete.id);
        this.teams.set(updatedTeams);
        this.toast.success('Team removed from tournament');
      } else {
        this.toast.error('Failed to remove team');
      }
    } catch (error) {
      console.error('Failed to delete team:', error);
      this.toast.error('Failed to remove team');
    }

    this.closeDeleteConfirmation();
  }

  generateSchedule() {
    this.toast.info('Schedule generation is not implemented yet');
  }

  createBracket() {
    this.toast.info('Bracket creation is not implemented yet');
  }

  setActiveTab(tab: TabType) {
    this.activeTab.set(tab);
  }

  cancelTournament() {
    this.toast.warn('Tournament cancellation is not implemented yet');
  }
}
