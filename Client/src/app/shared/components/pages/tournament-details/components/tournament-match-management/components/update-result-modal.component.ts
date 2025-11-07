import {
  Component,
  input,
  output,
  OnInit,
  inject,
  effect,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import {
  Match,
  MatchResult,
  MatchEvent,
  MatchEventType,
} from '@/app/api/models/match.models';
import { MatchStatus } from '@/app/api/models';
import { getEnumOptions, getEnumLabel } from '@/app/shared/utils/enumConvertor';

@Component({
  selector: 'app-update-result-modal',
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule, Modal],
  template: `
    <app-modal
      [isOpen]="isOpen()"
      title="Update Match Status"
      (close)="close.emit()"
    >
      @if (selectedMatch()) {
      <div class="space-y-6">
        <div class="text-center border rounded-lg p-4">
          <div class="flex items-center justify-between">
            <div class="text-sm font-medium">
              {{ selectedMatch()!.homeTeamName }}
            </div>
            <div class="text-xl font-bold">
              {{ calculateScore('home') }} - {{ calculateScore('away') }}
            </div>
            <div class="text-sm font-medium">
              {{ selectedMatch()!.awayTeamName }}
            </div>
          </div>
        </div>
        <div class="border-t pt-6">
          <div class="flex justify-between items-center mb-4">
            <h3 class="font-medium">Match Events</h3>
            <button (click)="addEvent.emit()" class="btn btn-outline btn-sm">
              <lucide-angular size="14" [img]="icons.Plus" />
              Add Event
            </button>
          </div>

          @if (matchEvents().length === 0) {
          <p class="text-center text-muted py-4">No events recorded yet</p>
          } @else {
          <div class="space-y-2 max-h-60 overflow-y-auto">
            @for(event of matchEvents(); track event.id){
            <div
              class="flex items-center justify-between p-3 border rounded-lg"
            >
              <div class="flex items-center gap-3">
                <lucide-angular
                  size="16"
                  [img]="getEventIcon(event.eventType)"
                  [class]="getEventIconClass(event.eventType)"
                />
                <div>
                  <div class="text-sm font-medium">
                    {{ formatEventType(event.eventType) }} -
                    {{ event.playerName }}
                  </div>
                  <div class="text-xs text-muted">
                    {{ event.minute }}' - {{ event.teamName }}
                  </div>
                </div>
              </div>
            </div>
            }
          </div>
          }
        </div>
      </div>
      }
    </app-modal>
  `,
})
export class UpdateResultModalComponent implements OnInit {
  isOpen = input.required<boolean>();
  selectedMatch = input.required<Match | null>();
  matchEvents = input.required<MatchEvent[]>();
  matchStatuses = getEnumOptions(MatchStatus);

  close = output<void>();
  updateResult = output<{ matchId: string; result: MatchResult }>();
  addEvent = output<void>();

  private fb = inject(FormBuilder);
  readonly icons = LucideIcons;

  updateResultForm!: FormGroup;

  constructor() {
    effect(() => {
      const match = this.selectedMatch();
      if (match && this.updateResultForm) {
        this.updateResultForm.patchValue({
          status: match.status,
        });
      }
    });
  }

  ngOnInit() {
    this.updateResultForm = this.fb.group({
      status: [MatchStatus.Scheduled, [Validators.required]],
    });
  }

  calculateScore(team: 'home' | 'away'): number {
    const match = this.selectedMatch();
    if (!match) return 0;

    const events = this.matchEvents();
    const teamId = team === 'home' ? match.homeTeamId : match.awayTeamId;

    return events.filter(
      (event) =>
        event.eventType === MatchEventType.Goal && event.teamId === teamId
    ).length;
  }

  onSubmit() {
    if (this.updateResultForm.invalid || !this.selectedMatch()) return;

    const formValue = this.updateResultForm.value;
    const resultData: MatchResult = {
      status: parseInt(formValue.status),
    };

    this.updateResult.emit({
      matchId: this.selectedMatch()!.id,
      result: resultData,
    });
  }

  formatEventType(eventType: string | number): string {
    return getEnumLabel(MatchEventType, eventType) || eventType.toString();
  }

  getEventIcon(eventType: MatchEventType): any {
    switch (eventType) {
      case MatchEventType.Goal:
        return this.icons.Target;
      case MatchEventType.YellowCard:
        return this.icons.AlertTriangle;
      case MatchEventType.RedCard:
        return this.icons.XCircle;
      case MatchEventType.Substitution:
        return this.icons.ArrowRight;
      case MatchEventType.Foul:
        return this.icons.AlertTriangle;
      default:
        return this.icons.CheckCircle;
    }
  }

  getEventIconClass(eventType: MatchEventType): string {
    switch (eventType) {
      case MatchEventType.Goal:
        return 'text-green-600';
      case MatchEventType.YellowCard:
        return 'text-yellow-600';
      case MatchEventType.RedCard:
        return 'text-red-600';
      case MatchEventType.Foul:
        return 'text-red-600';
      case MatchEventType.Substitution:
        return 'text-muted';
      default:
        return 'text-primary';
    }
  }
}
