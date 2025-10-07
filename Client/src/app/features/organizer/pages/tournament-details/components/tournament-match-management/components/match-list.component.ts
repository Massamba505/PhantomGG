import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { MatchCard } from '@/app/shared/components/cards/match-card/match-card';
import { Match } from '@/app/api/models/match.models';

@Component({
  selector: 'app-match-list',
  imports: [CommonModule, LucideAngularModule, MatchCard],
  template: `
    @if (isLoading()) {
      <div class="text-center py-12">
        <lucide-angular
          size="32"
          [img]="icons.Loader2"
          class="animate-spin mx-auto mb-4"
        />
        <p class="text-muted">Loading matches...</p>
      </div>
    } 
    @else if (matches().length === 0) {
      <div class="text-center py-12">
        <lucide-angular
          size="48"
          [img]="icons.Calendar"
          class="mx-auto mb-4 text-muted"
        />
        <h3 class="text-lg font-medium mb-2">No matches found</h3>
        <p class="text-muted mb-4">
          @if (activeTab() === 'all') { 
            Get started by creating matches or generating fixtures for this tournament. 
          } @else { 
            No {{ activeTab() }} matches at the moment. 
          }
        </p>
      </div>
    } 
    @else {
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        @for (match of matches(); track match.id) {
          <app-match-card
            [match]="match"
            role="Organizer"
            (view)="view.emit(match)"
            (edit)="edit.emit(match)"
            (updateResult)="updateResult.emit(match)"
          />
        }
      </div>
    }
  `
})
export class MatchListComponent {
  matches = input.required<Match[]>();
  activeTab = input.required<string>();
  isLoading = input.required<boolean>();
  
  view = output<Match>();
  edit = output<Match>();
  updateResult = output<Match>();
  
  readonly icons = LucideIcons;
}