import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TournamentFormComponent } from '../components/tournament-form/tournament-form.component';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Tournament, UpdateTournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-edit-tournament',
  standalone: true,
  imports: [CommonModule, TournamentFormComponent, LucideAngularModule],
  templateUrl: './edit-tournament.component.html',
  styleUrl: './edit-tournament.component.css'
})
export class EditTournamentComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private tournamentService = inject(TournamentService);

  tournament = signal<Tournament | null>(null);
  loading = signal(true);
  saving = signal(false);
  icons = LucideIcons;
  tournamentId = signal<string>('');

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.tournamentId.set(params['id']);
      this.loadTournament();
    });
  }

  loadTournament() {
    if (!this.tournamentId()) return;
    
    this.loading.set(true);
    
    this.tournamentService.getTournament(this.tournamentId()).subscribe({
      next: (tournament) => {
        this.tournament.set(tournament);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Failed to load tournament:', error);
        this.loading.set(false);
      }
    });
  }

  onUpdate(updateData: UpdateTournament) {
    this.saving.set(true);
    
    this.tournamentService.updateTournament(this.tournamentId(), updateData).subscribe({
      next: (updatedTournament) => {
        this.saving.set(false);
        // Navigate to tournament details page
        this.router.navigate(['..'], { relativeTo: this.route });
      },
      error: (error) => {
        console.error('Failed to update tournament:', error);
        this.saving.set(false);
        alert('Failed to update tournament. Please try again.');
      }
    });
  }

  goBack() {
    this.router.navigate(['..'], { relativeTo: this.route });
  }
}
