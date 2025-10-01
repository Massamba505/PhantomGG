import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Tournament } from '@/app/api/models/tournament.models';
import { Roles } from '@/app/shared/constants/roles';

export type UserRole = 'Organizer' | 'User' | 'Public';

@Component({
  selector: 'app-tournament-card',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './tournament-card.html',
  styleUrls: ['./tournament-card.css'],
})
export class TournamentCard {
  // Required tournament data
  tournament = input.required<Tournament>();

  // Controls card behavior
  role = input<UserRole>('Public');

  // Event outputs
  edit = output<Tournament>();
  delete = output<string>();
  join = output<Tournament>();
  leave = output<Tournament>();
  view = output<Tournament>();

  readonly icons = LucideIcons;

  /* -------------------------------
   * Event handlers
   * -----------------------------*/
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

  onView() {
    this.view.emit(this.tournament());
  }

  /* -------------------------------
   * Helpers
   * -----------------------------*/
  getProgressPercentage(): number {
    const t = this.tournament();
    return Math.min((t.teamCount / t.maxTeams) * 100, 100);
  }

  canJoin(): boolean {
    const t = this.tournament();
    const now = new Date();
    const regDeadline = t.registrationDeadline
      ? new Date(t.registrationDeadline)
      : new Date(t.startDate);

    return (
      this.role() === Roles.User.toString() &&
      t.status === 'RegistrationOpen' &&
      now < regDeadline &&
      t.teamCount < t.maxTeams
    );
  }

  isFull(): boolean {
    const t = this.tournament();
    return t.teamCount >= t.maxTeams;
  }

  isInProgress(): boolean {
    return this.tournament().status === "InProgress";
  }

  isOrganizer(): boolean {
    // debugger;/
    console.log(Roles.Organizer.toString())
    console.log(this.role())
    return this.role() === Roles.Organizer.toString();
  }
}
