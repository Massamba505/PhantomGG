import { Component, input, output, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Team } from '@/app/api/models/team.models';
import { Match, MatchStatus } from '@/app/api/models';
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
            <select [value]="" formControlName="homeTeamId" class="input-select">
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
            [min]="startDate()"
            formControlName="matchDate"
            class="input-field"
          />
        </div>
        <div>
          <label class="block text-sm font-medium mb-2">Match Status</label>
          <select formControlName="status" class="input-select">
              @for(matchStatus of matchStatuses; track matchStatus){
              <option [value]="matchStatus">
                  {{ matchStatus }}
              </option>
              }
          </select>
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
export class EditMatchModalComponent {
  isOpen = input.required<boolean>();
  teams = input.required<Team[]>();
  startDate = input.required<string | null>();
  selectedMatch = input.required<Match | null>();
  matchStatuses = Object.values(MatchStatus);

  close = output<void>();
  update = output<{ matchId: string; updateData: UpdateMatch }>();
  
  private fb = inject(FormBuilder);
  
  editMatchForm: FormGroup = this.fb.group({
    homeTeamId: ['', Validators.required],
    awayTeamId: ['', Validators.required],
    matchDate: ['', Validators.required],
    venue: [''],
    status:['']
  });

  constructor() {
    effect(() => {
      const match = this.selectedMatch();
      if (match && this.editMatchForm) {
        this.editMatchForm.patchValue({
          homeTeamId: match.homeTeamId,
          awayTeamId: match.awayTeamId,
          matchDate: new Date(match.matchDate).toISOString().slice(0, 16),
          venue: match.venue || '',
          status: match.status
        });
      }
    });
  }

  onSubmit() {
    if (this.editMatchForm.invalid || !this.selectedMatch()) return;
    
    const formValue = this.editMatchForm.value;
    const updateData: UpdateMatch = {
      homeTeamId: formValue.homeTeamId,
      awayTeamId: formValue.awayTeamId,
      matchDate: formValue.matchDate,
      venue: formValue.venue,
      status: formValue.status
    };

    this.update.emit({
      matchId: this.selectedMatch()!.id,
      updateData
    });
  }
}