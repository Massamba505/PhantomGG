import { Component, input, signal, OnInit, inject, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TeamRegistrationStatus, UserRoles } from '@/app/api/models/common.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamRole, TeamCardType } from '@/app/shared/components/cards/team-card/team-card';

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
  
  readonly icons = LucideIcons;
  teamView = output<string>();
  
  activeTab = signal<TeamTab>('approved');
  approvedTeams = signal<TournamentTeam[]>([]);
  pendingTeams = signal<TournamentTeam[]>([]);
  
  isLoading = signal(false);
  isActionLoading = signal<{ [key: string]: boolean }>({});

  ngOnInit() {
    this.loadTeams();
  }

  loadTeams() {
    this.isLoading.set(true);
    
    this.tournamentService.getTournamentTeams(this.tournamentId(), TeamRegistrationStatus.Approved).subscribe({
      next: (teams) => {
        this.approvedTeams.set(teams);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });

    this.tournamentService.getTournamentTeams(this.tournamentId(), TeamRegistrationStatus.Pending).subscribe({
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
        this.loadTeams();
      },
      error: () => {
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
  
  getTabClass(tab: TeamTab): string {
    const baseClass = 'px-2 py-1 font-semibold border-b-2 cursor-pointer sm:text-md text-xs ';
    const activeClass = 'border-primary text-primary';
    const inactiveClass = 'border-transparent text-muted';
    
    return baseClass + (this.activeTab() === tab ? activeClass : inactiveClass);
  }

  convertToTeam(tournamentTeam: TournamentTeam) {
    return {
      id: tournamentTeam.id,
      name: tournamentTeam.name,
      shortName: tournamentTeam.shortName || '',
      logoUrl: tournamentTeam.logoUrl,
      userId: tournamentTeam.managerId || '',
      createdAt: tournamentTeam.registeredAt,
      updatedAt: undefined,
      countPlayers: tournamentTeam.players.length,
      players: tournamentTeam.players
    };
  }

  getTeamRole(): TeamRole {
    return 'Public';
  }

  getTeamCardType(): TeamCardType {
    const tab = this.activeTab();
    return tab === 'pending' ? 'pending' : 'approved';
  }

  onTeamView(team: any) {
    this.teamView.emit(team.id);
  }
}