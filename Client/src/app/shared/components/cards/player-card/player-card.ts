import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Player } from '@/app/api/models/team.models';

export type PlayerRole = 'Manager' | 'TeamMember' | 'Public';

@Component({
  selector: 'app-player-card',
  templateUrl: './player-card.html',
  styleUrls: ['./player-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class PlayerCard {
  player = input.required<Player>();
  role = input<PlayerRole>('Public');
  
  edit = output<Player>();
  delete = output<string>();
  view = output<Player>();
  
  readonly icons = LucideIcons;

  isManager(): boolean {
    return this.role() === 'Manager';
  }

  isTeamMember(): boolean {
    return this.role() === 'TeamMember';
  }

  isPublic(): boolean {
    return this.role() === 'Public';
  }

  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.player());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.player().id);
  }

  onView(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.view.emit(this.player());
  }
}