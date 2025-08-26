import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Edit, Trash2, Users, Calendar, LucideAngularModule } from 'lucide-angular';
import { Tournament } from '../../models/tournament';
import { LucideIcons } from '../ui/icons/lucide-icons';

@Component({
  selector: 'app-tournament-card',
  templateUrl: './tournament-card.html',
  styleUrls: ['./tournament-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TournamentCard {
  @Input() tournament!: Tournament;
  @Output() edit = new EventEmitter<Tournament>();
  @Output() delete = new EventEmitter<string>();
  @Output() view = new EventEmitter<Tournament>();

  readonly icons = LucideIcons;

  statusColors = {
    draft: 'bg-muted text-muted-foreground',
    active: 'bg-primary text-primary-foreground',
    completed: 'bg-success text-success-foreground',
  };

  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.tournament);
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.tournament.id);
  }

  onView() {
    this.view.emit(this.tournament);
  }
}
