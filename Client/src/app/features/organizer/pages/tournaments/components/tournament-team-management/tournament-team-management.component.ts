import { Component, input, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

type TeamTab = 'approved' | 'pending' | 'rejected';

@Component({
  selector: 'app-tournament-team-management',
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './tournament-team-management.component.html',
  styleUrl: './tournament-team-management.component.css'
})
export class TournamentTeamManagementComponent implements OnInit {
  tournamentId = input.required<string>();
  
  private tournamentService = inject(TournamentService);
  private toastService = inject(ToastService);
  
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
    
    // Load all approved teams
    this.tournamentService.getTournamentTeams(this.tournamentId()).subscribe({
      next: (teams) => {
        this.approvedTeams.set(teams.filter(team => team.status === 'Approved'));
        this.rejectedTeams.set(teams.filter(team => team.status === 'Rejected'));
      },
      error: (error) => {
        console.error('Failed to load tournament teams:', error);
        this.toastService.error('Failed to load tournament teams');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });

    // Load pending teams separately
    this.tournamentService.getPendingTeams(this.tournamentId()).subscribe({
      next: (teams) => {
        this.pendingTeams.set(teams);
      },
      error: (error) => {
        console.error('Failed to load pending teams:', error);
        this.toastService.error('Failed to load pending teams');
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
      case 'rejected':
        return this.rejectedTeams();
      default:
        return [];
    }
  }

  approveTeam(team: TournamentTeam) {
    this.setActionLoading(team.id, true);
    
    this.tournamentService.approveTeam(this.tournamentId(), team.id).subscribe({
      next: () => {
        this.toastService.success(`${team.name} has been approved`);
        this.loadTeams(); // Reload to get updated data
      },
      error: (error) => {
        console.error('Failed to approve team:', error);
        this.toastService.error('Failed to approve team');
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
        console.error('Failed to reject team:', error);
        this.toastService.error('Failed to reject team');
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
        console.error('Failed to remove team:', error);
        this.toastService.error('Failed to remove team');
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

  isTeamActionLoading(teamId: string): boolean {
    return this.isActionLoading()[teamId] || false;
  }

  getTeamCount(tab: TeamTab): number {
    switch (tab) {
      case 'approved':
        return this.approvedTeams().length;
      case 'pending':
        return this.pendingTeams().length;
      case 'rejected':
        return this.rejectedTeams().length;
      default:
        return 0;
    }
  }
}