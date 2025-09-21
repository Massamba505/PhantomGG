import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Team } from '@/app/api/models';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TeamCard {
  team = input.required<Team>();
  showActions = input<boolean>(true);
  edit = output<Team>();
  delete = output<string>();
  view = output<Team>();
  readonly icons = LucideIcons;
  
  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.team());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.team().id);
  }

  onView() {
    this.view.emit(this.team());
  }

  onImageError(event: Event) {
    const img = event.target as HTMLImageElement;
    img.style.display = 'none';
  }
  
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
