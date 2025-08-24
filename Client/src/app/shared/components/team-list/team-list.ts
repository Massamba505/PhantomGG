import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Team } from '../../models/tournament';
import { TeamCard } from '../team-card/team-card';

@Component({
  selector: 'app-team-list',
  standalone: true,
  imports: [CommonModule, TeamCard],
  template: `
    <div class="team-list-container">
      <div class="team-list-header">
        <h2 class="team-list-title">{{ title }}</h2>
        <div class="team-list-actions">
          <button *ngIf="showAddButton" (click)="addTeam.emit()" class="add-team-btn">
            <i class="pi pi-plus"></i> Add Team
          </button>
        </div>
      </div>
      
      <div *ngIf="teams.length === 0" class="no-teams-message">
        <p>No teams have been added yet.</p>
        <button *ngIf="showAddButton" (click)="addTeam.emit()" class="add-team-btn mt-4">
          <i class="pi pi-plus"></i> Add First Team
        </button>
      </div>
      
      <div *ngIf="teams.length > 0" class="team-grid">
        <app-team-card
          *ngFor="let team of teams"
          [team]="team"
          [showActions]="showActions"
          [showDetails]="showDetails"
          (edit)="editTeam.emit($event)"
          (delete)="deleteTeam.emit($event)"
        ></app-team-card>
      </div>
    </div>
  `,
  styles: [`
    .team-list-container {
      width: 100%;
    }
    
    .team-list-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }
    
    .team-list-title {
      font-size: 1.25rem;
      font-weight: 600;
      color: var(--foreground);
      margin: 0;
    }
    
    .add-team-btn {
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
      background-color: var(--primary);
      color: white;
      border: none;
      border-radius: 0.375rem;
      padding: 0.5rem 1rem;
      font-size: 0.875rem;
      font-weight: 500;
      cursor: pointer;
      transition: background-color 0.2s ease;
    }
    
    .add-team-btn:hover {
      background-color: var(--primary-hover);
    }
    
    .no-teams-message {
      text-align: center;
      padding: 3rem 1rem;
      background-color: var(--card);
      border: 1px dashed var(--border);
      border-radius: 0.5rem;
      color: var(--muted-foreground);
    }
    
    .team-grid {
      display: grid;
      grid-template-columns: repeat(1, 1fr);
      gap: 1rem;
    }
    
    @media (min-width: 640px) {
      .team-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
    
    @media (min-width: 1024px) {
      .team-grid {
        grid-template-columns: repeat(3, 1fr);
      }
    }
    
    @media (min-width: 1280px) {
      .team-grid {
        grid-template-columns: repeat(4, 1fr);
      }
    }
    
    .mt-4 {
      margin-top: 1rem;
    }
  `]
})
export class TeamList {
  @Input() teams: Team[] = [];
  @Input() title = 'Teams';
  @Input() showAddButton = true;
  @Input() showActions = true;
  @Input() showDetails = false;
  
  @Output() addTeam = new EventEmitter<void>();
  @Output() editTeam = new EventEmitter<Team>();
  @Output() deleteTeam = new EventEmitter<string>();
}
