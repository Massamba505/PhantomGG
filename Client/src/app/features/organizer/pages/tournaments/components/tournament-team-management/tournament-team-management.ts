import { Component, input, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamRole, TeamCardType } from '@/app/shared/components/cards/team-card/team-card';
import { AuthStateService } from '@/app/store/AuthStateService';

type TeamTab = 'approved' | 'pending';

@Component({
  selector: 'app-tournament-team-management',
  imports: [CommonModule, LucideAngularModule, TeamCard],
  templateUrl: './tournament-team-management.html',
  styleUrl: './tournament-team-management.css'
})
export class TournamentTeamManagementComponent implements OnInit {
  tournamentId = input.required<string>();
  
  private tournamentService = inject(TournamentService);
  private toastService = inject(ToastService);
  private authStateStore = inject(AuthStateService);
  
  readonly icons = LucideIcons;
  
  activeTab = signal<TeamTab>('approved');
  approvedTeams = signal<TournamentTeam[]>([]);
  pendingTeams = signal<TournamentTeam[]>([]);
  rejectedTeams = signal<TournamentTeam[]>([]);
  
  isLoading = signal(false);
  isActionLoading = signal<{ [key: string]: boolean }>({});

  ngOnInit() {
    this.loadTeams();
  }

  loadTeams() {
    this.isLoading.set(true);
    
    this.tournamentService.getTournamentTeams(this.tournamentId()).subscribe({
      next: (teams) => {
        this.approvedTeams.set(teams.filter(team => team.status === 'Approved'));
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });

    this.tournamentService.getPendingTeams(this.tournamentId()).subscribe({
      next: (teams) => {
        this.pendingTeams.set(teams);
      }
    });
  }

  setActiveTab(tab: TeamTab) {
    this.activeTab.set(tab);
  }

  getCurrentTeams(): TournamentTeam[] {
    const tab = this.activeTab();
    switch (tab) {
      case 'approved':
        return this.approvedTeams();
      case 'pending':
        return this.pendingTeams();
      default:
        return [];
    }
  }

  approveTeam(team: TournamentTeam) {
    this.setActionLoading(team.id, true);
    
    this.tournamentService.approveTeam(this.tournamentId(), team.id).subscribe({
      next: () => {
        this.toastService.success(`${team.name} has been approved`);
        this.loadTeams();
      },
      error: (error) => {
        this.setActionLoading(team.id, false);
      },
      complete: () => {
        this.setActionLoading(team.id, false);
      }
    });
  }

  rejectTeam(team: TournamentTeam) {
    this.setActionLoading(team.id, true);
    
    this.tournamentService.rejectTeam(this.tournamentId(), team.id).subscribe({
      next: () => {
        this.toastService.success(`${team.name} has been rejected`);
        this.loadTeams(); // Reload to get updated data
      },
      error: (error) => {
        this.setActionLoading(team.id, false);
      },
      complete: () => {
        this.setActionLoading(team.id, false);
      }
    });
  }

  removeTeam(team: TournamentTeam) {
    this.setActionLoading(team.id, true);
    
    this.tournamentService.removeTeam(this.tournamentId(), team.id).subscribe({
      next: () => {
        this.toastService.success(`${team.name} has been removed from the tournament`);
        this.loadTeams(); // Reload to get updated data
      },
      error: (error) => {
        this.setActionLoading(team.id, false);
      },
      complete: () => {
        this.setActionLoading(team.id, false);
      }
    });
  }

  private setActionLoading(teamId: string, loading: boolean) {
    this.isActionLoading.update(current => ({
      ...current,
      [teamId]: loading
    }));
  }

  getTeamCount(tab: TeamTab): number {
    switch (tab) {
      case 'approved':
        return this.approvedTeams().length;
      case 'pending':
        return this.pendingTeams().length;
      default:
        return 0;
    }
  }

  convertToTeam(tournamentTeam: TournamentTeam) {
    return {
      id: tournamentTeam.id,
      name: tournamentTeam.name,
      shortName: tournamentTeam.shortName,
      logoUrl: tournamentTeam.logoUrl,
      userId: tournamentTeam.managerId || '',
      createdAt: tournamentTeam.registeredAt,
      updatedAt: undefined
    };
  }

  getTeamRole(): TeamRole {
    return 'Organizer'; // Organizer can approve/reject/remove teams
  }

  getTeamCardType(): TeamCardType {
    const tab = this.activeTab();
    return tab === 'pending' ? 'pending' : 'approved';
  }

  onTeamView(team: any) {
    console.log('View team:', team);
  }

  onTeamDelete(teamId: string) {
    const team = this.getCurrentTeams().find(t => t.id === teamId);
    if (!team) return;

    if (this.activeTab() === 'approved') {
      this.removeTeam(team);
    } else if (this.activeTab() === 'pending') {
      this.rejectTeam(team);
    }
  }

  onTeamApprove(teamId: string) {
    const team = this.getCurrentTeams().find(t => t.id === teamId);
    if (team) {
      this.approveTeam(team);
    }
  }

  onTeamReject(teamId: string) {
    const team = this.getCurrentTeams().find(t => t.id === teamId);
    if (team) {
      this.rejectTeam(team);
    }
  }
}