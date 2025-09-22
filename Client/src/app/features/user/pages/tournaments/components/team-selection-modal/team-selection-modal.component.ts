import { Component, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Team } from '@/app/api/models/team.models';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-team-selection-modal',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: "./team-selection-modal.component.html",
  styleUrl: './team-selection-modal.component.css'
})
export class TeamSelectionModalComponent {
  @Input() isOpen = false;
  @Input() teams: Team[] = [];
  @Input() tournament: Tournament | null = null;
  @Input() isJoining = false;
  
  @Output() teamSelected = new EventEmitter<Team>();
  @Output() modalClosed = new EventEmitter<void>();
  @Output() createTeamClicked = new EventEmitter<void>();
  
  readonly icons = LucideIcons;
  selectedTeam = signal<Team | null>(null);

  selectTeam(team: Team) {
    this.selectedTeam.set(team);
  }

  confirmSelection() {
    const team = this.selectedTeam();
    if (team) {
      this.teamSelected.emit(team);
      this.closeModal();
    }
  }

  closeModal() {
    this.selectedTeam.set(null);
    this.modalClosed.emit();
  }

  onCreateTeam() {
    this.createTeamClicked.emit();
    this.closeModal();
  }

  onBackdropClick(event: Event) {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  trackByTeamId(index: number, team: Team): string {
    return team.id;
  }
}