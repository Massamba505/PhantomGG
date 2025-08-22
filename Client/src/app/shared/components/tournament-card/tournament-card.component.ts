import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Edit, Trash2, Users, Calendar } from 'lucide-angular';
import { Tournament } from '../../models/tournament';

@Component({
  selector: 'app-tournament-card',
  templateUrl: './tournament-card.component.html',
  styleUrls: ['./tournament-card.component.css'],
  standalone: true,
  imports: [],
})
export class TournamentCardComponent {
  @Input() tournament!: Tournament;
  @Output() edit = new EventEmitter<Tournament>();
  @Output() delete = new EventEmitter<string>();
  @Output() view = new EventEmitter<Tournament>();

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
