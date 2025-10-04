import { Component, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamSearch } from '@/app/api/models/team.models';

@Component({
  selector: 'app-team-search',
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule
  ],
  templateUrl: './team-search.html',
  styleUrl: './team-search.css'
})
export class TeamSearchComponent {
  readonly icons = LucideIcons;
  
  searchTerm = signal('');
  
  searchChange = output<Partial<TeamSearch>>();
  clearFiltersSearch = output<void>();

  onSearch() {
    const searchCriteria: Partial<TeamSearch> = {};
    
    if (this.searchTerm().trim()) {
      searchCriteria.searchTerm = this.searchTerm().trim();
    }
    
    this.searchChange.emit(searchCriteria);
  }

  onClear() {
    this.searchTerm.set('');
    this.clearFiltersSearch.emit();
  }
}