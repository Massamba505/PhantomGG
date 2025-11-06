import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';
import { TournamentService, TournamentStatus } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentTeamManagementComponent } from '../../tournament-team-management/tournament-team-management';
import { TournamentMatchManagementComponent } from './components/tournament-match-management/tournament-match-management';
import { MatchDetailsModalComponent } from '../../match-details-modal/match-details-modal';
import { TeamDetailsModalComponent } from '../../team-details-modal/team-details-modal';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';

@Component({
  selector: 'app-public-tournament-details',
  imports: [
    CommonModule,
    LucideAngularModule,
    LineBreaksPipe,
    TournamentTeamManagementComponent,
    TournamentMatchManagementComponent,
    MatchDetailsModalComponent,
    TeamDetailsModalComponent,
  ],
  templateUrl: './tournament-details.html',
  styleUrl: './tournament-details.css',
})
export class TournamentDetailsComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private tournamentService = inject(TournamentService);
  private toastService = inject(ToastService);

  tournament = signal<Tournament | null>(null);
  loading = signal(true);
  tournamentId = signal<string>('');
  icons = LucideIcons;

  showDeleteModal = signal(false);
  isDeleting = signal(false);

  // Modal states
  showMatchModal = signal(false);
  showTeamModal = signal(false);
  selectedMatchId = signal<string>('');
  selectedTeamId = signal<string>('');

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.tournamentId.set(params['id']);
      this.loadTournament();
    });
  }

  readonly DESCRIPTION_LIMIT = 300;

  showFullDescription = signal(false);
  isLongDescription = signal(false);
  displayedDescription = signal<string>('');

  toggleDescription() {
    this.showFullDescription.update((prev) => !prev);
    this.updateDisplayedDescription();
  }

  private updateDisplayedDescription() {
    const fullDesc = this.tournament()?.description || '';
    const plainText = fullDesc.replace(/<[^>]*>/g, '');

    this.isLongDescription.set(plainText.length > this.DESCRIPTION_LIMIT);

    if (this.showFullDescription()) {
      this.displayedDescription.set(fullDesc);
    } else {
      const shortText = plainText.slice(0, this.DESCRIPTION_LIMIT) + '...';
      this.displayedDescription.set(shortText);
    }
  }

  loadTournament() {
    if (!this.tournamentId()) return;

    this.loading.set(true);

    this.tournamentService.getTournament(this.tournamentId()).subscribe({
      next: (tournament: any) => {
        this.tournament.set(tournament);
        const banner = this.tournament()!.bannerUrl?.split(' ').join('+');
        const logo = this.tournament()!.logoUrl?.split(' ').join('+');
        this.tournament.update((current) => ({
          ...current!,
          bannerUrl: banner,
          logoUrl: logo,
        }));
        this.updateDisplayedDescription();
      },
      complete: () => {
        this.loading.set(false);
      },
    });
  }

  goBack() {
    window.history.back();
  }

  onViewStatistics() {
    if (this.tournamentId()) {
      this.router.navigate([
        '/public/tournaments',
        this.tournamentId(),
        'statistics',
      ]);
    }
  }

  getStatus() {
    return getEnumLabel(TournamentStatus, this.tournament()!.status);
  }

  // Modal handlers
  openMatchModal(matchId: string) {
    this.selectedMatchId.set(matchId);
    this.showMatchModal.set(true);
  }

  closeMatchModal() {
    this.showMatchModal.set(false);
    this.selectedMatchId.set('');
  }

  openTeamModal(teamId: string) {
    this.selectedTeamId.set(teamId);
    this.showTeamModal.set(true);
  }

  closeTeamModal() {
    this.showTeamModal.set(false);
    this.selectedTeamId.set('');
  }
}
