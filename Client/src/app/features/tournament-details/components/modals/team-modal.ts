import { Component, EventEmitter, Input, OnInit, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Team, CreateTeamRequest, UpdateTeamRequest } from '@/app/shared/models/tournament';
import { TeamForm } from '@/app/shared/components/team-form/team-form';
import { ToastService } from '@/app/shared/services/toast.service';
import { Modal } from '@/app/shared/components/ui/modal/modal';

@Component({
  selector: 'app-team-modal',
  standalone: true,
  imports: [CommonModule, TeamForm, Modal],
  template: `
    <app-modal 
      [isOpen]="isOpen" 
      (close)="close.emit()" 
      [title]="team ? 'Edit Team' : 'Add Team'"
    >
      <app-team-form
        [team]="team"
        (save)="onSaveTeam($event)"
        (cancel)="close.emit()"
      ></app-team-form>
    </app-modal>
  `,
})
export class TeamModal implements OnInit, OnChanges {
  @Input() isOpen = false;
  @Input() team: Team | null = null;
  @Input() tournamentId: string | null = null;

  @Output() save = new EventEmitter<Team>();
  @Output() close = new EventEmitter<void>();

  constructor(private toastService: ToastService) {}

  ngOnInit() {
    console.log('TeamModal initialized with team:', this.team);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['team']) {
      console.log('Team changed in modal:', changes['team'].currentValue);
    }
  }

  onSaveTeam(formData: CreateTeamRequest | UpdateTeamRequest) {
    if (!this.tournamentId) {
      this.toastService.error('Tournament ID is required');
      return;
    }

    // Create a new team object with the form data
    const teamData: Team = {
      id: this.team?.id || crypto.randomUUID(),
      name: formData.name,
      manager: formData.manager,
      numberOfPlayers: formData.numberOfPlayers,
      logoUrl: formData.logoUrl,
      tournamentId: this.tournamentId,
      tournamentName: '', // This will be populated by the API
      createdAt: this.team?.createdAt || new Date().toISOString(),
      updatedAt: new Date().toISOString(),
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
