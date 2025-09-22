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

@Component({
  selector: 'app-user-tournaments',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    LucideAngularModule,
    TeamSelectionModalComponent
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
  isJoining = signal<{ [key: string]: boolean }>({});
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
    
    this.setJoiningState(tournament.id, true);
    
    this.tournamentService.registerForTournament(tournament.id, {
      teamId: team.id
    }).subscribe({
      next: () => {
        this.toastService.success(`Successfully registered ${team.name} for ${tournament.name}`);
        this.loadTournaments(); // Refresh to update registration status
      },
      error: (error: any) => {
        console.error('Failed to join tournament:', error);
        this.toastService.error('Failed to join tournament. You may already be registered.');
      },
      complete: () => {
        this.setJoiningState(tournament.id, false);
      }
    });
  }

  onTeamSelectionModalClosed() {
    this.showTeamSelectionModal.set(false);
    this.selectedTournament.set(null);
  }

  onCreateTeamFromModal() {
    this.router.navigate(['/user/teams/create']);
  }

  private setJoiningState(tournamentId: string, joining: boolean) {
    this.isJoining.update(current => ({
      ...current,
      [tournamentId]: joining
    }));
  }

  isTournamentJoining(tournamentId: string): boolean {
    const tournament = this.selectedTournament();
    return tournament?.id === tournamentId && this.isJoining()[tournamentId] || false;
  }

  canJoinTournament(tournament: Tournament): boolean {
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    const registrationDeadline = tournament.registrationDeadline 
      ? new Date(tournament.registrationDeadline) 
      : startDate;

    return (
      tournament.status === 'RegistrationOpen' &&
      now < registrationDeadline &&
      tournament.teamCount < tournament.maxTeams &&
      this.myTeams().length > 0
    );
  }

  getRegistrationStatus(tournament: Tournament): string {
    const now = new Date();
    const startDate = new Date(tournament.startDate);
    const registrationDeadline = tournament.registrationDeadline 
      ? new Date(tournament.registrationDeadline) 
      : startDate;

    if (tournament.status !== 'RegistrationOpen') {
      return 'Registration Closed';
    }
    
    if (now >= registrationDeadline) {
      return 'Registration Expired';
    }
    
    if (tournament.teamCount >= tournament.maxTeams) {
      return 'Tournament Full';
    }
    
    if (this.myTeams().length === 0) {
      return 'Need Team to Join';
    }
    
    return 'Open for Registration';
  }
}