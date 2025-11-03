import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TournamentForm } from '../../../../shared/components/forms/tournament-form/tournament-form';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Tournament, UpdateTournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-edit-tournament',
  imports: [CommonModule, TournamentForm, LucideAngularModule],
  templateUrl: './edit-tournament.html',
  styleUrl: './edit-tournament.css'
})
export class EditTournamentPage implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private tournamentService = inject(TournamentService);

  tournament = signal<Tournament | null>(null);
  loading = signal(true);
  saving = signal(false);
  icons = LucideIcons;
  tournamentId = signal<string>('');

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
      next: (tournament) => {
        this.tournament.set(tournament);
      },
      complete:()=>{
        this.loading.set(false);
      },
    });
  }

  onUpdate(updateData: UpdateTournament) {
    this.saving.set(true);
    
    this.tournamentService.updateTournament(this.tournamentId(), updateData).subscribe({
      next: (updatedTournament) => {
        this.saving.set(false);
        this.router.navigate(['..'], { relativeTo: this.route });
      },
      complete:()=>{
        this.saving.set(false);
      },
    });
  }

  goBack() {
    this.router.navigate(['..'], { relativeTo: this.route });
  }
}
