import { Component, OnInit, inject, signal } from '@angular/core';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TournamentService } from '@/app/core/services/tournament.service';
import { Tournament, TournamentSearchRequest } from '@/app/shared/models/tournament';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  standalone: true,
  imports: [DashboardLayout, CommonModule, RouterLink],
})
export class Dashboard implements OnInit {
  private tournamentService = inject(TournamentService);
  
  upcomingTournaments = signal<Tournament[]>([]);
  recentTournaments = signal<Tournament[]>([]);
  loading = signal<boolean>(false);
  
  ngOnInit() {
    this.loadDashboardData();
  }
  
  async loadDashboardData() {
    try {
      this.loading.set(true);
      
      // Load upcoming tournaments (active status)
      const upcomingRequest: TournamentSearchRequest = {
        status: 'active',
        pageSize: 5
      };
      
      // Load recent tournaments (completed status)
      const recentRequest: TournamentSearchRequest = {
        status: 'completed',
        pageSize: 5
      };
      
      const [upcomingResponse, recentResponse] = await Promise.all([
        this.tournamentService.searchTournaments(upcomingRequest).toPromise(),
        this.tournamentService.searchTournaments(recentRequest).toPromise()
      ]);
      
      this.upcomingTournaments.set(upcomingResponse?.data || []);
      this.recentTournaments.set(recentResponse?.data || []);
      
    } catch (error) {
      console.error('Failed to load dashboard data:', error);
      // Set empty arrays on error
      this.upcomingTournaments.set([]);
      this.recentTournaments.set([]);
    } finally {
      this.loading.set(false);
    }
  }
}
