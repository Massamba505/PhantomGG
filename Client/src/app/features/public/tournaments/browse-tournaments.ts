import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PublicLayout } from '@/app/shared/components/layouts/public-layout/public-layout';
import { AuthStateService } from '@/app/store/AuthStateService';
import { TournamentService } from '@/app/core/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { Tournament, TournamentSearchRequest } from '@/app/shared/models/tournament';
import { LucideAngularModule, Search, Filter, Calendar, MapPin, Users, Trophy, Clock, ExternalLink } from 'lucide-angular';

@Component({
  selector: 'app-browse-tournaments',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, PublicLayout, LucideAngularModule],
  template: `
    <app-public-layout>
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <!-- Page Header -->
        <div class="text-center mb-8">
          <h1 class="text-4xl font-bold text-foreground mb-4">
            Browse Tournaments
          </h1>
          <p class="text-xl text-muted-foreground max-w-2xl mx-auto">
            Discover exciting gaming tournaments and competitions. 
            {{ isAuthenticated() ? 'Join the action!' : 'Sign up to participate!' }}
          </p>
        </div>

        <!-- Search and Filters -->
        <div class="bg-card rounded-lg border border-border p-6 mb-8">
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
            <!-- Search -->
            <div class="md:col-span-2">
              <div class="relative">
                <lucide-angular [img]="Search" size="20" 
                               class="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground"></lucide-angular>
                <input
                  type="text"
                  placeholder="Search tournaments..."
                  [(ngModel)]="searchTerm"
                  (ngModelChange)="onSearchChange()"
                  class="pl-10 w-full px-4 py-2 border border-border rounded-md focus:ring-2 focus:ring-primary focus:border-transparent"
                />
              </div>
            </div>

            <!-- Status Filter -->
            <div>
              <div class="relative">
                <select
                  [(ngModel)]="statusFilter"
                  (ngModelChange)="onFilterChange()"
                  class="w-full px-4 py-2 border border-border rounded-md focus:ring-2 focus:ring-primary focus:border-transparent appearance-none bg-background"
                >
                  <option value="All">All Statuses</option>
                  <option value="RegistrationOpen">Open for Registration</option>
                  <option value="RegistrationClosed">Registration Closed</option>
                  <option value="InProgress">In Progress</option>
                  <option value="Completed">Completed</option>
                </select>
                <lucide-angular [img]="Filter" size="16"
                               class="absolute right-3 top-1/2 transform -translate-y-1/2 text-muted-foreground pointer-events-none"></lucide-angular>
              </div>
            </div>

            <!-- Game Filter -->
            <div>
              <select
                [(ngModel)]="gameFilter"
                (ngModelChange)="onFilterChange()"
                class="w-full px-4 py-2 border border-border rounded-md focus:ring-2 focus:ring-primary focus:border-transparent appearance-none bg-background"
              >
                <option value="All">All Games</option>
                <option value="FIFA">FIFA</option>
                <option value="Call of Duty">Call of Duty</option>
                <option value="League of Legends">League of Legends</option>
                <option value="Counter-Strike">Counter-Strike</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Tournament Grid -->
        @if (loading()) {
          <div class="flex justify-center items-center py-12">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
          </div>
        } @else {
          @if (filteredTournaments().length > 0) {
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
              @for (tournament of filteredTournaments(); track tournament.id) {
                <div class="bg-card rounded-lg border border-border overflow-hidden hover:shadow-lg transition-shadow">
                  <!-- Tournament Image -->
                  <div class="h-48 bg-gradient-to-r from-primary/20 to-primary/10 flex items-center justify-center">
                    <lucide-angular [img]="Trophy" size="48" class="text-primary"></lucide-angular>
                  </div>

                  <!-- Tournament Info -->
                  <div class="p-6">
                    <div class="flex items-start justify-between mb-3">
                      <h3 class="text-lg font-semibold text-foreground line-clamp-2">
                        {{ tournament.name }}
                      </h3>
                      <span class="px-2 py-1 text-xs rounded-full"
                            [ngClass]="getStatusBadgeClass(tournament.status)">
                        {{ formatStatus(tournament.status) }}
                      </span>
                    </div>

                    <p class="text-muted-foreground text-sm mb-4 line-clamp-2">
                      {{ tournament.description || 'No description available' }}
                    </p>

                    <!-- Tournament Details -->
                    <div class="space-y-2 mb-4">
                      <div class="flex items-center space-x-2 text-sm text-muted-foreground">
                        <lucide-angular [img]="Calendar" size="16"></lucide-angular>
                        <span>{{ formatDate(tournament.startDate) }}</span>
                      </div>
                      
                      @if (tournament.location) {
                        <div class="flex items-center space-x-2 text-sm text-muted-foreground">
                          <lucide-angular [img]="MapPin" size="16"></lucide-angular>
                          <span>{{ tournament.location }}</span>
                        </div>
                      }
                      
                      <div class="flex items-center space-x-2 text-sm text-muted-foreground">
                        <lucide-angular [img]="Users" size="16"></lucide-angular>
                        <span>{{ tournament.teamCount }}/{{ tournament.maxTeams }} teams</span>
                      </div>

                      @if (tournament.prize && tournament.prize > 0) {
                        <div class="flex items-center space-x-2 text-sm font-medium text-green-600">
                          <lucide-angular [img]="Trophy" size="16"></lucide-angular>
                          <span>\${{ tournament.prize.toLocaleString() }} prize pool</span>
                        </div>
                      }
                    </div>

                    <!-- Action Buttons -->
                    <div class="flex space-x-2">
                      <a [routerLink]="['/public/tournaments', tournament.id]"
                         class="flex-1 btn btn-outline text-center">
                        <lucide-angular [img]="ExternalLink" size="16"></lucide-angular>
                        View Details
                      </a>
                      
                      @if (canJoinTournament(tournament)) {
                        @if (isAuthenticated()) {
                          <a [routerLink]="['/user/join-tournament', tournament.id]"
                             class="flex-1 btn btn-primary text-center">
                            Join Tournament
                          </a>
                        } @else {
                          <a routerLink="/auth/register"
                             class="flex-1 btn btn-primary text-center">
                            Sign Up to Join
                          </a>
                        }
                      } @else {
                        <button disabled class="flex-1 btn btn-secondary text-center opacity-50 cursor-not-allowed">
                          Registration Closed
                        </button>
                      }
                    </div>
                  </div>
                </div>
              }
            </div>
          } @else {
            <!-- Empty State -->
            <div class="text-center py-12">
              <lucide-angular [img]="Trophy" size="64" class="mx-auto text-muted-foreground mb-4"></lucide-angular>
              <h3 class="text-xl font-semibold mb-2">No Tournaments Found</h3>
              <p class="text-muted-foreground mb-6">
                Try adjusting your search criteria or check back later for new tournaments.
              </p>
              @if (!isAuthenticated()) {
                <a routerLink="/auth/register" class="btn btn-primary">
                  Sign Up to Create Tournaments
                </a>
              }
            </div>
          }
        }

        <!-- Call to Action for Unauthenticated Users -->
        @if (!isAuthenticated()) {
          <div class="bg-primary/5 border border-primary/20 rounded-lg p-8 text-center">
            <h2 class="text-2xl font-bold mb-4">Ready to Compete?</h2>
            <p class="text-muted-foreground mb-6">
              Join PhantomGG to participate in tournaments, create teams, and track your progress.
            </p>
            <div class="flex justify-center space-x-4">
              <a routerLink="/auth/register" class="btn btn-primary">
                Sign Up Now
              </a>
              <a routerLink="/auth/login" class="btn btn-outline">
                Already have an account?
              </a>
            </div>
          </div>
        }
      </div>
    </app-public-layout>
  `,
  styles: [
    `
    .line-clamp-2 {
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }
    `
  ]
})
export class BrowseTournaments implements OnInit {
  private authService = inject(AuthStateService);
  private tournamentService = inject(TournamentService);
  private toast = inject(ToastService);

  isAuthenticated = computed(() => this.authService.isAuthenticated());
  loading = signal(false);
  searchTerm = '';
  statusFilter = 'All';
  gameFilter = 'All';

  tournaments = signal<Tournament[]>([]);
  
  filteredTournaments = computed(() => {
    let filtered = this.tournaments();

    // Apply search filter
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(t => 
        t.name.toLowerCase().includes(term) ||
        t.description?.toLowerCase().includes(term)
      );
    }

    return filtered;
  });

  // Icons
  Search = Search;
  Filter = Filter;
  Calendar = Calendar;
  MapPin = MapPin;
  Users = Users;
  Trophy = Trophy;
  Clock = Clock;
  ExternalLink = ExternalLink;

  ngOnInit() {
    this.loadTournaments();
  }

  async loadTournaments() {
    this.loading.set(true);
    
    try {
      const response = await this.tournamentService.getAllTournaments().toPromise();
      this.tournaments.set(response?.data || []);
    } catch (error) {
      console.error('Failed to load tournaments:', error);
      this.toast.error('Failed to load tournaments');
      this.tournaments.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  async searchTournaments() {
    this.loading.set(true);
    
    try {
      const searchRequest: TournamentSearchRequest = {
        searchTerm: this.searchTerm || undefined,
        status: this.statusFilter === 'All' ? undefined : this.statusFilter
      };
      
      const response = await this.tournamentService.searchTournaments(searchRequest).toPromise();
      this.tournaments.set(response?.data || []);
    } catch (error) {
      console.error('Failed to search tournaments:', error);
      this.toast.error('Failed to search tournaments');
    } finally {
      this.loading.set(false);
    }
  }

  onSearchChange() {
    // For now, just filter client-side. In production, you might debounce and search server-side
    // this.searchTournaments();
  }

  onFilterChange() {
    if (this.statusFilter !== 'All' || this.searchTerm) {
      this.searchTournaments();
    } else {
      this.loadTournaments();
    }
  }

  canJoinTournament(tournament: Tournament): boolean {
    return tournament.status === 'RegistrationOpen' && 
           tournament.teamCount < tournament.maxTeams;
  }

  formatStatus(status: string): string {
    switch (status) {
      case 'RegistrationOpen': return 'Open';
      case 'RegistrationClosed': return 'Closed';
      case 'InProgress': return 'Live';
      case 'Completed': return 'Finished';
      default: return status;
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'RegistrationOpen':
        return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200';
      case 'RegistrationClosed':
        return 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200';
      case 'InProgress':
        return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200';
      case 'Completed':
        return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200';
    }
  }

  formatDate(date: string | Date): string {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }
}
