import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-match-header',
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex flex-col md:flex-row justify-center md:justify-between items-start md:items-center gap-4 mb-6">
      <div>
        <h2 class="text-xl font-semibold mb-2">Match Management</h2>
        <p class="text-muted text-sm">
          Manage matches, results, and events for this tournament
        </p>
      </div>

      <div class="flex gap-2">
        <button
          (click)="generateFixtures.emit()"
          class="btn btn-outline"
          [disabled]="teamCount() < 2"
        >
          <lucide-angular size="16" [img]="icons.GitBranch" />
          Generate Fixtures
        </button>
        <button
          (click)="createMatch.emit()"
          class="btn btn-primary"
          [disabled]="teamCount() < 2"
        >
          <lucide-angular size="16" [img]="icons.Plus" />
          Add Match
        </button>
      </div>
    </div>
  `
})
export class MatchHeaderComponent {
  teamCount = input.required<number>();
  
  generateFixtures = output<void>();
  createMatch = output<void>();
  
  readonly icons = LucideIcons;
}