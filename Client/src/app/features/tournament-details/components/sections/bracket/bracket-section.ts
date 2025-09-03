import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-bracket-section',
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
          <p>Loading bracket...</p>
        </div>
      } @else if (hasBracket()) {
        <!-- Bracket Content -->
        <div class="p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold">Tournament Bracket</h3>
            <div class="flex gap-2">
              <button 
                (click)="onEditBracket()"
                class="inline-flex items-center gap-2 px-3 py-2 text-sm border border-border rounded-md transition-colors"
              >
                <lucide-angular [img]="icons.Edit" size="14"></lucide-angular>
                Edit
              </button>
              <button 
                (click)="onRegenerateBracket()"
                class="inline-flex items-center gap-2 px-3 py-2 text-sm border border-border rounded-md transition-colors"
              >
                <lucide-angular [img]="icons.SquarePen" size="14"></lucide-angular>
                Regenerate
              </button>
            </div>
          </div>
          
          <!-- Bracket visualization will go here -->
          <div class="text-center py-12 ">
            <p>Bracket visualization component will be implemented here</p>
          </div>
        </div>
      } @else {
        <!-- Empty State -->
        <div class="p-8 text-center">
          <div class="w-16 h-16 mx-auto mb-4 rounded-full  flex items-center justify-center">
            <lucide-angular 
              [img]="icons.GitBranch" 
              size="24"
              class=""
            ></lucide-angular>
          </div>
          <h3 class="text-lg font-semibold mb-2">Tournament Bracket Not Created</h3>
          <p class=" mb-6 max-w-sm mx-auto">
            Create a bracket to visualize the tournament progression and matchups
          </p>
          <button 
            (click)="onCreateBracket()" 
            class="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-md hover:bg-primary/90 transition-colors"
            [disabled]="teamsCount() < 2"
          >
            <lucide-angular [img]="icons.GitBranch" size="16"></lucide-angular>
            Create Bracket
          </button>
          @if (teamsCount() < 2) {
            <p class="text-sm  mt-3">
              At least 2 teams are required to create a bracket
            </p>
          }
        </div>
      }
    </div>
  `,
  imports: [CommonModule, LucideAngularModule]
})
export class BracketSection {
  loading = input<boolean>(false);
  hasBracket = input<boolean>(false);
  teamsCount = input<number>(0);

  createBracket = output<void>();
  editBracket = output<void>();
  regenerateBracket = output<void>();

  readonly icons = LucideIcons;

  onCreateBracket() {
    this.createBracket.emit();
  }

  onEditBracket() {
    this.editBracket.emit();
  }

  onRegenerateBracket() {
    this.regenerateBracket.emit();
  }
}
