import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ToastService } from '@/app/shared/services/toast.service';
import { Tournament, TournamentFormData } from '@/app/shared/models/tournament';
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

  private loadTournament(id: string) {
    setTimeout(() => {
      this.tournament = {
        id: id,
        name: 'Summer Championship 2024',
        description: 'Annual summer tournament featuring the best teams from across the region.',
        location: 'Downtown Sports Arena',
        registrationDeadline: '2024-06-01',
        startDate: '2024-06-15',
        endDate: '2024-06-30',
        maxTeams: 16,
        entryFee: 100,
        prizePool: 5000,
        contactEmail: 'contact@tournament.com',
        status: 'active',
        createdAt: '2024-05-01',
        teams: [],
        bannerUrl: 'https://example.com/banner.jpg'
      };
      this.isLoading = false;
    }, 500);
  }

  onTournamentSaved(tournamentData: TournamentFormData) {
    if (!this.tournament) return;

    console.log('Tournament update data:', tournamentData);
    
    
    setTimeout(() => {
      this.toast.success('Tournament updated successfully!');
      this.router.navigate(['/tournaments']);
    }, 1500);
  }

  onCancel() {
    this.router.navigate(['/tournaments']);
  }
}
