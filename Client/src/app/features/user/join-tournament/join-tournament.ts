import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserLayout } from '../../../shared/components/layouts/user-layout/user-layout';
import { Tournament } from '../../../shared/models/tournament';
import { Team } from '../../../shared/models/tournament';

interface JoinTournamentForm {
  tournamentId: string;
  teamId?: string;
  teamName?: string;
  createNewTeam: boolean;
  agreeToTerms: boolean;
}

interface UserTeam extends Team {
  role?: string;
}

@Component({
  selector: 'app-join-tournament',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, UserLayout],
  templateUrl: './join-tournament.component.html',
  styleUrl: './join-tournament.component.css'
})
export class JoinTournament implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  isLoading = signal(true);
  isSubmitting = signal(false);
  registrationSuccess = signal(false);
  searchTerm = '';
  
  selectedTournament = signal<Tournament | null>(null);
  availableTournaments = signal<Tournament[]>([]);
  userTeams = signal<UserTeam[]>([]);

  private formData = signal<JoinTournamentForm>({
    tournamentId: '',
    teamId: undefined,
    teamName: undefined,
    createNewTeam: false,
    agreeToTerms: false
  });

  form = computed(() => this.formData());

  filteredTournaments = computed(() => {
    const tournaments = this.availableTournaments();
    if (!this.searchTerm) return tournaments;
    
    return tournaments.filter(t => 
      t.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      t.description.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  });

  canSubmit = computed(() => {
    const form = this.form();
    const hasTeam = form.createNewTeam ? !!form.teamName : !!form.teamId;
    return form.tournamentId && hasTeam && form.agreeToTerms && !this.isSubmitting();
  });

  ngOnInit() {
    this.loadData();
    
    // Check if tournament ID is provided in route params
    this.route.queryParams.subscribe(params => {
      if (params['tournament']) {
        this.updateForm({ tournamentId: params['tournament'] });
        this.loadSpecificTournament(params['tournament']);
      }
    });
  }

  private async loadData() {
    try {
      this.isLoading.set(true);
      
      await this.simulateApiCall();
      
      // Load available tournaments
      const mockTournaments: Tournament[] = [
        {
          id: '4',
          name: 'Summer League Championship',
          description: 'Competitive summer tournament with top prizes and professional teams from around the world.',
          startDate: '2024-04-15',
          endDate: '2024-04-17',
          maxTeams: 64,
          teamCount: 45,
          entryFee: 50,
          prize: 15000,
          status: 'Open',
          organizer: 'org1',
          organizerName: 'Gaming League Pro',
          createdAt: '2024-03-01T00:00:00Z',
          location: 'Online'
        },
        {
          id: '5',
          name: 'Friday Night Battles',
          description: 'Weekly competitive matches every Friday night for all skill levels.',
          startDate: '2024-03-29',
          endDate: '2024-03-29',
          maxTeams: 16,
          teamCount: 8,
          entryFee: 0,
          prize: 2000,
          status: 'Open',
          organizer: 'org2',
          organizerName: 'Valorant Community',
          createdAt: '2024-03-15T00:00:00Z',
          location: 'Online'
        }
      ];

      // Load user's teams
      const mockTeams: UserTeam[] = [
        {
          id: '1',
          name: 'Phantom Raiders',
          manager: 'John Doe',
          numberOfPlayers: 5,
          tournamentId: '1',
          tournamentName: 'Spring Championship 2024',
          createdAt: '2024-01-15T00:00:00Z',
          role: 'Leader'
        },
        {
          id: '2',
          name: 'Lightning Bolts',
          manager: 'Jane Smith',
          numberOfPlayers: 4,
          tournamentId: '2',
          tournamentName: 'Weekly Valorant Cup #12',
          createdAt: '2024-02-01T00:00:00Z',
          role: 'Member'
        }
      ];

      this.availableTournaments.set(mockTournaments);
      this.userTeams.set(mockTeams);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      this.isLoading.set(false);
    }
  }

  private async loadSpecificTournament(tournamentId: string) {
    // Find and select the specific tournament
    const tournament = this.availableTournaments().find(t => t.id === tournamentId);
    if (tournament) {
      this.selectedTournament.set(tournament);
      this.updateForm({ tournamentId });
    }
  }

  private simulateApiCall(): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, 1000));
  }

  private updateForm(updates: Partial<JoinTournamentForm>) {
    this.formData.set({ ...this.form(), ...updates });
  }

  searchTournaments() {
    // Reactive filtering happens automatically via computed
    console.log('Searching for:', this.searchTerm);
  }

  selectTournament(tournament: Tournament) {
    this.selectedTournament.set(tournament);
    this.updateForm({ tournamentId: tournament.id });
  }

  selectTeam(teamId: string) {
    this.updateForm({ teamId });
  }

  getTeamInitials(teamName: string): string {
    return teamName
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  async submitRegistration() {
    if (!this.canSubmit()) {
      return;
    }

    try {
      this.isSubmitting.set(true);
      
      // Simulate API call
      await this.simulateApiCall();
      
      // Show success
      this.registrationSuccess.set(true);
    } catch (error) {
      console.error('Error joining tournament:', error);
      alert('Failed to join tournament. Please try again.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  resetForm() {
    this.selectedTournament.set(null);
    this.formData.set({
      tournamentId: '',
      teamId: undefined,
      teamName: undefined,
      createNewTeam: false,
      agreeToTerms: false
    });
    this.registrationSuccess.set(false);
  }

  viewTournament() {
    if (this.selectedTournament()) {
      this.router.navigate(['/public/tournaments', this.selectedTournament()?.id]);
    }
  }

  goToMyTournaments() {
    this.router.navigate(['/user/my-tournaments']);
  }

  goBack() {
    this.router.navigate(['/user/dashboard']);
  }

  // Template binding helpers
  updateCreateNewTeam(value: boolean) {
    this.updateForm({ createNewTeam: value, teamId: undefined, teamName: undefined });
  }

  updateTeamName(teamName: string) {
    this.updateForm({ teamName });
  }

  updateAgreeToTerms(agreeToTerms: boolean) {
    this.updateForm({ agreeToTerms });
  }
}
