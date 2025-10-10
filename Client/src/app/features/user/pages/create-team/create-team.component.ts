import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TeamService } from '@/app/api/services/team.service';
import { CreateTeam, UpdateTeam } from '@/app/api/models/team.models';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from 'lucide-angular';
import { TeamFormComponent } from '../../../../shared/components/forms/team-form/team-form.component';

@Component({
  selector: 'app-create-team',
  imports: [
    CommonModule,
    TeamFormComponent,
    LucideAngularModule
  ],
  templateUrl: './create-team.component.html',
  styleUrl: './create-team.component.css'
})
export class CreateTeamComponent {
  private teamService = inject(TeamService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  saving = signal(false);
  icons = LucideIcons;

  onSubmit(teamData: CreateTeam | UpdateTeam) {
    this.saving.set(true);
    
    const createTeamData = teamData as CreateTeam;
    
    this.teamService.createTeam(createTeamData).subscribe({
      next: (team) => {
        this.toastService.success('Team created successfully!');
        this.router.navigate(['/user/teams']);
        this.saving.set(false);
      },
      error: (error) => {
        this.toastService.error('Failed to create team. Please try again.');
        this.saving.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/user/teams']);
  }
}