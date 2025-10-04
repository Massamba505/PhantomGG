import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TeamFormComponent } from '../../../../shared/components/forms/team-form/team-form.component';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { CreateTeam, UpdateTeam, Team } from '@/app/api/models/team.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-edit-team',
  standalone: true,
  imports: [CommonModule, TeamFormComponent, LucideAngularModule],
  templateUrl: "./edit-team.html"
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
  readonly icons = LucideIcons;

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

  handleFormUpdate(teamData: CreateTeam | UpdateTeam) {
    if (this.isSaving() || !this.teamId) return;

    this.isSaving.set(true);
    
    // For edit team component, we expect UpdateTeam data
    const updateData = teamData as UpdateTeam;
    
    this.teamService.updateTeam(this.teamId, updateData).subscribe({
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