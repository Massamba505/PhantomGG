import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Match } from '@/app/api/models/match.models';
import { MatchStatus } from '@/app/api/models';

export type MatchTab = 'all' | 'scheduled' | 'inprogress' | 'completed';

@Component({
  selector: 'app-match-tabs',
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex flex-wrap gap-0 sm:gap-2 border-b mb-6">
      <button
        data-cy="matches-tab"
        (click)="setActiveTab('all')"
        [class]="getTabClass('all')"
      >
        All
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ matches().length }}
        </span>
      </button>

      <button
        data-cy="scheduled-matches-tab"
        (click)="setActiveTab('scheduled')"
        [class]="getTabClass('scheduled')"
      >
        Scheduled
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('scheduled') }}
        </span>
      </button>

      <button
        data-cy="inprogress-matches-tab"
        (click)="setActiveTab('inprogress')"
        [class]="getTabClass('inprogress')"
      >
        In Progress
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('inprogress') }}
        </span>
      </button>

      <button
        data-cy="completed-matches-tab"
        (click)="setActiveTab('completed')"
        [class]="getTabClass('completed')"
      >
        Completed
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('completed') }}
        </span>
      </button>
    </div>
  `,
})
export class MatchTabsComponent {
  matches = input.required<Match[]>();
  activeTab = input.required<MatchTab>();

  tabChange = output<MatchTab>();

  setActiveTab(tab: MatchTab) {
    this.tabChange.emit(tab);
  }

  getTabClass(tab: MatchTab): string {
    const baseClass =
      'px-2 py-1 font-semibold border-b-2 cursor-pointer sm:text-md text-xs ';
    const activeClass = 'border-primary text-primary';
    const inactiveClass = 'border-transparent text-muted';

    return baseClass + (this.activeTab() === tab ? activeClass : inactiveClass);
  }

  getTabCount(tab: MatchTab): number {
    if (tab === 'all') return this.matches().length;

    return this.matches().filter((match) => {
      const status = match.status;
      switch (tab) {
        case 'scheduled':
          return status === MatchStatus.Scheduled;
        case 'inprogress':
          return status === MatchStatus.InProgress;
        case 'completed':
          return status === MatchStatus.Completed;
        default:
          return true;
      }
    }).length;
  }
}
