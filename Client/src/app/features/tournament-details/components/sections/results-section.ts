import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-results-section',
  template: `
    <div class="card">
      @if (loading()) {
        <!-- Loading State -->
        <div class="p-8 text-center">
          <div class="inline-flex items-center justify-center w-12 h-12 rounded-full  mb-4">
            <lucide-angular 
              [img]="icons.Loader2" 
              size="20" 
              class="animate-spin"
            ></lucide-angular>
          </div>
          <p>Loading results...</p>
        </div>
      } @else if (hasResults()) {
        <!-- Results Content -->
        <div class="p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold">Tournament Results</h3>
            <div class="flex gap-2">
              <button 
                (click)="onExportResults()"
                class="inline-flex items-center gap-2 px-3 py-2 text-sm border border-border rounded-md transition-colors"
              >
                <lucide-angular [img]="icons.ArrowRight" size="14"></lucide-angular>
                Export
              </button>
            </div>
          </div>
          
          <!-- Results content will go here -->
          <div class="space-y-4">
            <!-- Winner Section -->
            <div class="bg-gradient-to-r from-yellow-50 to-orange-50 border border-yellow-200 rounded-lg p-4">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-full bg-yellow-100 flex items-center justify-center">
                  <lucide-angular 
                    [img]="icons.Crown" 
                    size="20"
                    class="text-yellow-600"
                  ></lucide-angular>
                </div>
                <div>
                  <h4 class="font-semibold text-yellow-800">Tournament Champion</h4>
                  <p class="text-sm text-yellow-700">Results display will be implemented here</p>
                </div>
              </div>
            </div>
            
            <!-- Standings/Results List -->
            <div class="text-center py-8">
              <p>Match results and standings will be displayed here</p>
            </div>
          </div>
        </div>
      } @else {
        <!-- Empty State -->
        <div class="p-8 text-center">
          <div class="w-16 h-16 mx-auto mb-4 rounded-full  flex items-center justify-center">
            <lucide-angular 
              [img]="icons.Target" 
              size="24"
              class=""
            ></lucide-angular>
          </div>
          <h3 class="text-lg font-semibold mb-2">No Results Available</h3>
          <p class="mb-6 max-w-sm mx-auto">
            Match results will appear here once games begin and matches are completed
          </p>
          
          @if (tournamentStatus() === 'draft') {
            <div class="inline-flex items-center gap-2 px-4 py-2   rounded-md">
              <lucide-angular [img]="icons.Timer" size="16"></lucide-angular>
              Tournament not started
            </div>
          } @else if (tournamentStatus() === 'active') {
            <button 
              (click)="onViewLiveMatches()" 
              class="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
            >
              <lucide-angular [img]="icons.Eye" size="16"></lucide-angular>
              View Live Matches
            </button>
          } @else {
            <div class="inline-flex items-center gap-2 px-4 py-2   rounded-md cursor-not-allowed opacity-60">
              <lucide-angular [img]="icons.Target" size="16"></lucide-angular>
              View Results
            </div>
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
export class ResultsSection {
  loading = input<boolean>(false);
  hasResults = input<boolean>(false);
  tournamentStatus = input<string>('draft');

  viewLiveMatches = output<void>();
  exportResults = output<void>();

  readonly icons = LucideIcons;

  onViewLiveMatches() {
    this.viewLiveMatches.emit();
  }

  onExportResults() {
    this.exportResults.emit();
  }
}
