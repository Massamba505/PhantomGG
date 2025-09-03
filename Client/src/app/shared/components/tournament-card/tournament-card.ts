import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Tournament } from '../../models/tournament';
import { LucideIcons } from '../ui/icons/lucide-icons';
import { getFormattedDate } from '../../utils/DateFormat';

@Component({
  selector: 'app-tournament-card',
  templateUrl: './tournament-card.html',
  styleUrls: ['./tournament-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TournamentCard {
  tournament = input.required<Tournament>();
  edit = output<Tournament>();
  delete = output<string>();
  view = output<Tournament>();

  readonly icons = LucideIcons;

  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.tournament());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.tournament().id);
  }

  onView() {
    this.view.emit(this.tournament());
  }

  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.style.display = 'none';
  }

  getFormattedDate(dateString: string, short: boolean = false): string {
    return getFormattedDate(dateString, short)
  }
}
