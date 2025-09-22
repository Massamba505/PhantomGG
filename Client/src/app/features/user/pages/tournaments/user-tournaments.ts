import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentService } from '@/app/api/services/tournament.service';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Tournament, TournamentSearch } from '@/app/api/models/tournament.models';
import { Team } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamSelectionModalComponent } from './components/team-selection-modal/team-selection-modal.component';
import { TournamentCard } from '@/app/shared/components/cards/tournament-card/tournament-card';

@Component({
  selector: 'app-user-tournaments',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TeamSelectionModalComponent,
    TournamentCard
  ],
  templateUrl: './user-tournaments.html',
  styleUrl: './user-tournaments.css'
})
export class UserTournaments implements OnInit {
  private tournamentService = inject(TournamentService);
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  
  readonly icons = LucideIcons;
  
  tournaments = signal<Tournament[]>([]);
  myTeams = signal<Team[]>([]);
  isLoading = signal(false);
  isJoiningTournament = signal(false);
  searchCriteria = signal<Partial<TournamentSearch>>({});
  
  showTeamSelectionModal = signal(false);
  selectedTournament = signal<Tournament | null>(null);

  ngOnInit() {
    this.loadTournaments();
    this.loadMyTeams();
  }

  loadTournaments() {
    this.isLoading.set(true);
    
    this.tournamentService.getTournaments(this.searchCriteria()).subscribe({
      next: (response) => {
        this.tournaments.set(response.data);
      },
      error: (error) => {
        console.error('Failed to load tournaments:', error);
        this.toastService.error('Failed to load tournaments');
      },
      complete: () => {
        this.isLoading.set(false);
      }
    });
  }

  loadMyTeams() {
    this.teamService.getMyTeams().subscribe({
      next: (teams) => {
        this.myTeams.set(teams);
      },
      error: (error) => {
        console.error('Failed to load teams:', error);
      }
    });
  }

  onSearchChange(searchCriteria: Partial<TournamentSearch>) {
    this.searchCriteria.set(searchCriteria);
    this.loadTournaments();
  }

  joinTournament(tournament: Tournament) {
    const teams = this.myTeams();
    if (teams.length === 0) {
      this.toastService.error('You need to create a team first before joining tournaments');
      return;
    }

    this.selectedTournament.set(tournament);
    this.showTeamSelectionModal.set(true);
  }

  onTeamSelected(team: Team) {
    const tournament = this.selectedTournament();
    if (!tournament) return;
    
    this.isJoiningTournament.set(true);
    
    this.tournamentService.registerForTournament(tournament.id, {
      teamId: team.id
    }).subscribe({
      next: () => {
        this.toastService.success(`Successfully registered ${team.name} for ${tournament.name}`);
        this.loadTournaments();
        this.closeTeamSelectionModal();
      },
      error: (error: any) => {
        this.isJoiningTournament.set(false);
      },
      complete: () => {
        this.isJoiningTournament.set(false);
      }
    });
  }

  onTeamSelectionModalClosed() {
    this.closeTeamSelectionModal();
  }

  private closeTeamSelectionModal() {
    this.showTeamSelectionModal.set(false);
    this.selectedTournament.set(null);
    this.isJoiningTournament.set(false);
  }

  onCreateTeamFromModal() {
    this.router.navigate(['/user/teams/create']);
  }

  canJoinTournament(tournament: Tournament): boolean {
    return (
      tournament.status === 'RegistrationOpen' &&
      tournament.teamCount < tournament.maxTeams &&
      this.myTeams().length > 0
    );
  }

  getRegistrationStatus(tournament: Tournament): string {
    return tournament.status;
  }

  // Tournament card event handlers
  onTournamentJoin(tournament: Tournament) {
    this.joinTournament(tournament);
  }

  onTournamentView(tournament: Tournament) {
    this.router.navigate(['/user/tournaments', tournament.id]);
  }

  onTournamentLeave(tournament: Tournament) {
    // Implementation for leaving tournament if needed
    console.log('Leave tournament:', tournament);
  }
}