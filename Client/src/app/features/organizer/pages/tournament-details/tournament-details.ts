import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';
import { TournamentService, TournamentStatus } from '@/app/api/services';
import { ConfirmDeleteModal } from '@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal';
import { ToastService } from '@/app/shared/services/toast.service';
import { TournamentTeamManagementComponent } from './components/tournament-team-management/tournament-team-management';
import { TournamentMatchManagementComponent } from './components/tournament-match-management/tournament-match-management';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';

@Component({
  selector: 'app-tournament-details',
  imports: [
    CommonModule,
    LucideAngularModule,
    LineBreaksPipe,
    ConfirmDeleteModal,
    TournamentTeamManagementComponent,
    TournamentMatchManagementComponent,
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

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.tournamentId.set(params['id']);
      this.loadTournament();
    });
  }

  readonly DESCRIPTION_LIMIT_LINES = 7;
  readonly DESCRIPTION_LIMIT_CHAR = 300;
  displayedDescription = signal<string>('');

  showFullDescription = signal(false);
  isLongDescription = signal(false);

  toggleDescription() {
    this.showFullDescription.update((prev) => !prev);
  }

  private updateDisplayedDescription() {
    const desc = this.tournament()?.description || '';
    const plainText = desc
      .replace(/<br\s*\/?>/gi, '\n')
      .replace(/<[^>]*>/g, '');

    const lines = plainText.split(/\r?\n/).filter((line) => line.trim() !== '');
    const isLong =
      lines.length > this.DESCRIPTION_LIMIT_LINES ||
      plainText.length > this.DESCRIPTION_LIMIT_CHAR;

    this.isLongDescription.set(isLong);
  }

  loadTournament() {
    if (!this.tournamentId()) return;

    this.loading.set(true);

    this.tournamentService.getTournament(this.tournamentId()).subscribe({
      next: (tournament: any) => {
        this.tournament.set(tournament);
        const banner = this.tournament()!.bannerUrl;
        const logo = this.tournament()!.logoUrl;
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

  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.style.display = 'none';
  }

  editTournament() {
    this.router.navigate(['..', this.tournamentId(), 'edit'], {
      relativeTo: this.route,
    });
  }

  goBack() {
    this.router.navigate(['/organizer/tournaments']);
  }

  onViewStatistics() {
    if (this.tournamentId()) {
      this.router.navigate([
        '/organizer/tournaments',
        this.tournamentId(),
        'statistics',
      ]);
    }
  }

  deleteTournament() {
    if (!this.tournament()) return;
    this.showDeleteModal.set(true);
  }

  closeDeleteModal() {
    this.showDeleteModal.set(false);
  }
  getStatus() {
    return getEnumLabel(TournamentStatus, this.tournament()!.status);
  }

  confirmDelete() {
    if (!this.tournament() || this.isDeleting()) return;

    this.isDeleting.set(true);

    this.tournamentService.deleteTournament(this.tournamentId()).subscribe({
      next: () => {
        this.toastService.success('Tournament deleted successfully');
        this.router.navigate(['/organizer/tournaments']);
      },
      error: (error: any) => {
        this.isDeleting.set(false);
      },
    });
  }
}
