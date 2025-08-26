import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Team, TeamFormData } from '../../models/tournament';
import { TeamForm } from '../team-form/team-form';
import { ToastService } from '../../services/toast.service';
import { Modal } from '../modal/modal';

@Component({
  selector: 'app-team-modal',
  standalone: true,
  imports: [CommonModule, TeamForm, Modal],
  template: `
    <app-modal [isOpen]="isOpen" (close)="close.emit()" title="Team">
      <app-team-form
        [team]="team"
        (save)="onSaveTeam($event)"
        (cancel)="close.emit()"
      ></app-team-form>
    </app-modal>
  `,
})
export class TeamModal implements OnInit {
  @Input() isOpen = false;
  @Input() team: Team | null = null;
  @Input() tournamentId: string | null = null;

  @Output() save = new EventEmitter<Team>();
  @Output() close = new EventEmitter<void>();

  constructor(private toastService: ToastService) {}

  ngOnInit() {
    // Add any initialization logic here
  }

  onSaveTeam(formData: TeamFormData) {
    if (!this.tournamentId) {
      this.toastService.error('Tournament ID is required');
      return;
    }

    // Create a new team object with the form data
    const teamData: Team = {
      id: this.team?.id || crypto.randomUUID(),
      tournamentId: this.tournamentId,
      createdAt: this.team?.createdAt || new Date().toISOString(),
      ...formData,
    };

    this.save.emit(teamData);
    this.close.emit();
  }

  onOverlayClick(event: MouseEvent) {
    // Only close if the overlay itself is clicked, not the modal content
    if ((event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close.emit();
    }
  }
}
