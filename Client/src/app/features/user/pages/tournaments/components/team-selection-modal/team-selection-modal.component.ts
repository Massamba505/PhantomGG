import { Component, signal, input, output, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Team } from '@/app/api/models/team.models';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from "@/app/shared/components/ui/modal/modal";

@Component({
  selector: 'app-team-selection-modal',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, Modal],
  templateUrl: "./team-selection-modal.component.html",
  styleUrl: './team-selection-modal.component.css'
})
export class TeamSelectionModalComponent {
  isOpen = input(false);
  teams = input<Team[]>([]);
  tournament = input<Tournament | null>(null);
  isJoining = input(false);
  
  teamSelected = output<Team>();
  modalClosed = output<void>();
  createTeamClicked = output<void>();
  
  readonly icons = LucideIcons;
  selectedTeam = signal<Team | null>(null);

  getModalTitle(): string {
    if (this.tournament) {
      return `Select Team for ${this.tournament.name}`;
    }
    return 'Select Team';
  }

  selectTeam(team: Team) {
    this.selectedTeam.set(team);
  }

  confirmSelection() {
    const team = this.selectedTeam();
    if (team) {
      this.teamSelected.emit(team);
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

}