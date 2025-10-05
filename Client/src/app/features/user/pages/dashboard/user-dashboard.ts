import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Team } from '@/app/api/models/team.models';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-user-dashboard',
  imports: [
    CommonModule,
    RouterModule,
    LucideAngularModule
  ],
  templateUrl: "./user-dashboard.html"
})
export class UserDashboard implements OnInit {
  private teamService = inject(TeamService);
  private tournamentService = inject(TournamentService);
  
  readonly icons = LucideIcons;
  
  myTeams = signal<Team[]>([]);
  availableTournaments = signal<Tournament[]>([]);

  ngOnInit() {
    this.loadMyTeams();
    this.loadAvailableTournaments();
  }

  loadMyTeams() {
    this.teamService.getTeams({ scope: 'my' }).subscribe({
      next: (response: any) => {
        this.myTeams.set(response.data);
      },
      error: (error: any) => {
        console.error('Failed to load teams:', error);
      }
    });
  }

  loadAvailableTournaments() {
    this.tournamentService.getTournaments({ pageSize: 10 }).subscribe({
      next: (response) => {
        this.availableTournaments.set(response.data.filter(t => t.status === 'Open'));
      },
      error: (error) => {
        console.error('Failed to load tournaments:', error);
      }
    });
  }
}
