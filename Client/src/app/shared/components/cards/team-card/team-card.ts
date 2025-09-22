import { Component, input, OnInit, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Team, User } from '@/app/api/models';
import { Roles } from '@/app/shared/constants/roles';

export interface TeamCardConfig {
  isPublicView?: boolean;
  Manager?: User | null;
  type: CardTypes;
}

export type CardTypes = 'approved' | 'pending' | 'viewOnly';
@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TeamCard implements OnInit {
  team = input.required<Team>();
  isLoading = signal(false);

  config = input<TeamCardConfig>({
    isPublicView: false,
    Manager: undefined,
    type: 'viewOnly'
  });
  
  isManager = false;
  pendingCard = false;
  approveCard = false;
  viewOnlyCard = false;
  
  edit = output<Team>();
  delete = output<string>();
  view = output<Team>();
  approve = output<string>();
  reject = output<string>();
  
  readonly icons = LucideIcons;

  ngOnInit(): void {
    this.isManager = this.config().Manager?.id == this.team().id
    
    this.pendingCard = this.config().type == 'pending';
    this.approveCard = this.config().type == 'approved';
    this.viewOnlyCard = this.config().type == 'viewOnly';
  }
  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.team());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.team().id);
  }

  onRemoveTeam(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.team().id);
  }

  onRejectTeam(event: Event) {
    event.stopPropagation();
    this.reject.emit(this.team().id);
  }

  approveTeam(event: Event) {
    event.stopPropagation();
    this.approve.emit(this.team().id);
  }

  onView() {
    this.view.emit(this.team());
  }
  
  getInitials(name: string): string {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  getFormattedDate(dateString: string): string {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return dateString;
    
    const now = new Date();
    const diffTime = Math.abs(now.getTime() - date.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays === 0) return 'Today';
    if (diffDays === 1) return '1 day ago';
    if (diffDays < 7) return `${diffDays} days ago`;
    if (diffDays < 30) return `${Math.floor(diffDays / 7)} weeks ago`;
    
    return new Intl.DateTimeFormat('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  }
}
