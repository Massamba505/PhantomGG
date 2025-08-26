import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Tournament } from '@/app/shared/models/tournament';

@Component({
  selector: 'app-edit-tournament-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, Modal],
  template: `
    <app-modal 
      [isOpen]="isOpen" 
      (close)="close.emit()" 
      title="Edit Tournament"
    >
      <form (ngSubmit)="onSave()" class="space-y-4">
        <div>
          <label class="block text-sm font-medium mb-2">
            Tournament Name
          </label>
          <input
            type="text"
            [(ngModel)]="tournamentData.name"
            name="name"
            class="input-field w-full"
            placeholder="Enter tournament name"
            required
          />
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">
            Description
          </label>
          <textarea
            [(ngModel)]="tournamentData.description"
            name="description"
            class="input-field w-full h-24 resize-none"
            placeholder="Enter tournament description"
            required
          ></textarea>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">
              Start Date
            </label>
            <input
              type="date"
              [(ngModel)]="tournamentData.startDate"
              name="startDate"
              class="input-field w-full"
              required
            />
          </div>

          <div>
            <label class="block text-sm font-medium mb-2">
              End Date
            </label>
            <input
              type="date"
              [(ngModel)]="tournamentData.endDate"
              name="endDate"
              class="input-field w-full"
              required
            />
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">
              Maximum Teams
            </label>
            <input
              type="number"
              [(ngModel)]="tournamentData.maxTeams"
              name="maxTeams"
              class="input-field w-full"
              min="2"
              max="64"
              required
            />
          </div>

          <div>
            <label class="block text-sm font-medium mb-2">
              Status
            </label>
            <select
              [(ngModel)]="tournamentData.status"
              name="status"
              class="input-field w-full"
            >
              <option value="draft">Draft</option>
              <option value="active">Active</option>
              <option value="completed">Completed</option>
            </select>
          </div>
        </div>

        <div class="flex space-x-3 pt-4">
          <button type="submit" class="btn btn-primary flex-1" [disabled]="!isFormValid()">
            Save Changes
          </button>
          <button type="button" (click)="close.emit()" class="btn btn-secondary flex-1">
            Cancel
          </button>
        </div>
      </form>
    </app-modal>
  `
})
export class EditTournamentModal implements OnChanges {
  @Input() isOpen = false;
  @Input() tournament: Tournament | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Tournament>();

  tournamentData: Partial<Tournament> = {
    name: '',
    description: '',
    startDate: '',
    endDate: '',
    maxTeams: 16,
    status: 'draft'
  };

  ngOnChanges(changes: SimpleChanges) {
    if (changes['tournament'] && this.tournament) {
      this.tournamentData = { ...this.tournament };
    }
  }

  onSave() {
    if (this.isFormValid() && this.tournament) {
      const updatedTournament: Tournament = {
        ...this.tournament,
        ...this.tournamentData
      } as Tournament;
      
      this.save.emit(updatedTournament);
      this.close.emit();
    }
  }

  isFormValid(): boolean {
    return !!(
      this.tournamentData.name?.trim() &&
      this.tournamentData.startDate &&
      this.tournamentData.endDate &&
      this.tournamentData.maxTeams &&
      this.tournamentData.maxTeams > 1
    );
  }
}
