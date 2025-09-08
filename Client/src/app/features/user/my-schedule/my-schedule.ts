import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserLayout } from '../../../shared/components/layouts/user-layout/user-layout';

interface ScheduleEvent {
  id: number;
  title: string;
  type: 'tournament' | 'match' | 'practice' | 'meeting';
  date: Date;
  startTime: string;
  endTime: string;
  location: string;
  team?: string;
  teamName?: string;
  tournamentName?: string;
  opponent?: string;
  status: 'upcoming' | 'ongoing' | 'completed' | 'cancelled' | 'live';
  description: string;
  result?: 'win' | 'loss' | 'draw';
}

interface FilterOption {
  label: string;
  value: string;
  count?: number;
}

@Component({
  selector: 'app-my-schedule',
  standalone: true,
  imports: [CommonModule, RouterModule, UserLayout],
  templateUrl: './my-schedule.component.html',
  styleUrl: './my-schedule.component.css'
})
export class MySchedule {
  private allEvents = signal<ScheduleEvent[]>([
    {
      id: 1,
      title: 'Valorant Championship Qualifier',
      type: 'tournament',
      date: new Date('2024-12-20'),
      startTime: '18:00',
      endTime: '22:00',
      location: 'Online',
      team: 'Shadow Runners',
      teamName: 'Shadow Runners',
      tournamentName: 'Valorant Championship',
      status: 'upcoming',
      description: 'Qualifier round for the regional championship'
    },
    {
      id: 2,
      title: 'Match vs Phoenix Squad',
      type: 'match',
      date: new Date('2024-12-18'),
      startTime: '19:30',
      endTime: '21:00',
      location: 'Online',
      team: 'Shadow Runners',
      teamName: 'Shadow Runners',
      opponent: 'Phoenix Squad',
      status: 'upcoming',
      description: 'League match - Round 3'
    },
    {
      id: 3,
      title: 'Team Practice Session',
      type: 'practice',
      date: new Date('2024-12-17'),
      startTime: '20:00',
      endTime: '22:00',
      location: 'Online',
      team: 'Shadow Runners',
      teamName: 'Shadow Runners',
      status: 'upcoming',
      description: 'Strategy practice for upcoming tournament'
    },
    {
      id: 4,
      title: 'League Finals',
      type: 'tournament',
      date: new Date('2024-12-15'),
      startTime: '15:00',
      endTime: '18:00',
      location: 'Gaming Arena',
      team: 'Shadow Runners',
      teamName: 'Shadow Runners',
      tournamentName: 'Regional League',
      status: 'completed',
      description: 'Regional league finals - Championship match',
      result: 'win'
    }
  ]);

  isLoading = signal(false);
  selectedFilter = signal<'all' | 'upcoming' | 'completed' | 'tournament' | 'match'>('all');
  activeFilter = signal<'all' | 'upcoming' | 'completed' | 'tournament' | 'match'>('all');
  selectedDate = signal<Date | null>(null);

  filterOptions = signal<FilterOption[]>([
    { label: 'All Events', value: 'all' },
    { label: 'Upcoming', value: 'upcoming' },
    { label: 'Completed', value: 'completed' },
    { label: 'Tournaments', value: 'tournament' },
    { label: 'Matches', value: 'match' }
  ]);

  events = computed(() => this.allEvents());

  filteredEvents = computed(() => {
    let filtered = this.events();
    
    const filter = this.selectedFilter();
    if (filter !== 'all') {
      if (filter === 'upcoming' || filter === 'completed') {
        filtered = filtered.filter(event => event.status === filter);
      } else {
        filtered = filtered.filter(event => event.type === filter);
      }
    }
    
    const selectedDate = this.selectedDate();
    if (selectedDate) {
      filtered = filtered.filter(event => 
        event.date.toDateString() === selectedDate.toDateString()
      );
    }
    
    return filtered.sort((a, b) => a.date.getTime() - b.date.getTime());
  });

  upcomingEvents = computed(() => 
    this.events().filter(event => event.status === 'upcoming')
  );

  upcomingEventsCount = computed(() => this.upcomingEvents().length);

  todayEvents = computed(() => {
    const today = new Date();
    return this.events().filter(event => 
      event.date.toDateString() === today.toDateString()
    );
  });

  todayEventsCount = computed(() => this.todayEvents().length);

  thisWeekEventsCount = computed(() => {
    const today = new Date();
    const nextWeek = new Date(today.getTime() + 7 * 24 * 60 * 60 * 1000);
    return this.events().filter(event => 
      event.date >= today && event.date <= nextWeek
    ).length;
  });

  completedEventsCount = computed(() => 
    this.events().filter(event => event.status === 'completed').length
  );

  setFilter(filter: 'all' | 'upcoming' | 'completed' | 'tournament' | 'match'): void {
    this.selectedFilter.set(filter);
    this.activeFilter.set(filter);
  }

  setActiveFilter(filter: string): void {
    this.setFilter(filter as 'all' | 'upcoming' | 'completed' | 'tournament' | 'match');
  }

  setDateFilter(date: Date | null): void {
    this.selectedDate.set(date);
  }

  getEventTypeClass(type: string): string {
    switch (type) {
      case 'tournament':
        return 'event-tournament';
      case 'match':
        return 'event-match';
      case 'practice':
        return 'event-practice';
      case 'meeting':
        return 'event-meeting';
      default:
        return '';
    }
  }

  getEventTypeLabel(type: string): string {
    switch (type) {
      case 'tournament':
        return 'Tournament';
      case 'match':
        return 'Match';
      case 'practice':
        return 'Practice';
      case 'meeting':
        return 'Meeting';
      default:
        return type;
    }
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'upcoming':
        return 'status-upcoming';
      case 'ongoing':
        return 'status-ongoing';
      case 'completed':
        return 'status-completed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }

  getEventIcon(type: string): string {
    switch (type) {
      case 'tournament':
        return '🏆';
      case 'match':
        return '⚔️';
      case 'practice':
        return '🎯';
      case 'meeting':
        return '👥';
      default:
        return '📅';
    }
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  isPast(date: Date): boolean {
    const today = new Date();
    return date < today;
  }

  findTournaments(): void {
    console.log('Finding tournaments to join');
  }

  findTeams(): void {
    console.log('Finding teams to join');
  }

  viewMatchDetails(eventId: number): void {
    console.log(`Viewing match details for event ${eventId}`);
  }

  viewTournament(eventId: number): void {
    console.log(`Viewing tournament details for event ${eventId}`);
  }

  joinLive(eventId: number): void {
    console.log(`Joining live event ${eventId}`);
  }

  addToCalendar(event: ScheduleEvent): void {
    console.log(`Adding event ${event.id} to calendar`);
  }

  joinEvent(eventId: number): void {
    console.log(`Joining event ${eventId}`);
  }

  cancelEvent(eventId: number): void {
    console.log(`Cancelling event ${eventId}`);
  }

  rescheduleEvent(eventId: number): void {
    console.log(`Rescheduling event ${eventId}`);
  }
}
