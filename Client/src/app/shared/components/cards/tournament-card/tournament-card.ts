import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Tournament } from '@/app/api/models/tournament.models';

export interface TournamentCardConfig {
  isOrganizer?: boolean;
}

@Component({
  selector: 'app-tournament-card',
  templateUrl: './tournament-card.html',
  styleUrls: ['./tournament-card.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class TournamentCard {
  tournament = input.required<Tournament>();

  isOrganizer = input<boolean>();
  edit = output<Tournament>();
  delete = output<string>();
  join = output<Tournament>();
  leave = output<Tournament>();
  view = output<Tournament>();
  
  readonly icons = LucideIcons;
  
  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.tournament());
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.tournament().id);
  }

  onJoin(event: Event) {
    event.stopPropagation();
    this.join.emit(this.tournament());
  }

  onLeave(event: Event) {
    event.stopPropagation();
    this.leave.emit(this.tournament());
  }

  onView() {
    this.view.emit(this.tournament());
  }

  getProgressPercentage(): number {
    const tournament = this.tournament();
    return Math.min((tournament.teamCount / tournament.maxTeams) * 100, 100);
  }

  canJoin(): boolean {
    const tournament = this.tournament();
    const now = new Date();
    const registrationDeadline = tournament.registrationDeadline 
      ? new Date(tournament.registrationDeadline) 
      : new Date(tournament.startDate);

    return (
      tournament.status === 'RegistrationOpen' &&
      now < registrationDeadline &&
      tournament.teamCount < tournament.maxTeams
    );
  }
}