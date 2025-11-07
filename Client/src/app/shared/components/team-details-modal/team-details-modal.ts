import {
  Component,
  OnInit,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { TeamService } from '@/app/api/services/team.service';
import { Team, Player } from '@/app/api/models/team.models';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';
import { PlayerPosition } from '@/app/api/models';

@Component({
  selector: 'app-team-details-modal',
  imports: [CommonModule, LucideAngularModule, Modal],
  templateUrl: './team-details-modal.html',
  styleUrl: './team-details-modal.css',
})
export class TeamDetailsModalComponent implements OnInit {
  private teamService = inject(TeamService);

  isOpen = input<boolean>(false);
  teamId = input<string>('');
  close = output<void>();

  team = signal<Team | null>(null);
  loading = signal(false);

  icons = LucideIcons;

  getPosition(position: number) {
    return getEnumLabel(PlayerPosition, position);
  }

  ngOnInit() {
    this.loadTeamDetails();
  }

  ngOnChanges() {
    if (this.isOpen() && this.teamId()) {
      this.loadTeamDetails();
    }
  }

  loadTeamDetails() {
    if (!this.teamId() || !this.isOpen()) return;

    this.loading.set(true);

    this.teamService.getTeam(this.teamId()).subscribe({
      next: (team) => {
        this.team.set(team);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading team:', error);
        this.loading.set(false);
      },
    });
  }

  onClose() {
    this.close.emit();
    this.team.set(null);
  }

  formatPlayerName(player: Player): string {
    return `${player.firstName} ${player.lastName}`;
  }

  getPlayerInitials(player: Player): string {
    return `${player.firstName.charAt(0)}${player.lastName.charAt(0)}`;
  }
}
