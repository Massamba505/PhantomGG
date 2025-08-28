import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type TabType = 'teams' | 'schedule' | 'bracket' | 'results';

export interface TabItem {
  key: TabType;
  label: string;
  disabled?: boolean;
}

@Component({
  selector: 'app-tournament-tabs',
  template: `
    <div class="mb-6">
      <div class="border-b border-border">
        <div class="flex overflow-x-auto">
          @for (tab of tabs(); track tab.key) {
            <button
              (click)="onTabClick(tab.key)"
              class="px-4 py-2 font-medium border-b-2 transition-colors whitespace-nowrap"
              [class]="getTabClasses(tab)"
              [disabled]="tab.disabled"
              [attr.aria-selected]="activeTab() === tab.key"
            >
              {{ tab.label }}
            </button>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tab-active {
      border-color: #3b82f6;
      color: #3b82f6;
      background-color: rgba(59, 130, 246, 0.05);
    }
    
    .tab-inactive {
      border-color: transparent;
      color: #6b7280;
    }
    
    .tab-inactive:hover:not(:disabled) {
      color: #374151;
      background-color: rgba(0, 0, 0, 0.05);
    }
    
    .tab-inactive:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  `],
  standalone: true,
  imports: [CommonModule]
})
export class TournamentTabs {
  activeTab = input.required<TabType>();
  tabs = input<TabItem[]>([
    { key: 'teams', label: 'Teams' },
    { key: 'schedule', label: 'Schedule' },
    { key: 'bracket', label: 'Bracket' },
    { key: 'results', label: 'Results' }
  ]);

  tabChange = output<TabType>();

  onTabClick(tab: TabType) {
    this.tabChange.emit(tab);
  }

  getTabClasses(tab: TabItem): string {
    const isActive = this.activeTab() === tab.key;
    return isActive ? 'tab-active' : 'tab-inactive';
  }
}
