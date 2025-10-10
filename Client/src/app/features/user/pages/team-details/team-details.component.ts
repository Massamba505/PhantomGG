import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { AuthStateService } from '@/app/store/AuthStateService';
import { Team, Player, CreatePlayer, UpdatePlayer } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { PlayerCard, PlayerRole } from '@/app/shared/components/cards/player-card/player-card';
import { PlayerForm } from '@/app/shared/components/forms/player-form/player-form';

@Component({
  selector: 'app-team-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LucideAngularModule,
    Modal,
    PlayerCard,
    PlayerForm
  ],
  templateUrl: './team-details.component.html',
  styleUrl: './team-details.component.css'
})
export class TeamDetailsComponent implements OnInit {
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private authStateService = inject(AuthStateService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  readonly icons = LucideIcons;
  
  team = signal<Team | null>(null);
  players = signal<Player[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showPlayerModal = signal(false);
  editingPlayer = signal<Player | null>(null);

  ngOnInit() {
    const teamId = this.route.snapshot.params['id'];
    if (teamId) {
      this.loadTeamDetails(teamId);
      this.loadTeamPlayers(teamId);
    }
  }

  loadTeamDetails(teamId: string) {
    this.loading.set(true);
    this.teamService.getTeam(teamId).subscribe({
      next: (team) => {
        this.team.set(team);
        this.error.set(null);
      },
      error: (error) => {
        this.error.set('Failed to load team details');
        this.toastService.error('Failed to load team details');
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }

  loadTeamPlayers(teamId: string) {
    this.teamService.getTeamPlayers(teamId).subscribe({
      next: (players) => {
        this.players.set(players);
      },
      error: (error) => {
        this.toastService.error('Failed to load team players');
      }
    });
  }

  onBackToTeams() {
    this.router.navigate(['/user/teams']);
  }

  onEditTeam() {
    const team = this.team();
    if (team) {
      this.router.navigate(['/user/teams', team.id, 'edit']);
    }
  }

  onShowAddPlayer() {
    if (!this.team()) {
      this.toastService.error('Please wait for team details to load');
      return;
    }
    this.editingPlayer.set(null);
    this.showPlayerModal.set(true);
  }

  onEditPlayer(player: Player) {
    this.editingPlayer.set(player);
    this.showPlayerModal.set(true);
  }

  onClosePlayerModal() {
    this.showPlayerModal.set(false);
    this.editingPlayer.set(null);
  }

  onPlayerSave(playerData: CreatePlayer | UpdatePlayer) {
    const team = this.team();
    if (!team) return;

    const isEdit = !!this.editingPlayer();
    
    if (isEdit) {
      const player = this.editingPlayer()!;
      this.teamService.updateTeamPlayer(team.id, player.id, playerData as UpdatePlayer).subscribe({
        next: (updatedPlayer) => {
          this.players.update(players => 
            players.map(p => p.id === player.id ? updatedPlayer : p)
          );
          this.toastService.success('Player updated successfully');
          this.onClosePlayerModal();
        },
        error: (error) => {
          this.toastService.error('Failed to update player');
        }
      });
    } else {
      this.teamService.addPlayerToTeam(team.id, playerData as CreatePlayer).subscribe({
        next: (newPlayer) => {
          this.players.update(players => [...players, newPlayer]);
          this.toastService.success('Player added successfully');
          this.onClosePlayerModal();
        },
        error: (error) => {
          this.toastService.error('Failed to add player');
        }
      });
    }
  }

  onRemovePlayer(playerId: string) {
    const player = this.players().find(p => p.id === playerId);
    if (!player || !this.team()) return;
    
    if (confirm(`Are you sure you want to remove ${player.firstName} ${player.lastName} from the team?`)) {
      this.teamService.removePlayerFromTeam(this.team()!.id, player.id).subscribe({
        next: () => {
          this.players.update(players => players.filter(p => p.id !== player.id));
          this.toastService.success('Player removed successfully');
        },
        error: (error) => {
          this.toastService.error('Failed to remove player');
        }
      });
    }
  }

  isManager(): boolean {
    const user = this.authStateService.user();
    const team = this.team();
    return !!(user && team && user.id === team.userId);
  }

  getPlayerRole(): PlayerRole {
    return this.isManager() ? 'Manager' : 'TeamMember';
  }

  retryLoad() {
    const teamId = this.route.snapshot.params['id'];
    if (teamId) {
      this.loadTeamDetails(teamId);
      this.loadTeamPlayers(teamId);
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  getPlayerInitials(player: Player): string {
    return `${player.firstName.charAt(0)}${player.lastName.charAt(0)}`.toUpperCase();
  }
}