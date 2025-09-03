import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-schedule-section',
  template: `
    <div class="card">
      @if (loading()) {
        <!-- Loading State -->
        <div class="p-8 text-center">
          <div class="inline-flex items-center justify-center w-12 h-12 rounded-full  mb-4">
            <lucide-angular 
              [img]="icons.Loader2" 
              size="20" 
              class="animate-spin "
            ></lucide-angular>
          </div>
          <p class="">Loading schedule...</p>
        </div>
      } @else if (hasSchedule()) {
        <!-- Schedule Content -->
        <div class="p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold">Tournament Schedule</h3>
            <button 
              (click)="onRegenerateSchedule()"
              class="inline-flex items-center gap-2 px-3 py-2 text-sm border border-border rounded-md  transition-colors"
            >
              <lucide-angular [img]="icons.SquarePen" size="14"></lucide-angular>
              Regenerate
            </button>
          </div>
          
          <!-- Schedule content will go here -->
          <div class="text-center py-12 ">
            <p>Schedule display component will be implemented here</p>
          </div>
        </div>
      } @else {
        <!-- Empty State -->
        <div class="p-8 text-center">
          <div class="w-16 h-16 mx-auto mb-4 rounded-full  flex items-center justify-center">
            <lucide-angular 
              [img]="icons.Calendar" 
              size="24"
              class=""
            ></lucide-angular>
          </div>
          <h3 class="text-lg font-semibold mb-2">Schedule Not Generated</h3>
          <p class=" mb-6 max-w-sm mx-auto">
            Generate a schedule to start planning matches for your tournament
          </p>
          <button 
            (click)="onGenerateSchedule()" 
            class="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
            [disabled]="teamsCount() < 2"
          >
            <lucide-angular [img]="icons.Calendar" size="16"></lucide-angular>
            Generate Schedule
          </button>
          @if (teamsCount() < 2) {
            <p class="text-sm  mt-3">
              At least 2 teams are required to generate a schedule
            </p>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .card {
      background: white;
      border: 1px solid #e2e8f0;
      border-radius: 8px;
      box-shadow: 0 1px 3px 0 rgb(0 0 0 / 0.1);
    }

    .animate-spin {
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      from {
        transform: rotate(0deg);
      }
      to {
        transform: rotate(360deg);
      }
    }
  `],
  standalone: true,
  imports: [CommonModule, LucideAngularModule]
})
export class ScheduleSection {
  loading = input<boolean>(false);
  hasSchedule = input<boolean>(false);
  teamsCount = input<number>(0);

  generateSchedule = output<void>();
  regenerateSchedule = output<void>();

  readonly icons = LucideIcons;

  onGenerateSchedule() {
    this.generateSchedule.emit();
  }

  onRegenerateSchedule() {
    this.regenerateSchedule.emit();
  }
}
