import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { ToastService } from '@/app/shared/services/toast.service';
import { Tournament, TournamentFormData } from '@/app/shared/models/tournament';
import { TournamentForm } from "../create-tournament-form/create-tournament-form";

@Component({
  selector: 'app-edit-tournament',
  templateUrl: "./edit-tournament.html",
  imports: [DashboardLayout, CommonModule, TournamentForm],
})
export class EditTournament implements OnInit {
  tournament: Tournament | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private toast: ToastService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.loadTournament(id);
  }

  private loadTournament(id: string | null) {
    this.tournament = {
      id: id || '1',
      name: 'Summer Championship 2024',
      description: 'Annual summer tournament featuring the best teams.',
      location: 'Downtown Arena',
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
  }

  onTournamentSaved(tournamentData: TournamentFormData) {
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
