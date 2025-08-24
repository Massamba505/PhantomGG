import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Team } from '../../models/tournament';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  standalone: true,
  imports: [],
})
export class TeamCard {
  @Input() team!: Team;
  @Output() edit = new EventEmitter<Team>();
  @Output() delete = new EventEmitter<string>();
}
