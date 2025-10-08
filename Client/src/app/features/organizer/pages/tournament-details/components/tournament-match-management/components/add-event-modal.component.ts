import {
  Component,
  input,
  output,
  OnInit,
  inject,
  computed,
  signal
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators
} from '@angular/forms';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { Team, Player } from '@/app/api/models/team.models';
import {
  Match,
  CreateMatchEvent,
  MatchEventType
} from '@/app/api/models/match.models';

@Component({
  selector: 'app-add-event-modal',
  imports: [CommonModule, ReactiveFormsModule, Modal],
  template: `
    <app-modal
      [isOpen]="isOpen()"
      title="Add Match Event"
      (close)="close.emit()"
    >
      <form [formGroup]="addEventForm" (ngSubmit)="onSubmit()" class="space-y-4">
        <div>
          <label class="block text-sm font-medium mb-2">Team</label>
          <select formControlName="teamId" class="input-select">
            <option [ngValue]="null" disabled>Select team</option>
            @for (team of matchTeams(); track team.id) {
              <option [ngValue]="team.id">{{ team.name }}</option>
            }
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Player</label>
          <select formControlName="playerId" class="input-select">
            <option [ngValue]="null" disabled>Select player</option>
            @for (player of playersForSelectedTeam(); track player.id) {
              <option [ngValue]="player.id">
                {{ player.firstName }} {{ player.lastName }} ({{ player.position }})
              </option>
            }
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Event Type</label>
          <select formControlName="eventType" class="input-field">
            <option [ngValue]="null" disabled>Select event type</option>
            @for (eventType of eventTypes(); track eventType.id) {
              <option [ngValue]="eventType.id">
                {{ formatEventType(eventType.value) }}
              </option>
            }
          </select>
        </div>

        <div>
          <label class="block text-sm font-medium mb-2">Minute</label>
          <input
            type="number"
            formControlName="minute"
            min="0"
            max="120"
            placeholder="Match minute"
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
            [disabled]="addEventForm.invalid"
          >
            Add Event
          </button>
        </div>
      </form>
    </app-modal>
  `
})
export class AddEventModalComponent implements OnInit {
  isOpen = input.required<boolean>();
  selectedMatch = input.required<Match | null>();
  teams = input.required<Team[]>();

  close = output<void>();
  addEvent = output<CreateMatchEvent>();

  private fb = inject(FormBuilder);
  addEventForm!: FormGroup;

  selectedTeamId = signal<string | null>(null);
  selectedPlayerId = signal<string | null>(null);

  matchTeams = computed(() => {
    const match = this.selectedMatch();
    if (!match) return [];

    return this.teams().filter(
      (team) => team.id === match.homeTeamId || team.id === match.awayTeamId
    );
  });

  playersForSelectedTeam = computed(() => {
    const teamId = this.selectedTeamId();
    if (!teamId) return [];

    const team = this.teams().find((team) => team.id === teamId);
    return team?.players ?? [];
  });

  selectedPlayer = computed(() => {
    const playerId = this.selectedPlayerId();
    return this.playersForSelectedTeam().find(p => p.id === playerId) ?? null;
  });

  eventTypes = computed(() => {
    return Object.keys(MatchEventType)
      .filter(key => isNaN(Number(key)))
      .map(key => ({
        id: MatchEventType[key as keyof typeof MatchEventType] as number,
        value: key
      }));
  });

  ngOnInit() {
    this.addEventForm = this.fb.group({
      teamId: [null, Validators.required],
      playerId: [null, Validators.required],
      eventType: [null, Validators.required],
      minute: [0, [Validators.required, Validators.min(0), Validators.max(120)]]
    });

    this.addEventForm.get('teamId')?.valueChanges.subscribe((teamId) => {
      this.selectedTeamId.set(teamId ?? null);
      this.addEventForm.get('playerId')?.reset(null);
    });

    this.addEventForm.get('playerId')?.valueChanges.subscribe((playerId) => {
      this.selectedPlayerId.set(playerId ?? null);
    });
  }

  onSubmit() {
    if (this.addEventForm.invalid || !this.selectedMatch()) return;

    const formValue = this.addEventForm.value;

    const eventData: CreateMatchEvent = {
      matchId: this.selectedMatch()!.id,
      teamId: formValue.teamId,
      playerId: formValue.playerId,
      eventType: formValue.eventType,
      minute: formValue.minute
    };

    this.addEvent.emit(eventData);
  }

  reset() {
    this.addEventForm?.reset();
    this.selectedTeamId.set(null);
    this.selectedPlayerId.set(null);
  }

  formatEventType(eventType: string): string {
    return eventType.replace(/([A-Z])/g, ' $1').trim();
  }
}
