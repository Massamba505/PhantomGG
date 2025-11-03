import { Component, signal, computed, output, input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TournamentSearch } from '@/app/api/models/tournament.models';
import { TournamentStatus, TournamentFormats } from '@/app/api/models/common.models';
import { getEnumOptions } from '@/app/shared/utils/enumConvertor';

@Component({
  selector: 'app-tournament-search',
  imports: [CommonModule, FormsModule],
  templateUrl: './tournament-search.html',
  styleUrl: './tournament-search.css'
})
export class TournamentSearchComponent implements OnInit {
  searchChange = output<Partial<TournamentSearch>>();
  clearFiltersSearch = output<void>();
  withDraft = input.required<boolean>()
  searchTerm = signal('');
  selectedStatus = signal<TournamentStatus | undefined>(undefined);
  location = signal('');
  format = signal<string | undefined>(undefined);
  startDateFrom = signal<string | undefined>(undefined);
  startDateTo = signal<string | undefined>(undefined);
  isPublic = signal<boolean | undefined>(undefined);

  statusOptions!: Array<{ label: string; value: TournamentStatus }>;
  formatOptions = getEnumOptions(TournamentFormats);

  ngOnInit(): void {
    this.statusOptions = getEnumOptions(TournamentStatus).filter(status => {
      const isDraft = status.value === TournamentStatus.Draft;
      return this.withDraft() || !isDraft;
    });
  }

  searchCriteria = computed<Partial<TournamentSearch>>(() => {
    return {
      searchTerm: this.searchTerm()?.trim() || undefined,
      status: this.selectedStatus() || undefined,
      location: this.location()?.trim() || undefined,
      format: this.format() || undefined,
      startFrom: this.startDateFrom() || undefined,
      startTo: this.startDateTo() || undefined,
      isPublic: this.isPublic() || undefined,
    };
  });

  onSearchInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }

  onStatusChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    const value = target.value ? parseInt(target.value) : undefined;
    this.selectedStatus.set(value);
  }

  onLocationInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.location.set(target.value);
  }

  onFormatChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    const value = target.value || undefined;
    this.format.set(value);
  }

  onStartDateFromChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value || undefined;
    this.startDateFrom.set(value);
  }

  onStartDateToChange(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value || undefined;
    this.startDateTo.set(value);
  }

  onIsPublicChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    let value: boolean | undefined;
    if (target.value === 'true') value = true;
    else if (target.value === 'false') value = false;
    else value = undefined;
    this.isPublic.set(value);
  }

  search() {
    const cleanedCriteria = Object.fromEntries(
      Object.entries(this.searchCriteria()).filter(([_, value]) => value !== undefined)
    ) as Partial<TournamentSearch>;
    this.searchChange.emit(cleanedCriteria);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.selectedStatus.set(undefined);
    this.location.set('');
    this.format.set(undefined);
    this.startDateFrom.set(undefined);
    this.startDateTo.set(undefined);
    this.isPublic.set(undefined);
    this.clearFiltersSearch.emit();
  }

  hasFilters(): boolean {
    return this.searchTerm() !== '' || 
           this.selectedStatus() !== undefined || 
           this.location() !== '' ||
           this.format() !== undefined ||
           this.startDateFrom() !== undefined ||
           this.startDateTo() !== undefined ||
           this.isPublic() !== undefined;
  }
}