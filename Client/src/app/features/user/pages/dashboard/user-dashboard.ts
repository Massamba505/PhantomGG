import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamService } from '@/app/api/services/team.service';
import { TournamentService } from '@/app/api/services/tournament.service';
import { MatchService } from '@/app/api/services/match.service';
import { TeamDto } from '@/app/api/models/team.models';
import { TournamentDto } from '@/app/api/models/tournament.models';
import { MatchDto } from '@/app/api/models/match.models';
import { MatchStatus } from '@/app/api/models';
import { forkJoin } from 'rxjs';

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
  private readonly teamService = inject(TeamService);
  private readonly tournamentService = inject(TournamentService);
  private readonly matchService = inject(MatchService);

  readonly icons = LucideIcons;

  // State signals
  isLoading = signal(true);
  myTeams = signal<TeamDto[]>([]);
  activeTournaments = signal<TournamentDto[]>([]);
  upcomingMatches = signal<MatchDto[]>([]);
  recentMatches = signal<MatchDto[]>([]);

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.isLoading.set(true);

    forkJoin({
      teams: this.teamService.getTeams({ page: 1, pageSize: 6 }),
      tournaments: this.tournamentService.getMyTournaments({ page: 1, pageSize: 6 }),
      upcomingMatches: this.matchService.getMyMatches({ 
        page: 1, 
        pageSize: 5, 
        status: 'Scheduled'
      }),
      recentMatches: this.matchService.getMyMatches({ 
        page: 1, 
        pageSize: 5, 
        status: 'Completed'
      })
    }).subscribe({
      next: (data) => {
        this.myTeams.set(data.teams.data);
        this.activeTournaments.set(data.tournaments.data);
        this.upcomingMatches.set(data.upcomingMatches.data);
        this.recentMatches.set(data.recentMatches.data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        this.isLoading.set(false);
      }
    });
  }

  getMatchScore(match: MatchDto): string {
    if (match.status === MatchStatus.Completed) {
      return `${match.homeScore ?? 0} - ${match.awayScore ?? 0}`;
    }
    return 'vs';
  }

  getMatchDate(date: string): Date {
    return new Date(date);
  }

  getTournamentStatus(tournament: TournamentDto): string {
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    const endDate = new Date(tournament.endDate);
    const registrationDeadline = tournament.registrationDeadline 
      ? new Date(tournament.registrationDeadline) 
      : null;

    if (registrationDeadline && now < registrationDeadline) {
      return 'Registration Open';
    } else if (now >= startDate && now <= endDate) {
      return 'In Progress';
    } else if (now < startDate) {
      return 'Upcoming';
    } else {
      return 'Completed';
    }
  }
}
