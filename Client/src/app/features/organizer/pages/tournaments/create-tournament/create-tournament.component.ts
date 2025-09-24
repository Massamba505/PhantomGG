import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { TournamentFormComponent } from '../components/tournament-form/tournament-form.component';
import { TournamentService } from '@/app/api/services/tournament.service';
import { CreateTournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LucideAngularModule } from "lucide-angular";

@Component({
  selector: 'app-create-tournament',
  imports: [
    CommonModule,
    TournamentFormComponent,
    LucideAngularModule
],
  templateUrl: './create-tournament.component.html',
  styleUrl: './create-tournament.component.css'
})
export class CreateTournamentComponent {
  private tournamentService = inject(TournamentService);
  private router = inject(Router);

  saving = signal(false);
  icons = LucideIcons;

  onSubmit(tournamentData: CreateTournament) {
    this.saving.set(true);
    
    console.log('Tournament data being sent:', tournamentData);
    
    this.tournamentService.createTournament(tournamentData).subscribe({
      next: (tournament) => {
        this.router.navigate(['/organizer/tournaments', tournament.id]);
        this.saving.set(false);
      },
      error: (error) => {
        console.error('Error creating tournament:', error);
        this.saving.set(false);
      }
    });
  }

  goBack() {
    this.router.navigate(['/organizer/tournaments']);
  }
}
