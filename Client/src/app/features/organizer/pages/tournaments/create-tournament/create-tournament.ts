import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TournamentForm } from '../../../../../shared/components/forms/tournament-form/tournament-form';
import { TournamentService } from '@/app/api/services/tournament.service';
import { CreateTournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";

@Component({
  selector: 'app-create-tournament',
  imports: [
    CommonModule,
    TournamentForm,
    LucideAngularModule
  ],
  templateUrl: './create-tournament.html',
  styleUrl: './create-tournament.css'
})
export class CreateTournamentPage {
  private tournamentService = inject(TournamentService);
  private router = inject(Router);

  saving = signal(false);
  icons = LucideIcons;

  onSubmit(tournamentData: CreateTournament) {
    this.saving.set(true);
    
    this.tournamentService.createTournament(tournamentData).subscribe({
      next: (tournament) => {
        this.router.navigate(['/organizer/tournaments', tournament.id]);
        this.saving.set(false);
      },
      error: (error) => {
        this.saving.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/organizer/tournaments']);
  }
}
