import { Component, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';

import { TournamentSearch } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { getEnumOptions } from '@/app/shared/utils/enumConvertor';
import { TournamentStatus } from '@/app/api/models';

@Component({
  selector: 'app-tournament-filter',
  templateUrl: './tournament-filter.html',
  styleUrls: ['./tournament-filter.css'],
  imports: [CommonModule, FormsModule, LucideAngularModule],
})
export class TournamentFilter {
  filtersChanged = output<TournamentSearch>();
  
  searchTerm = signal('');
  status = signal(null);
  location = signal('');
  startDateFrom = signal('');
  startDateTo = signal('');
  
  readonly icons = LucideIcons;
  
  readonly statusOptions = getEnumOptions(TournamentStatus);

  onSearch() {
    const filters: TournamentSearch = {
      searchTerm: this.searchTerm() || undefined,
      status: this.status() || undefined,
      location: this.location() || undefined,
      startFrom: this.startDateFrom() || undefined,
      startTo: this.startDateTo() || undefined,
      isPublic: true,
      page: 1,
      pageSize: 12
    };
    
    this.filtersChanged.emit(filters);
  }

  onClear() {
    this.searchTerm.set('');
    this.status.set(null);
    this.location.set('');
    this.startDateFrom.set('');
    this.startDateTo.set('');
    this.onSearch();
  }
}