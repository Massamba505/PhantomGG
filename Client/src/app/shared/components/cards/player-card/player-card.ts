import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Player } from '@/app/api/models/team.models';

export interface PlayerCardConfig {
  isManager?: boolean;
  showActions?: boolean;
}

@Component({
  selector: 'app-player-card',
  templateUrl: './player-card.html',
  imports: [CommonModule, LucideAngularModule],
})
export class PlayerCard {
  player = input.required<Player>();
  config = input<PlayerCardConfig>({
    isManager: false,
    showActions: true
  });
  
  edit = output<Player>();
  delete = output<string>();
  view = output<Player>();
  
  readonly icons = LucideIcons;

  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.player());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.player().id);
  }

  onView() {
    this.view.emit(this.player());
  }

}