import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { CurrencyFormatPipe } from '@/app/shared/pipe/currency-format.pipe';

@Component({
  selector: 'app-tournament-card',
  standalone: true,
  imports: [CommonModule, LucideAngularModule, CurrencyFormatPipe],
  templateUrl: './tournament-card.component.html',
  styleUrl: './tournament-card.component.css'
})
export class TournamentCardComponent {
  @Input({ required: true }) tournament!: Tournament;
  @Output() edit = new EventEmitter<Tournament>();
  @Output() delete = new EventEmitter<Tournament>();
  @Output() view = new EventEmitter<Tournament>();

  readonly icons = LucideIcons;

  onEdit() {
    this.edit.emit(this.tournament);
  }

  onDelete() {
    this.delete.emit(this.tournament);
  }

  onView() {
    this.view.emit(this.tournament);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }
}
