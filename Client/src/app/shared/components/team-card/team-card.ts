import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Team } from '../../models/tournament';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../ui/icons/lucide-icons';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
})
export class TeamCard {
  team = input.required<Team>();
  showActions = input<boolean>(true);
  edit = output<Team>();
  delete = output<string>();
  readonly icons = LucideIcons;
  
  
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
