import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Team, TeamSearch } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamCardConfig } from '@/app/shared/components/cards/team-card/team-card';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-user-teams',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TeamCard
  ],
  templateUrl: './user-teams.html',
  styleUrl: './user-teams.css'
})
export class UserTeams implements OnInit {
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private authStateStore = inject(AuthStateService);
  
  readonly icons = LucideIcons;
  
  teams = signal<Team[]>([]);
  isLoading = signal(false);
  searchCriteria = signal<Partial<TeamSearch>>({});

  getTeamCardConfig(): TeamCardConfig {
    const Manager = this.authStateStore.user();
    return {
      isPublicView: false,
      Manager: Manager,
      type: 'viewOnly'
    };
  }

  ngOnInit() {
    this.loadMyTeams();
  }

  loadMyTeams() {
    this.isLoading.set(true);
    
    this.teamService.getMyTeams().subscribe({
      next: (teams) => {
        this.teams.set(teams);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  onSearchChange(searchCriteria: Partial<TeamSearch>) {
    this.searchCriteria.set(searchCriteria);
    this.searchTeams();
  }

  onClearSearch() {
    this.searchCriteria.set({});
    this.loadMyTeams();
  }

  private searchTeams() {
    this.isLoading.set(true);
    
    this.teamService.getTeams(this.searchCriteria()).subscribe({
      next: (response) => {
        this.teams.set(response.data);
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  onTeamEdit(team: Team) {
    this.router.navigate(['/user/teams', team.id, 'edit']);
  }

  onTeamView(team: Team) {
    this.router.navigate(['/user/teams', team.id]);
  }

  onTeamDelete(teamId: string) {
    const team = this.teams().find(t => t.id === teamId);
    if (!team) return;

    if (confirm(`Are you sure you want to delete ${team.name}? This action cannot be undone.`)) {
      this.teamService.deleteTeam(teamId).subscribe({
        next: () => {
          this.toastService.success('Team deleted successfully');
          this.loadMyTeams();
        }
      });
    }
  }
}