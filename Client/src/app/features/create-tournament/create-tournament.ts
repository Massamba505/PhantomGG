import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentService } from '@/app/core/services/tournament.service';
import { CreateTournamentRequest } from '@/app/shared/models/tournament';
import { TournamentForm } from '@/app/shared/components/tournament-form/tournament-form';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.html',
  styleUrls: ['./create-tournament.css'],
  standalone: true,
  imports: [DashboardLayout, CommonModule, TournamentForm],
})
export class CreateTournament {
  private router = inject(Router);
  private toast = inject(ToastService);
  private tournamentService = inject(TournamentService);

  async onTournamentSaved(tournamentData: CreateTournamentRequest) {
    try {
      console.log('Creating tournament:', tournamentData);
      
      const response = await this.tournamentService.createTournament(tournamentData).toPromise();
      
      if (response?.success) {
        this.toast.success('Tournament created successfully!');
        this.router.navigate(['/tournaments']);
      } else {
        this.toast.error('Failed to create tournament');
      }
    } catch (error) {
      console.error('Failed to create tournament:', error);
      this.toast.error('Failed to create tournament');
    }
  }
}
