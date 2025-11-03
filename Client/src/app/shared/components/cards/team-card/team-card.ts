import { Component, input, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Team, TeamRegistrationStatus } from '@/app/api/models';

export type TeamRole = 'Manager' | 'Organizer' | 'Public';
export type TeamCardType = 'pending' | 'approved' | 'default';

@Component({
  selector: 'app-team-card',
  templateUrl: './team-card.html',
  styleUrls: ['./team-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TeamCard {
  team = input.required<Team>();
  role = input<TeamRole>('Public');
  cardType = input<TeamCardType>('default');
  isLoading = signal(false);
  
  edit = output<Team>();
  delete = output<string>();
  view = output<Team>();
  approve = output<string>();
  reject = output<string>();
  
  readonly icons = LucideIcons;

  isManager(): boolean {
    return this.role() === 'Manager';
  }

  isOrganizer(): boolean {
    return this.role() === 'Organizer';
  }

  isPublic(): boolean {
    return this.role() === 'Public';
  }

  isPendingCard(): boolean {
    return this.cardType() === 'pending';
  }

  isApprovedCard(): boolean {
    return this.cardType() === 'approved';
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

  onApproveTeam(event: Event) {
    event.stopPropagation();
    this.approve.emit(this.team().id);
  }

  onView(event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.view.emit(this.team());
  }
}
