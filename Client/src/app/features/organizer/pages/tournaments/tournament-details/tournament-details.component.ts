import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { CurrencyFormatPipe } from '@/app/shared/pipe/currency-format.pipe';
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';
import { OrganizerService } from '@/app/api/services';
import { ConfirmDeleteModal } from '@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-tournament-details',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, CurrencyFormatPipe, LineBreaksPipe, ConfirmDeleteModal],
  templateUrl: './tournament-details.component.html',
  styleUrl: './tournament-details.component.css'
})
export class TournamentDetailsComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private organizerService = inject(OrganizerService);
  private toastService = inject(ToastService);

  tournament = signal<Tournament | null>(null);
  loading = signal(true);
  tournamentId = signal<string>('');
  icons = LucideIcons;
  
  showDeleteModal = signal(false);
  isDeleting = signal(false);

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.tournamentId.set(params['id']);
      this.loadTournament();
    });
  }

  loadTournament() {
    if (!this.tournamentId()) return;
    
    this.loading.set(true);
    
    this.organizerService.getTournamentDetails(this.tournamentId()).subscribe({
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

  editTournament() {
    this.router.navigate(['..', this.tournamentId(), 'edit'], { relativeTo: this.route });
  }

  goBack() {
    this.router.navigate(['../..'], { relativeTo: this.route });
  }

  deleteTournament() {
    if (!this.tournament()) return;
    this.showDeleteModal.set(true);
  }

  closeDeleteModal() {
    this.showDeleteModal.set(false);
  }

  confirmDelete() {
    if (!this.tournament() || this.isDeleting()) return;
    
    this.isDeleting.set(true);
    
    this.organizerService.deleteTournament(this.tournamentId()).subscribe({
      next: () => {
        this.toastService.success('Tournament deleted successfully');
        this.router.navigate(['/organizer/tournaments']);
      },
      error: (error) => {
        console.error('Failed to delete tournament:', error);
        this.isDeleting.set(false);
      }
    });
  }
}
