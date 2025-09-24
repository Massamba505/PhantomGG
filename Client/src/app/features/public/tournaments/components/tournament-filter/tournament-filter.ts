import { Component, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

import { TournamentSearch } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-tournament-filter',
  templateUrl: './tournament-filter.html',
  styleUrls: ['./tournament-filter.css'],
  imports: [CommonModule, FormsModule, LucideAngularModule],
})
export class TournamentFilter {
  filtersChanged = output<TournamentSearch>();
  
  searchTerm = signal('');
  status = signal('');
  location = signal('');
  startDateFrom = signal('');
  startDateTo = signal('');
  
  readonly icons = LucideIcons;
  
  readonly statusOptions = [
    { value: '', label: 'All Statuses' },
    { value: 'RegistrationOpen', label: 'Registration Open' },
    { value: 'InProgress', label: 'In Progress' },
    { value: 'Completed', label: 'Completed' }
  ];

  onSearch() {
    const filters: TournamentSearch = {
      searchTerm: this.searchTerm() || undefined,
      status: this.status() || undefined,
      location: this.location() || undefined,
      startDateFrom: this.startDateFrom() || undefined,
      startDateTo: this.startDateTo() || undefined,
      isPublic: true,
      pageNumber: 1,
      pageSize: 12
    };
    
    this.filtersChanged.emit(filters);
  }

  onClear() {
    this.searchTerm.set('');
    this.status.set('');
    this.location.set('');
    this.startDateFrom.set('');
    this.startDateTo.set('');
    this.onSearch();
  }
}