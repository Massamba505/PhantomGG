import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TeamFormComponent } from '../components/team-form/team-form.component';
import { TeamService } from '@/app/api/services/team.service';
import { CreateTeam } from '@/app/api/models/team.models';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from 'lucide-angular';

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

  onSubmit(teamData: CreateTeam) {
    this.saving.set(true);
    
    this.teamService.createTeam(teamData).subscribe({
      next: (team) => {
        this.toastService.success('Team created successfully!');
        this.router.navigate(['/user/teams']);
        this.saving.set(false);
      },
      error: (error) => {
        console.error('Failed to create team:', error);
        this.toastService.error('Failed to create team. Please try again.');
        this.saving.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/user/teams']);
  }
}