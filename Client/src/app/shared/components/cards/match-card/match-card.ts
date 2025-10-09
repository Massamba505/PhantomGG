import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../../ui/icons/lucide-icons';
import { Match } from '@/app/api/models/match.models';
import { MatchStatus } from '@/app/api/models';

export type MatchUserRole = 'Organizer' | 'User' | 'Public';

@Component({
  selector: 'app-match-card',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './match-card.html',
  styleUrls: ['./match-card.css'],
})
export class MatchCard {
  match = input.required<Match>();
  role = input<MatchUserRole>('Public');
  
  edit = output<Match>();
  updateResult = output<Match>();
  view = output<Match>();

  readonly icons = LucideIcons;

  onEdit(e: Event) {
    e.stopPropagation();
    this.edit.emit(this.match());
  }

  onUpdateResult(e: Event) {
    e.stopPropagation();
    this.updateResult.emit(this.match());
  }

  onView(e?: Event) {
    if (e) {
      e.stopPropagation();
    }
    this.view.emit(this.match());
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

  canUpdateResult(): boolean {
    const matchStatus = this.match().status;
    return this.isOrganizer() && (matchStatus === MatchStatus.InProgress);
  }

  canEdit(): boolean {
    const matchStatus = this.match().status;
    return this.isOrganizer() && (matchStatus === MatchStatus.Scheduled);
  }

  isInProgress(): boolean {
    const matchStatus = this.match().status;
    return matchStatus === MatchStatus.InProgress;
  }

  isCompleted(): boolean {
    const matchStatus = this.match().status;
    return matchStatus === MatchStatus.Completed;
  }

  getScoreDisplay(): string {
    if (this.isCompleted() || this.isInProgress()) {
      const homeScore = this.match().homeScore ?? 0;
      const awayScore = this.match().awayScore ?? 0;
      return `${homeScore} - ${awayScore}`;
    }
    return 'vs';
  }
}