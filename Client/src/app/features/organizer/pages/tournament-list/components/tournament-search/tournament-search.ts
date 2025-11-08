import {
  Component,
  signal,
  inject,
  OnInit,
  computed,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  TournamentFormat,
  TournamentSearch,
} from '@/app/api/models/tournament.models';
import { TournamentService, TournamentStatus } from '@/app/api/services';
import { getEnumOptions } from '@/app/shared/utils/enumConvertor';

@Component({
  selector: 'app-tournament-search',
  imports: [CommonModule, FormsModule],
  templateUrl: './tournament-search.html',
  styleUrl: './tournament-search.css',
})
export class TournamentSearchComponent implements OnInit {
  searchChange = output<Partial<TournamentSearch>>();
  clearFiltersSearch = output<void>();

  private readonly tournamentService = inject(TournamentService);

  searchTerm = signal('');
  selectedStatus = signal<TournamentStatus | undefined>(undefined);
  statusOptions = getEnumOptions(TournamentStatus);
  location = signal('');
  format = signal<string | undefined>(undefined);
  formats = signal<TournamentFormat[]>([]);
  startDateFrom = signal<string | undefined>(undefined);
  startDateTo = signal<string | undefined>(undefined);
  isPublic = signal<boolean | undefined>(undefined);

  searchCriteria = computed<Partial<TournamentSearch>>(() => {
    return {
      searchTerm: this.searchTerm()?.trim() || undefined,
      status: this.selectedStatus() || undefined,
      location: this.location()?.trim() || undefined,
      format: this.format() || undefined,
      startDateFrom: this.startDateFrom() || undefined,
      startDateTo: this.startDateTo() || undefined,
      isPublic: this.isPublic(),
    };
  });

  ngOnInit() {
    this.getTournamentFormats();
  }

  getTournamentFormats() {
    const formats = this.tournamentService.getTournamentFormats();
    this.formats.set(formats);
  }

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
    let value: string | undefined = target.value;
    if (value == 'All') {
      value = undefined;
    }
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
    if (target.value == 'true') {
      value = true;
    } else if (target.value == 'false') {
      value = false;
    } else {
      value = undefined;
    }
    this.isPublic.set(value);
  }

  search() {
    const cleanedCriteria = Object.fromEntries(
      Object.entries(this.searchCriteria()).filter(
        ([_, value]) => value !== undefined
      )
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
    return (
      this.searchTerm() !== '' ||
      this.selectedStatus() !== undefined ||
      this.location() !== '' ||
      this.format() !== undefined ||
      this.startDateFrom() !== undefined ||
      this.startDateTo() !== undefined ||
      this.isPublic() !== undefined
    );
  }
}
