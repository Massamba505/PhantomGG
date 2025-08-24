import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Team } from '../../models/tournament';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  standalone: true,
  imports: [CommonModule, RouterLink],
})
export class TeamCard {
  @Input() team!: Team;
  @Input() showActions = true;
  @Input() showDetails = false;
  @Output() edit = new EventEmitter<Team>();
  @Output() delete = new EventEmitter<string>();
  
  getFormattedDate(dateString: string): string {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return dateString;
    
    return new Intl.DateTimeFormat('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  }
}
