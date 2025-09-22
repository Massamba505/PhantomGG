import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TeamFormComponent } from '../components/team-form/team-form.component';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { CreateTeam, UpdateTeam, Team } from '@/app/api/models/team.models';

@Component({
  selector: 'app-edit-team',
  standalone: true,
  imports: [CommonModule, TeamFormComponent],
  template: `
    <div class="container mx-auto px-4 py-8">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <div class="flex items-center gap-4">
          <button 
            (click)="goBack()" 
            class="p-2 rounded-lg bg-gray-800 hover:bg-gray-700 text-white transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
            </svg>
          </button>
          <h1 class="text-3xl font-bold text-white">Edit Team</h1>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="isLoading()" class="flex justify-center items-center py-12">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>

      <!-- Team Form -->
      <div *ngIf="!isLoading() && team()" class="max-w-2xl mx-auto">
        <app-team-form 
          [team]="team()"
          [loadingState]="isSaving()"
          (formUpdate)="handleFormUpdate($event)"
          (formCancel)="goBack()">
        </app-team-form>
      </div>

      <!-- Error State -->
      <div *ngIf="!isLoading() && !team()" class="text-center py-12">
        <h2 class="text-xl font-semibold text-red-400 mb-4">Team Not Found</h2>
        <p class="text-gray-400">The team you're looking for doesn't exist or you don't have permission to edit it.</p>
        <button 
          (click)="goBack()" 
          class="mt-4 px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          Go Back
        </button>
      </div>
    </div>
  `
})
export class EditTeamComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);

  team = signal<Team | null>(null);
  isLoading = signal(true);
  isSaving = signal(false);
  teamId: string | null = null;

  ngOnInit() {
    this.teamId = this.route.snapshot.paramMap.get('id');
    if (this.teamId) {
      this.loadTeam();
    } else {
      this.isLoading.set(false);
    }
  }

  private loadTeam() {
    if (!this.teamId) return;

    this.teamService.getTeam(this.teamId).subscribe({
      next: (team) => {
        this.team.set(team);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading team:', error);
        this.toastService.error('Failed to load team details');
        this.isLoading.set(false);
      }
    });
  }

  handleFormUpdate(teamData: UpdateTeam) {
    if (this.isSaving() || !this.teamId) return;

    this.isSaving.set(true);
    
    this.teamService.updateTeam(this.teamId, teamData).subscribe({
      next: (updatedTeam) => {
        this.toastService.success('Team updated successfully!');
        this.router.navigate(['/user/teams']);
      },
      error: (error) => {
        console.error('Error updating team:', error);
        this.toastService.error('Failed to update team. Please try again.');
        this.isSaving.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/user/teams']);
  }
}