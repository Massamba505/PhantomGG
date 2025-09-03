import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentService } from '@/app/core/services/tournament.service';
import { Tournament, UpdateTournamentRequest } from '@/app/shared/models/tournament';
import { TournamentForm } from '@/app/shared/components/tournament-form/tournament-form';

@Component({
  selector: 'app-edit-tournament',
  templateUrl: "./edit-tournament.html",
  standalone: true,
  imports: [CommonModule, TournamentForm],
})
export class EditTournament implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);
  private tournamentService = inject(TournamentService);

  tournament: Tournament | null = null;
  isLoading = true;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadTournament(id);
    } else {
      this.toast.error('Tournament ID is required');
      this.router.navigate(['/tournaments']);
    }
  }

  private async loadTournament(id: string) {
    try {
      this.isLoading = true;
      const response = await this.tournamentService.getTournamentById(id).toPromise();
      
      if (response?.success && response.data) {
        this.tournament = response.data;
      } else {
        this.toast.error('Tournament not found');
        this.router.navigate(['/tournaments']);
      }
    } catch (error) {
      console.error('Failed to load tournament:', error);
      this.toast.error('Failed to load tournament');
      this.router.navigate(['/tournaments']);
    } finally {
      this.isLoading = false;
    }
  }

  async onTournamentSaved(tournamentData: UpdateTournamentRequest) {
    if (!this.tournament) return;

    try {
      console.log('Tournament update data:', tournamentData);
      
      const response = await this.tournamentService.updateTournament(this.tournament.id, tournamentData).toPromise();
      
      if (response?.success) {
        this.toast.success('Tournament updated successfully!');
        this.router.navigate(['/tournaments']);
      } else {
        this.toast.error('Failed to update tournament');
      }
    } catch (error) {
      console.error('Failed to update tournament:', error);
      this.toast.error('Failed to update tournament');
    }
  }

  onCancel() {
    this.router.navigate(['/tournaments']);
  }
}
