import { Component, input, output, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { Team, Tournament } from '@/app/shared/models/tournament';
import { TeamCard } from '@/app/shared/components/team-card/team-card';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-teams-section',
  templateUrl: './teams-section.html',
  styleUrls: ['./teams-section.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, TeamCard]
})
export class TeamsSection {
  teams = input.required<Team[]>();
  tournament = input.required<Tournament | null>();
  loading = input<boolean>(false);

  addTeam = output<void>();
  editTeam = output<Team>();
  deleteTeam = output<string>();

  readonly icons = LucideIcons;

  searchTerm = signal<string>('');

  filteredTeams = computed(() => {
    const teams = this.teams();
    const search = this.searchTerm().toLowerCase().trim();
    
    if (!search) {
      return teams;
    }

    return teams.filter(team => 
      team.name.toLowerCase().includes(search) ||
      team.manager.toLowerCase().includes(search) ||
      team.tournamentName.toLowerCase().includes(search)
    );
  });

  onSearchChange() {
  }

  clearSearch() {
    this.searchTerm.set('');
  }

  onAddTeam() {
    this.addTeam.emit();
  }

  onEditTeam(team: Team) {
    this.editTeam.emit(team);
  }

  onDeleteTeam(teamId: string) {
    this.deleteTeam.emit(teamId);
  }
}
