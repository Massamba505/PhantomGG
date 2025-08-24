import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Team, TeamFormData } from '../../models/tournament';
import { TeamForm } from '../team-form/team-form';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-team-modal',
  standalone: true,
  imports: [CommonModule, TeamForm],
  template: `
    <div class="modal-overlay" [class.active]="isOpen" (click)="onOverlayClick($event)">
      <div class="modal-container" role="dialog" aria-modal="true">
        <div class="modal-header">
          <h3 class="modal-title">{{ team ? 'Edit Team' : 'Add New Team' }}</h3>
          <button class="modal-close" (click)="close.emit()" aria-label="Close modal">
            <i class="pi pi-times"></i>
          </button>
        </div>
        
        <div class="modal-body">
          <app-team-form
            [team]="team"
            (save)="onSaveTeam($event)"
            (cancel)="close.emit()"
          ></app-team-form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 50;
      opacity: 0;
      visibility: hidden;
      transition: opacity 0.2s ease, visibility 0.2s ease;
    }
    
    .modal-overlay.active {
      opacity: 1;
      visibility: visible;
    }
    
    .modal-container {
      background-color: var(--background);
      border-radius: 0.5rem;
      width: 100%;
      max-width: 32rem;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      transform: translateY(0);
      transition: transform 0.3s ease;
      max-height: 90vh;
      display: flex;
      flex-direction: column;
    }
    
    .modal-overlay:not(.active) .modal-container {
      transform: translateY(1rem);
    }
    
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 1.5rem;
      border-bottom: 1px solid var(--border);
    }
    
    .modal-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--foreground);
      margin: 0;
    }
    
    .modal-close {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 2rem;
      height: 2rem;
      border-radius: 0.375rem;
      border: none;
      background-color: transparent;
      color: var(--muted-foreground);
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .modal-close:hover {
      background-color: var(--muted);
      color: var(--foreground);
    }
    
    .modal-body {
      padding: 1.5rem;
      overflow-y: auto;
      flex: 1;
    }
  `]
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
      ...formData
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
