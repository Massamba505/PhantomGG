import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Team } from '../../models/tournament';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.component.html',
  styleUrls: ['./team-card.component.css'],
  standalone: true,
  imports: [],
})
export class TeamCardComponent {
  @Input() team!: Team;
  @Output() edit = new EventEmitter<Team>();
  @Output() delete = new EventEmitter<string>();
}
