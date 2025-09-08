import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserLayout } from '@/app/shared/components/layouts/user-layout/user-layout';
import { AuthStateService } from '@/app/store/AuthStateService';
import { TournamentService } from '@/app/core/services/tournament.service';
import { TeamService } from '@/app/core/services/team.service';
import { UserService } from '@/app/core/services/user.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideAngularModule, Trophy, Users, Calendar, Clock, Star, TrendingUp } from 'lucide-angular';

interface DashboardStats {
  activeTournaments: number;
  myTeams: number;
  upcomingMatches: number;
  completedMatches: number;
}

interface RecentActivity {
  id: string;
  type: 'tournament_joined' | 'match_completed' | 'team_joined';
  title: string;
  description: string;
  timestamp: Date;
  icon: any;
}

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, UserLayout, LucideAngularModule],
  template: `
    <app-user-layout>
      <div class="space-y-6">
        <!-- Welcome Header -->
        <div class="bg-gradient-to-r from-primary to-primary/80 rounded-lg p-6 text-primary-foreground">
          <h1 class="text-2xl font-bold mb-2">
            Welcome back, {{ user()?.firstName || 'Player' }}!
          </h1>
          <p class="text-primary-foreground/90">
            Ready to compete? Check out your tournaments and upcoming matches below.
          </p>
        </div>

        <!-- Quick Stats -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div class="bg-card rounded-lg p-4 border border-border">
            <div class="flex items-center space-x-3">
              <div class="p-2 bg-blue-500/10 rounded-lg">
                <lucide-angular [img]="Trophy" size="20" class="text-blue-500"></lucide-angular>
              </div>
              <div>
                <p class="text-sm text-muted-foreground">Active Tournaments</p>
                <p class="text-2xl font-bold">{{ stats().activeTournaments }}</p>
              </div>
            </div>
          </div>

          <div class="bg-card rounded-lg p-4 border border-border">
            <div class="flex items-center space-x-3">
              <div class="p-2 bg-green-500/10 rounded-lg">
                <lucide-angular [img]="Users" size="20" class="text-green-500"></lucide-angular>
              </div>
              <div>
                <p class="text-sm text-muted-foreground">My Teams</p>
                <p class="text-2xl font-bold">{{ stats().myTeams }}</p>
              </div>
            </div>
          </div>

          <div class="bg-card rounded-lg p-4 border border-border">
            <div class="flex items-center space-x-3">
              <div class="p-2 bg-orange-500/10 rounded-lg">
                <lucide-angular [img]="Calendar" size="20" class="text-orange-500"></lucide-angular>
              </div>
              <div>
                <p class="text-sm text-muted-foreground">Upcoming Matches</p>
                <p class="text-2xl font-bold">{{ stats().upcomingMatches }}</p>
              </div>
            </div>
          </div>

          <div class="bg-card rounded-lg p-4 border border-border">
            <div class="flex items-center space-x-3">
              <div class="p-2 bg-purple-500/10 rounded-lg">
                <lucide-angular [img]="Star" size="20" class="text-purple-500"></lucide-angular>
              </div>
              <div>
                <p class="text-sm text-muted-foreground">Matches Won</p>
                <p class="text-2xl font-bold">{{ stats().completedMatches }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Main Content Grid -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Recent Activity -->
          <div class="lg:col-span-2">
            <div class="bg-card rounded-lg border border-border">
              <div class="p-6 border-b border-border">
                <h2 class="text-lg font-semibold">Recent Activity</h2>
              </div>
              <div class="p-6">
                @if (recentActivity().length > 0) {
                  <div class="space-y-4">
                    @for (activity of recentActivity(); track activity.id) {
                      <div class="flex items-start space-x-3 p-3 rounded-lg hover:bg-muted/50 transition-colors">
                        <div class="p-2 bg-primary/10 rounded-lg">
                          <lucide-angular [img]="activity.icon" size="16" class="text-primary"></lucide-angular>
                        </div>
                        <div class="flex-1 min-w-0">
                          <p class="font-medium">{{ activity.title }}</p>
                          <p class="text-sm text-muted-foreground">{{ activity.description }}</p>
                          <p class="text-xs text-muted-foreground mt-1">
                            {{ formatRelativeTime(activity.timestamp) }}
                          </p>
                        </div>
                      </div>
                    }
                  </div>
                } @else {
                  <div class="text-center py-8">
                    <lucide-angular [img]="Clock" size="48" class="mx-auto text-muted-foreground mb-4"></lucide-angular>
                    <p class="text-muted-foreground">No recent activity</p>
                    <p class="text-sm text-muted-foreground">Join a tournament to get started!</p>
                  </div>
                }
              </div>
            </div>
          </div>

          <!-- Quick Actions & Upcoming -->
          <div class="space-y-6">
            <!-- Quick Actions -->
            <div class="bg-card rounded-lg border border-border">
              <div class="p-6 border-b border-border">
                <h2 class="text-lg font-semibold">Quick Actions</h2>
              </div>
              <div class="p-6 space-y-3">
                <a routerLink="/tournaments" 
                   class="flex items-center space-x-3 p-3 rounded-lg hover:bg-muted transition-colors group">
                  <lucide-angular [img]="Trophy" size="20" class="text-primary group-hover:scale-110 transition-transform"></lucide-angular>
                  <div>
                    <p class="font-medium">Browse Tournaments</p>
                    <p class="text-sm text-muted-foreground">Find tournaments to join</p>
                  </div>
                </a>

                <a routerLink="/user/join-tournament"
                   class="flex items-center space-x-3 p-3 rounded-lg hover:bg-muted transition-colors group">
                  <lucide-angular [img]="Users" size="20" class="text-primary group-hover:scale-110 transition-transform"></lucide-angular>
                  <div>
                    <p class="font-medium">Join Tournament</p>
                    <p class="text-sm text-muted-foreground">Register for competition</p>
                  </div>
                </a>

                <a routerLink="/profile"
                   class="flex items-center space-x-3 p-3 rounded-lg hover:bg-muted transition-colors group">
                  <lucide-angular [img]="TrendingUp" size="20" class="text-primary group-hover:scale-110 transition-transform"></lucide-angular>
                  <div>
                    <p class="font-medium">Update Profile</p>
                    <p class="text-sm text-muted-foreground">Manage your information</p>
                  </div>
                </a>
              </div>
            </div>

            <!-- Upcoming Matches -->
            <div class="bg-card rounded-lg border border-border">
              <div class="p-6 border-b border-border">
                <h2 class="text-lg font-semibold">Upcoming Matches</h2>
              </div>
              <div class="p-6">
                @if (upcomingMatches().length > 0) {
                  <div class="space-y-3">
                    @for (match of upcomingMatches(); track match.id) {
                      <div class="p-3 rounded-lg border border-border">
                        <p class="font-medium text-sm">{{ match.tournament }}</p>
                        <p class="text-sm text-muted-foreground">{{ match.opponent }}</p>
                        <p class="text-xs text-muted-foreground">{{ formatMatchTime(match.scheduledTime) }}</p>
                      </div>
                    }
                  </div>
                } @else {
                  <div class="text-center py-4">
                    <lucide-angular [img]="Calendar" size="32" class="mx-auto text-muted-foreground mb-2"></lucide-angular>
                    <p class="text-sm text-muted-foreground">No upcoming matches</p>
                  </div>
                }
              </div>
            </div>
          </div>
        </div>
      </div>
    </app-user-layout>
  `,
  styles: []
})
export class UserDashboard implements OnInit {
  private authService = inject(AuthStateService);
  private tournamentService = inject(TournamentService);
  private teamService = inject(TeamService);
  private userService = inject(UserService);
  private toast = inject(ToastService);

  user = computed(() => this.authService.user());
  loading = signal(false);

  // Dashboard data
  stats = signal<DashboardStats>({
    activeTournaments: 0,
    myTeams: 0,
    upcomingMatches: 0,
    completedMatches: 0
  });

  recentActivity = signal<RecentActivity[]>([]);
  upcomingMatches = signal<any[]>([]);

  // Icons
  Trophy = Trophy;
  Users = Users;
  Calendar = Calendar;
  Clock = Clock;
  Star = Star;
  TrendingUp = TrendingUp;

  ngOnInit() {
    this.loadDashboardData();
  }

  async loadDashboardData() {
    this.loading.set(true);
    
    try {
      // Load user stats and data
      await this.loadUserStats();
      await this.loadRecentActivity();
      await this.loadUpcomingMatches();
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
      this.toast.error('Failed to load dashboard data');
    } finally {
      this.loading.set(false);
    }
  }

  private async loadUserStats() {
    // Mock data for now - replace with actual API calls
    this.stats.set({
      activeTournaments: 3,
      myTeams: 2,
      upcomingMatches: 5,
      completedMatches: 12
    });
  }

  private async loadRecentActivity() {
    // Mock data for now - replace with actual API calls
    const mockActivity: RecentActivity[] = [
      {
        id: '1',
        type: 'tournament_joined',
        title: 'Joined Winter Championship',
        description: 'Successfully registered for the tournament',
        timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000), // 2 hours ago
        icon: Trophy
      },
      {
        id: '2',
        type: 'match_completed',
        title: 'Match vs Team Alpha',
        description: 'Won 3-1 in the quarter-finals',
        timestamp: new Date(Date.now() - 24 * 60 * 60 * 1000), // 1 day ago
        icon: Star
      },
      {
        id: '3',
        type: 'team_joined',
        title: 'Joined Phoenix Raiders',
        description: 'Accepted invitation to join the team',
        timestamp: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000), // 3 days ago
        icon: Users
      }
    ];

    this.recentActivity.set(mockActivity);
  }

  private async loadUpcomingMatches() {
    // Mock data for now - replace with actual API calls
    const mockMatches = [
      {
        id: '1',
        tournament: 'Winter Championship',
        opponent: 'Team Beta',
        scheduledTime: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000) // 2 days from now
      },
      {
        id: '2',
        tournament: 'Spring League',
        opponent: 'Warriors FC',
        scheduledTime: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000) // 5 days from now
      }
    ];

    this.upcomingMatches.set(mockMatches);
  }

  formatRelativeTime(date: Date): string {
    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    const diffInHours = Math.floor(diffInMs / (1000 * 60 * 60));
    const diffInDays = Math.floor(diffInHours / 24);

    if (diffInHours < 1) {
      return 'Just now';
    } else if (diffInHours < 24) {
      return `${diffInHours} hour${diffInHours > 1 ? 's' : ''} ago`;
    } else {
      return `${diffInDays} day${diffInDays > 1 ? 's' : ''} ago`;
    }
  }

  formatMatchTime(date: Date): string {
    const now = new Date();
    const diffInMs = date.getTime() - now.getTime();
    const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

    if (diffInDays === 0) {
      return `Today at ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
    } else if (diffInDays === 1) {
      return `Tomorrow at ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
    } else {
      return `${date.toLocaleDateString()} at ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
    }
  }
}
