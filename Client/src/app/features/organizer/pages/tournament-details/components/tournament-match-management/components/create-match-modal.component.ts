import { Component, input, output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Team } from '@/app/api/models/team.models';
import { CreateMatch } from '@/app/api/models/match.models';

@Component({
  selector: 'app-create-match-modal',
  imports: [CommonModule, ReactiveFormsModule, Modal],
  template: `
    <app-modal
      [isOpen]="isOpen()"
      title="Create New Match"
      (close)="close.emit()"
    >
      <form
        [formGroup]="createMatchForm"
        (ngSubmit)="onSubmit()"
        class="space-y-4"
      >
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">Home Team</label>
            <select formControlName="homeTeamId" class="input-select">
              <option disabled selected [ngValue]="null">
                Select home team
              </option>
              @for (team of teams(); track team.id) {
              <option [value]="team.id">{{ team.name }}</option>
              }
            </select>
          </div>

          <div>
            <label class="block text-sm font-medium mb-2">Away Team</label>
            <select formControlName="awayTeamId" class="input-select">
              <option disabled selected [ngValue]="null">
                Select away team
              </option>
              @for (team of teams(); track team.id) {
              <option [value]="team.id">{{ team.name }}</option>
              }
            </select>
          </div>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2"
            >Match Date & Time</label
          >
          <input
            type="datetime-local"
            [min]="startDate()"
            formControlName="matchDate"
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
            [disabled]="createMatchForm.invalid"
          >
            Create Match
          </button>
        </div>
      </form>
    </app-modal>
  `,
})
export class CreateMatchModalComponent {
  isOpen = input.required<boolean>();
  teams = input.required<Team[]>();
  tournamentId = input.required<string>();
  startDate = input.required<string>();

  close = output<void>();
  create = output<CreateMatch>();

  private readonly fb = inject(FormBuilder);
  createMatchForm: FormGroup = this.fb.group({
    homeTeamId: ['', Validators.required],
    awayTeamId: ['', Validators.required],
    matchDate: ['', Validators.required],
  });

  onSubmit() {
    if (this.createMatchForm.invalid) return;

    const formValue = this.createMatchForm.value;
    const createData: CreateMatch = {
      tournamentId: this.tournamentId(),
      homeTeamId: formValue.homeTeamId,
      awayTeamId: formValue.awayTeamId,
      matchDate: formValue.matchDate,
    };

    this.create.emit(createData);
  }

  reset() {
    this.createMatchForm?.reset();
  }
}
