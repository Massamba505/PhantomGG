import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentFormData } from '@/app/shared/models/tournament';
import { TournamentForm } from '@/app/shared/components/tournament-form/tournament-form';

@Component({
  selector: 'app-create-tournament',
  templateUrl: './create-tournament.html',
  styleUrls: ['./create-tournament.css'],
  standalone: true,
  imports: [DashboardLayout, CommonModule, TournamentForm],
})
export class CreateTournament {
  constructor(
    private router: Router,
    private toast: ToastService
  ) {}

  onTournamentSaved(tournamentData: TournamentFormData) {
    console.log('Tournament data received:', tournamentData);
    
    setTimeout(() => {
      this.toast.success('Tournament created successfully!');
      this.router.navigate(['/tournaments']);
    }, 1500);
  }
}
