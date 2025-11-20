import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TournamentService } from '@/app/api/services/tournament.service';
import { MatchService } from '@/app/api/services/match.service';
import { TournamentDto } from '@/app/api/models/tournament.models';
import { TournamentTeamDto } from '@/app/api/models/team.models';
import { MatchDto } from '@/app/api/models/match.models';
import { TeamAction } from '@/app/api/models/common.models';
import { ToastService } from '@/app/shared/services/toast.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-organizer-dashboard',
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './organizer-dashboard.component.html',
})
export class OrganizerDashboardComponent implements OnInit {
  private readonly tournamentService = inject(TournamentService);
  private readonly matchService = inject(MatchService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  readonly icons = LucideIcons;

  isLoading = signal(true);
  myTournaments = signal<TournamentDto[]>([]);
  pendingApprovals = signal<TournamentTeamDto[]>([]);
  upcomingMatches = signal<MatchDto[]>([]);
  recentMatches = signal<MatchDto[]>([]);
  
  processingApproval = signal<string | null>(null);

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.isLoading.set(true);

    forkJoin({
      tournaments: this.tournamentService.getTournaments({ page: 1, pageSize: 6 }),
      pendingApprovals: this.tournamentService.getPendingApprovals(),
      upcomingMatches: this.matchService.getOrganizerMatches({ 
        page: 1, 
        pageSize: 5, 
        status: 'Scheduled'
      }),
      recentMatches: this.matchService.getOrganizerMatches({ 
        page: 1, 
        pageSize: 5, 
        status: 'Completed'
      })
    }).subscribe({
      next: (data) => {
        this.myTournaments.set(data.tournaments.data);
        this.pendingApprovals.set(data.pendingApprovals);
        this.upcomingMatches.set(data.upcomingMatches.data);
        this.recentMatches.set(data.recentMatches.data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading dashboard data:', error);
        this.toastService.error('Failed to load dashboard data');
        this.isLoading.set(false);
      }
    });
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

  approveTeam(approval: TournamentTeamDto, event: Event) {
    event.stopPropagation();
    const key = `${approval.tournamentId}-${approval.teamId}`;
    this.processingApproval.set(key);

    this.tournamentService.approveTeam(approval.tournamentId, approval.teamId).subscribe({
      next: () => {
        this.toastService.success(`${approval.name} approved successfully`);
        this.processingApproval.set(null);
        this.loadDashboardData();
      },
      error: (error) => {
        console.error('Error approving team:', error);
        this.toastService.error('Failed to approve team');
        this.processingApproval.set(null);
      }
    });
  }

  rejectTeam(approval: TournamentTeamDto, event: Event) {
    event.stopPropagation();
    const key = `${approval.tournamentId}-${approval.teamId}`;
    this.processingApproval.set(key);

    this.tournamentService.rejectTeam(approval.tournamentId, approval.teamId).subscribe({
      next: () => {
        this.toastService.success(`${approval.name} rejected`);
        this.processingApproval.set(null);
        this.loadDashboardData();
      },
      error: (error) => {
        console.error('Error rejecting team:', error);
        this.toastService.error('Failed to reject team');
        this.processingApproval.set(null);
      }
    });
  }

  viewTournament(tournamentId: string) {
    this.router.navigate(['/organizer/tournaments', tournamentId]);
  }
}
