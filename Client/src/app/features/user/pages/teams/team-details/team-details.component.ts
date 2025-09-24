import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Team, Player, CreatePlayer, UpdatePlayer } from '@/app/api/models/team.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-team-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    LucideAngularModule
  ],
  templateUrl: './team-details.component.html',
  styleUrl: './team-details.component.css'
})
export class TeamDetailsComponent implements OnInit {
  private teamService = inject(TeamService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  
  readonly icons = LucideIcons;
  
  team = signal<Team | null>(null);
  players = signal<Player[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  showAddPlayerModal = signal(false);
  editingPlayer = signal<Player | null>(null);
  
  addPlayerForm: FormGroup;
  editPlayerForm: FormGroup;

  constructor() {
    this.addPlayerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      position: ['', [Validators.maxLength(30)]],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      photoUrl: [null]
    });

    this.editPlayerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      position: ['', [Validators.maxLength(30)]],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      photoUrl: [null]
    });
  }

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
        console.error('Failed to load team:', error);
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
        console.error('Failed to load players:', error);
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
    this.addPlayerForm.reset();
    this.showAddPlayerModal.set(true);
  }

  onCloseAddPlayerModal() {
    this.showAddPlayerModal.set(false);
  }

  onAddPlayer() {
    if (this.addPlayerForm.valid && this.team()) {
      const formValue = this.addPlayerForm.value;
      const playerData: CreatePlayer = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        position: formValue.position || undefined,
        email: formValue.email || undefined,
        photoUrl: formValue.photoUrl || undefined,
        teamId: this.team()!.id
      };

      this.teamService.addPlayerToTeam(this.team()!.id, playerData).subscribe({
        next: (newPlayer) => {
          this.players.update(players => [...players, newPlayer]);
          this.toastService.success('Player added successfully');
          this.showAddPlayerModal.set(false);
          this.addPlayerForm.reset();
        },
        error: (error) => {
          console.error('Failed to add player:', error);
          this.toastService.error('Failed to add player');
        }
      });
    }
  }

  onEditPlayer(player: Player) {
    this.editingPlayer.set(player);
    this.editPlayerForm.patchValue({
      firstName: player.firstName,
      lastName: player.lastName,
      position: player.position || '',
      email: player.email || '',
      photoUrl: null
    });
  }

  onCloseEditPlayerModal() {
    this.editingPlayer.set(null);
  }

  onUpdatePlayer() {
    const player = this.editingPlayer();
    if (this.editPlayerForm.valid && player && this.team()) {
      const formValue = this.editPlayerForm.value;
      const updateData: UpdatePlayer = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        position: formValue.position || undefined,
        email: formValue.email || undefined,
        photoUrl: formValue.photoUrl || undefined
      };

      this.teamService.updateTeamPlayer(this.team()!.id, player.id, updateData).subscribe({
        next: (updatedPlayer) => {
          this.players.update(players => 
            players.map(p => p.id === player.id ? updatedPlayer : p)
          );
          this.toastService.success('Player updated successfully');
          this.editingPlayer.set(null);
        },
        error: (error) => {
          console.error('Failed to update player:', error);
          this.toastService.error('Failed to update player');
        }
      });
    }
  }

  onRemovePlayer(player: Player) {
    if (this.team() && confirm(`Are you sure you want to remove ${player.firstName} ${player.lastName} from the team?`)) {
      this.teamService.removePlayerFromTeam(this.team()!.id, player.id).subscribe({
        next: () => {
          this.players.update(players => players.filter(p => p.id !== player.id));
          this.toastService.success('Player removed successfully');
        },
        error: (error) => {
          console.error('Failed to remove player:', error);
          this.toastService.error('Failed to remove player');
        }
      });
    }
  }

  onFileSelect(event: Event, formControl: string) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      if (formControl === 'add') {
        this.addPlayerForm.patchValue({ photoUrl: file });
      } else if (formControl === 'edit') {
        this.editPlayerForm.patchValue({ photoUrl: file });
      }
    }
  }

}