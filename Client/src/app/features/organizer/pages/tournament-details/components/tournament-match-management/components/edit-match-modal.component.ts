import { Component, input, output, OnInit, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Team } from '@/app/api/models/team.models';
import { Tournament, Match } from '@/app/api/models';
import { UpdateMatch } from '@/app/api/models/match.models';

@Component({
  selector: 'app-edit-match-modal',
  imports: [CommonModule, ReactiveFormsModule, Modal],
  template: `
    <app-modal
      [isOpen]="isOpen()"
      title="Edit Match"
      (close)="close.emit()"
    >
      <form
        [formGroup]="editMatchForm"
        (ngSubmit)="onSubmit()"
        class="space-y-4"
      >
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">Home Team</label>
            <select formControlName="homeTeamId" class="input-select">
                @for(team of teams(); track team.id){
                <option [value]="team.id">
                    {{ team.name }}
                </option>
                }
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium mb-2">Away Team</label>
            <select formControlName="awayTeamId" class="input-select">
                @for(team of teams(); track team.id){
                <option [value]="team.id">
                    {{ team.name }}
                </option>
                }
            </select>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Match Date & Time</label>
          <input
            type="datetime-local"
            [min]="tournament()?.startDate"
            formControlName="matchDate"
            class="input-field"
          />
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Venue</label>
          <input
            type="text"
            formControlName="venue"
            placeholder="Enter venue"
            class="input-field"
          />
        </div>

        <div class="flex gap-2 pt-4">
          <button
            type="button"
            (click)="close.emit()"
            class="btn btn-outline flex-1"
          >
            Cancel
          </button>
          <button
            type="submit"
            class="btn btn-primary flex-1"
            [disabled]="editMatchForm.invalid"
          >
            Update Match
          </button>
        </div>
      </form>
    </app-modal>
  `
})
export class EditMatchModalComponent implements OnInit {
  isOpen = input.required<boolean>();
  teams = input.required<Team[]>();
  tournament = input.required<Tournament | null>();
  selectedMatch = input.required<Match | null>();
  
  close = output<void>();
  update = output<{ matchId: string; updateData: UpdateMatch }>();
  
  private fb = inject(FormBuilder);
  
  editMatchForm!: FormGroup;

  constructor() {
    effect(() => {
      const match = this.selectedMatch();
      if (match && this.editMatchForm) {
        this.editMatchForm.patchValue({
          homeTeamId: match.homeTeamId,
          awayTeamId: match.awayTeamId,
          matchDate: new Date(match.matchDate).toISOString().slice(0, 16),
          venue: match.venue || ''
        });
      }
    });
  }

  ngOnInit() {
    this.editMatchForm = this.fb.group({
      homeTeamId: ['', Validators.required],
      awayTeamId: ['', Validators.required],
      matchDate: ['', Validators.required],
      venue: ['']
    });
  }

  onSubmit() {
    if (this.editMatchForm.invalid || !this.selectedMatch()) return;
    
    const formValue = this.editMatchForm.value;
    const updateData: UpdateMatch = {
      homeTeamId: formValue.homeTeamId,
      awayTeamId: formValue.awayTeamId,
      matchDate: formValue.matchDate,
      venue: formValue.venue
    };

    this.update.emit({
      matchId: this.selectedMatch()!.id,
      updateData
    });
  }
}