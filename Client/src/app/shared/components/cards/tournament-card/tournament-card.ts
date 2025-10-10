import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Tournament } from '@/app/api/models/tournament.models';
import { TournamentStatus } from '@/app/api/models';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';

export type CardRoles = 'Organizer' | 'User' | 'Public';

@Component({
  selector: 'app-tournament-card',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './tournament-card.html',
  styleUrls: ['./tournament-card.css'],
})
export class TournamentCard {
  tournament = input.required<Tournament>();
  role = input<CardRoles>('Public');
  edit = output<Tournament>();
  delete = output<string>();
  join = output<Tournament>();
  leave = output<Tournament>();
  view = output<Tournament>();


  readonly icons = LucideIcons;

  onEdit(e: Event) {
    e.stopPropagation();
    this.edit.emit(this.tournament());
  }

  onDelete(e: Event) {
    e.stopPropagation();
    this.delete.emit(this.tournament().id);
  }

  onJoin(e: Event) {
    e.stopPropagation();
    this.join.emit(this.tournament());
  }

  onLeave(e: Event) {
    e.stopPropagation();
    this.leave.emit(this.tournament());
  }

  onView(e?: Event) {
    if (e) {
      e.stopPropagation();
    }
    this.view.emit(this.tournament());
  }

  canJoin(): boolean {
    const t = this.tournament();
    const now = new Date();
    const regDeadline = t.registrationDeadline
      ? new Date(t.registrationDeadline)
      : new Date(t.startDate);
    return (
      this.role() === 'User' &&
      t.status === TournamentStatus.RegistrationOpen &&
      now < regDeadline &&
      t.teamCount < t.maxTeams
    );
  }

  isOrganizer(): boolean {
    return this.role() === 'Organizer';
  }

  isUser(): boolean {
    return this.role() === 'User';
  }
  isPublic(): boolean {
    return this.role() === 'Public';
  }

  getStatus(){
    return getEnumLabel(TournamentStatus, this.tournament().status);
  }
}
