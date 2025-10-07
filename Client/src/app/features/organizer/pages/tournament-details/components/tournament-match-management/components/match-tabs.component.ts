import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { Match } from '@/app/api/models/match.models';

export type MatchTab = 'all' | 'scheduled' | 'inprogress' | 'completed';

@Component({
  selector: 'app-match-tabs',
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex flex-wrap gap-0 sm:gap-2 border-b mb-6">
      <button
        (click)="setActiveTab('all')"
        [class]="getTabClass('all')"
      >
        All
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ matches().length }}
        </span>
      </button>

      <button
        (click)="setActiveTab('scheduled')"
        [class]="getTabClass('scheduled')"
      >
        Scheduled
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('scheduled') }}
        </span>
      </button>

      <button
        (click)="setActiveTab('inprogress')"
        [class]="getTabClass('inprogress')"
      >
        In Progress
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('inprogress') }}
        </span>
      </button>

      <button
        (click)="setActiveTab('completed')"
        [class]="getTabClass('completed')"
      >
        Completed
        <span class="btn btn-outline px-2 py-0.5 ml-0.5 rounded-full">
          {{ getTabCount('completed') }}
        </span>
      </button>
    </div>
  `
})
export class MatchTabsComponent {
  matches = input.required<Match[]>();
  activeTab = input.required<MatchTab>();
  
  tabChange = output<MatchTab>();

  setActiveTab(tab: MatchTab) {
    this.tabChange.emit(tab);
  }

  getTabClass(tab: MatchTab): string {
    const baseClass = 'px-2 py-1 font-medium border-b-2 cursor-pointer sm:text-lg text-xs ';
    const activeClass = 'border-primary text-primary';
    const inactiveClass = 'border-transparent text-muted hover:text-foreground';
    
    return baseClass + (this.activeTab() === tab ? activeClass : inactiveClass);
  }

  getTabCount(tab: MatchTab): number {
    if (tab === 'all') return this.matches().length;
    
    return this.matches().filter(match => {
      const status = match.status?.toLowerCase();
      switch (tab) {
        case 'scheduled':
          return status === 'scheduled' || status === 'pending';
        case 'inprogress':
          return status === 'inprogress' || status === 'in progress';
        case 'completed':
          return status === 'completed' || status === 'finished';
        default:
          return true;
      }
    }).length;
  }
}