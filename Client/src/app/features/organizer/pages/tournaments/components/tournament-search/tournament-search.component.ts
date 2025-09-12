import { Component, Output, EventEmitter, signal, effect, inject, OnInit, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TournamentFormat, TournamentSearch } from '@/app/api/models/tournament.models';
import { TournamentService } from '@/app/api/services';

@Component({
  selector: 'app-tournament-search',
  imports: [CommonModule, FormsModule],
  templateUrl: './tournament-search.component.html',
  styleUrl: './tournament-search.component.css'
})
export class TournamentSearchComponent implements OnInit {
  @Output() searchChange = new EventEmitter<Partial<TournamentSearch>>();
  private tournamentService = inject(TournamentService);

  searchTerm = signal('');
  selectedStatus = signal<string | undefined>(undefined);
  location = signal('');
  formatId = signal<string | undefined>(undefined);
  formats = signal<TournamentFormat[]>([]);
  minPrizePool = signal<number | undefined>(undefined);
  maxPrizePool = signal<number | undefined>(undefined);
  startDateFrom = signal<string | undefined>(undefined);
  startDateTo = signal<string | undefined>(undefined);
  isPublic = signal<boolean | undefined>(undefined);

    searchCriteria = computed<Partial<TournamentSearch>>(() => {
    return {
        searchTerm: this.searchTerm()?.trim() || undefined,
        status: this.selectedStatus() || undefined,
        location: this.location()?.trim() || undefined,
        formatId: this.formatId() || undefined,
        minPrizePool: this.minPrizePool() || undefined,
        maxPrizePool: this.maxPrizePool() || undefined,
        startDateFrom: this.startDateFrom() || undefined,
        startDateTo: this.startDateTo() || undefined,
        isPublic: this.isPublic() || undefined,
      };
    });


  ngOnInit() {
    this.getTournamentFormats();
  }

  getTournamentFormats(){
    this.tournamentService.getTournamentFormats().subscribe({
        next:(data)=>{
            this.formats.set(data) 
        }
    })
  }

  onSearchInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchTerm.set(target.value);
  }

  onStatusChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    const value = target.value || undefined;
    this.selectedStatus.set(value);
  }

  onLocationInput(event: Event) {
    const target = event.target as HTMLInputElement;
    this.location.set(target.value);
  }

  onFormatChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    let value: string | undefined = target.value;
    if(value == "All") {
        value = undefined;
    }
    this.formatId.set(value);
  }

  onMinPrizePoolInput(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value ? +target.value : undefined;
    this.minPrizePool.set(value);
  }

  onMaxPrizePoolInput(event: Event) {
    const target = event.target as HTMLInputElement;
    const value = target.value ? +target.value : undefined;
    this.maxPrizePool.set(value);
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

  search(){
    const cleanedCriteria = Object.fromEntries(
      Object.entries(this.searchCriteria()).filter(([_, value]) => value !== undefined)
    ) as Partial<TournamentSearch>;
    this.searchChange.emit(cleanedCriteria);
  }

  clearFilters() {
    this.searchTerm.set('');
    this.selectedStatus.set(undefined);
    this.location.set('');
    this.formatId.set(undefined);
    this.minPrizePool.set(undefined);
    this.maxPrizePool.set(undefined);
    this.startDateFrom.set(undefined);
    this.startDateTo.set(undefined);
    this.isPublic.set(undefined);
  }

  hasFilters(): boolean {
    return this.searchTerm() !== '' || 
           this.selectedStatus() !== undefined || 
           this.location() !== '' ||
           this.formatId() !== undefined ||
           this.minPrizePool() !== undefined ||
           this.maxPrizePool() !== undefined ||
           this.startDateFrom() !== undefined ||
           this.startDateTo() !== undefined ||
           this.isPublic() !== undefined;
  }
}
