import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Team, TeamSearch } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-user-teams',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule
  ],
  templateUrl: './user-teams.html',
  styleUrl: './user-teams.css'
})
export class UserTeams implements OnInit {
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  
  readonly icons = LucideIcons;
  
  teams = signal<Team[]>([]);
  isLoading = signal(false);
  searchCriteria = signal<Partial<TeamSearch>>({});

  ngOnInit() {
    this.loadMyTeams();
  }

  loadMyTeams() {
    this.isLoading.set(true);
    
    this.teamService.getMyTeams().subscribe({
      next: (teams) => {
        this.teams.set(teams);
      },
      error: (error) => {
        console.error('Failed to load teams:', error);
        this.toastService.error('Failed to load teams');
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
      error: (error) => {
        console.error('Failed to search teams:', error);
        this.toastService.error('Failed to search teams');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  onTeamAction(action: { type: string, team: Team }) {
    switch (action.type) {
      case 'edit':
        this.editTeam(action.team);
        break;
      case 'view':
        this.viewTeam(action.team);
        break;
      case 'delete':
        this.deleteTeam(action.team);
        break;
    }
  }

  editTeam(team: Team) {
    // Navigation will be handled by RouterModule
    console.log('Edit team:', team);
  }

  viewTeam(team: Team) {
    // Navigation will be handled by RouterModule  
    console.log('View team:', team);
  }

  deleteTeam(team: Team) {
    if (confirm(`Are you sure you want to delete ${team.name}? This action cannot be undone.`)) {
      this.teamService.deleteTeam(team.id).subscribe({
        next: () => {
          this.toastService.success('Team deleted successfully');
          this.loadMyTeams();
        },
        error: (error) => {
          console.error('Failed to delete team:', error);
          this.toastService.error('Failed to delete team');
        }
      });
    }
  }
}