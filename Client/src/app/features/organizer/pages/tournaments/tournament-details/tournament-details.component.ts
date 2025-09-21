import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';
import { TournamentService } from '@/app/api/services';
import { ConfirmDeleteModal } from '@/app/shared/components/ui/ConfirmDeleteModal/ConfirmDeleteModal';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-tournament-details',
  imports: [CommonModule, LucideAngularModule, LineBreaksPipe, ConfirmDeleteModal],
  templateUrl: './tournament-details.component.html',
  styleUrl: './tournament-details.component.css'
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
    this.route.params.subscribe(params => {
      this.tournamentId.set(params['id']);
      this.loadTournament();
    });
  }

  loadTournament() {
    if (!this.tournamentId()) return;
    
    this.loading.set(true);
    
    this.tournamentService.getTournament(this.tournamentId()).subscribe({
      next: (tournament: any) => {
        this.tournament.set(tournament);
      },
      complete:()=>{
        this.loading.set(false);
      },
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
    
    this.tournamentService.deleteTournament(this.tournamentId()).subscribe({
      next: () => {
        this.toastService.success('Tournament deleted successfully');
        this.router.navigate(['/organizer/tournaments']);
      },
      error: (error: any) => {
        this.isDeleting.set(false);
      }
    });
  }
}
