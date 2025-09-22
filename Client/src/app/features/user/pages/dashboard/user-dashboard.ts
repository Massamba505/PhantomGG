import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { TeamService } from '@/app/api/services/team.service';
import { TournamentService } from '@/app/api/services/tournament.service';
import { Team } from '@/app/api/models/team.models';
import { Tournament } from '@/app/api/models/tournament.models';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-user-dashboard',
  imports: [
    CommonModule,
    RouterModule,
    LucideAngularModule
  ],
  template: `
    <div class="space-y-6">
      <!-- Stats Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- My Teams -->
        <div class="card">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400">My Teams</h3>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ myTeams().length }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <lucide-angular [img]="icons.Users" [size]="24" class="text-blue-600 dark:text-blue-400"></lucide-angular>
            </div>
          </div>
          <div class="mt-4">
            <a routerLink="/user/teams" class="text-sm text-blue-600 dark:text-blue-400 hover:underline">
              View all teams →
            </a>
          </div>
        </div>

        <!-- Available Tournaments -->
        <div class="card">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400">Available Tournaments</h3>
              <p class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ availableTournaments().length }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <lucide-angular [img]="icons.Trophy" [size]="24" class="text-green-600 dark:text-green-400"></lucide-angular>
            </div>
          </div>
          <div class="mt-4">
            <a routerLink="/user/tournaments" class="text-sm text-green-600 dark:text-green-400 hover:underline">
              Browse tournaments →
            </a>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="card">
          <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-3">Quick Actions</h3>
          <div class="space-y-2">
            <a 
              routerLink="/user/teams/create" 
              class="flex items-center p-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 rounded-lg transition-colors"
            >
              <lucide-angular [img]="icons.UserPlus" [size]="16" class="mr-2"></lucide-angular>
              Create Team
            </a>
            <a 
              routerLink="/user/tournaments" 
              class="flex items-center p-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 rounded-lg transition-colors"
            >
              <lucide-angular [img]="icons.Trophy" [size]="16" class="mr-2"></lucide-angular>
              Join Tournament
            </a>
          </div>
        </div>
      </div>

      <!-- Recent Teams -->
      @if (myTeams().length > 0) {
        <div class="card">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">My Teams</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            @for (team of myTeams(); track team.id) {
              <div class="p-4 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center space-x-3">
                  @if (team.logoUrl) {
                    <img [src]="team.logoUrl" [alt]="team.name + ' logo'" class="w-10 h-10 rounded-lg object-cover">
                  } @else {
                    <div class="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center text-white font-bold">
                      {{ team.name.charAt(0).toUpperCase() }}
                    </div>
                  }
                  <div class="flex-1">
                    <h3 class="font-medium text-gray-900 dark:text-gray-100">{{ team.name }}</h3>
                    @if (team.shortName) {
                      <p class="text-sm text-gray-600 dark:text-gray-400">{{ team.shortName }}</p>
                    }
                  </div>
                </div>
              </div>
            }
          </div>
        </div>
      }

      <!-- Getting Started -->
      @if (myTeams().length === 0) {
        <div class="card">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-gray-100 mb-4">Getting Started</h2>
          <div class="space-y-4">
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-blue-100 dark:bg-blue-900 text-blue-600 dark:text-blue-400 rounded-full flex items-center justify-center text-sm font-bold">
                1
              </div>
              <div>
                <h3 class="font-medium text-gray-900 dark:text-gray-100">Create your first team</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Start by creating a team to participate in tournaments</p>
                <a routerLink="/user/teams/create" class="text-sm text-blue-600 dark:text-blue-400 hover:underline">
                  Create team →
                </a>
              </div>
            </div>
            
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-gray-100 dark:bg-gray-800 text-gray-400 dark:text-gray-600 rounded-full flex items-center justify-center text-sm font-bold">
                2
              </div>
              <div>
                <h3 class="font-medium text-gray-500 dark:text-gray-400">Join tournaments</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Browse and register for available tournaments</p>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `
})
export class UserDashboard implements OnInit {
  private teamService = inject(TeamService);
  private tournamentService = inject(TournamentService);
  
  readonly icons = LucideIcons;
  
  myTeams = signal<Team[]>([]);
  availableTournaments = signal<Tournament[]>([]);

  ngOnInit() {
    this.loadMyTeams();
    this.loadAvailableTournaments();
  }

  loadMyTeams() {
    this.teamService.getMyTeams().subscribe({
      next: (response) => {
        this.myTeams.set(response.data);
      },
      error: (error) => {
        console.error('Failed to load teams:', error);
      }
    });
  }

  loadAvailableTournaments() {
    this.tournamentService.getTournaments({ pageSize: 10 }).subscribe({
      next: (response) => {
        this.availableTournaments.set(response.data.filter(t => t.status === 'Open'));
      },
      error: (error) => {
        console.error('Failed to load tournaments:', error);
      }
    });
  }
}
