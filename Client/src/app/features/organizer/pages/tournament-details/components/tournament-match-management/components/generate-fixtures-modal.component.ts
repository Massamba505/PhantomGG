import { Component, input, output, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Team } from '@/app/api/models/team.models';
import { Tournament } from '@/app/api/models';
import { GenerateFixtures } from '@/app/api/models/match.models';

@Component({
  selector: 'app-generate-fixtures-modal',
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, Modal],
  template: `
    <app-modal
      [isOpen]="isOpen()"
      title="Generate Tournament Fixtures"
      (close)="close.emit()"
    >
      <form
        [formGroup]="generateFixturesForm"
        (ngSubmit)="onSubmit()"
        class="space-y-4"
      >
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-4">
          <div class="flex items-center gap-2 mb-2">
            <lucide-angular
              size="16"
              [img]="icons.AlertCircle"
              class="text-blue-600"
            />
            <span class="text-sm font-medium text-blue-800">Tournament Info</span>
          </div>
          <p class="text-sm text-blue-700">
            {{ teams().length }} teams registered. This will generate
            {{ singleRoundMatches() }} matches
            @if (generateFixturesForm.get('includeReturnMatches')?.value) {
              ({{ doubleRoundMatches() }} with return matches)
            }
          </p>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Start Date</label>
          <input
            type="date"
            [min]="tournament()?.startDate"
            formControlName="startDate"
            class="input-field"
          />
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Days Between Matches</label>
          <input
            type="number"
            formControlName="daysBetweenMatches"
            min="1"
            placeholder="3"
            class="input-field"
          />
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Default Venue (Optional)</label>
          <input
            type="text"
            formControlName="defaultVenue"
            placeholder="Stadium or venue name"
            class="input-field"
          />
        </div>

        <div class="flex items-center gap-2">
          <input
            type="checkbox"
            formControlName="includeReturnMatches"
            class="checkbox"
            id="returnMatches"
          />
          <label for="returnMatches" class="text-sm">
            Include return matches (each team plays twice)
          </label>
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
            [disabled]="generateFixturesForm.invalid"
          >
            Generate Fixtures
          </button>
        </div>
      </form>
    </app-modal>
  `
})
export class GenerateFixturesModalComponent implements OnInit {
  isOpen = input.required<boolean>();
  teams = input.required<Team[]>();
  tournament = input.required<Tournament | null>();
  tournamentId = input.required<string>();
  
  close = output<void>();
  generateFixtures = output<GenerateFixtures>();
  
  private fb = inject(FormBuilder);
  readonly icons = LucideIcons;
  
  generateFixturesForm!: FormGroup;

  singleRoundMatches = computed(() => {
    const teamCount = this.teams().length;
    return (teamCount * (teamCount - 1)) / 2;
  });

  doubleRoundMatches = computed(() => {
    const teamCount = this.teams().length;
    return teamCount * (teamCount - 1);
  });

  ngOnInit() {
    this.generateFixturesForm = this.fb.group({
      startDate: ['', Validators.required],
      daysBetweenMatches: [3, [Validators.min(1)]],
      defaultVenue: [''],
      includeReturnMatches: [false]
    });
  }

  onSubmit() {
    if (this.generateFixturesForm.invalid) return;
    
    const formValue = this.generateFixturesForm.value;
    const fixtureData: GenerateFixtures = {
      tournamentId: this.tournamentId(),
      startDate: formValue.startDate,
      daysBetweenMatches: formValue.daysBetweenMatches,
      defaultVenue: formValue.defaultVenue,
      includeReturnMatches: formValue.includeReturnMatches
    };

    this.generateFixtures.emit(fixtureData);
  }

  reset() {
    this.generateFixturesForm?.reset({
      daysBetweenMatches: 3,
      includeReturnMatches: false
    });
  }
}