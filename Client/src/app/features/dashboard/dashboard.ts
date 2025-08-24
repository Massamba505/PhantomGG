import { Component } from '@angular/core';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  standalone: true,
  imports: [DashboardLayout, CommonModule, RouterLink],
})
export class Dashboard {
  // Dummy data for the dashboard
  upcomingTournaments = [
    {
      id: 1,
      name: 'Summer Showdown',
      date: '2023-07-15',
      game: 'League of Legends',
      participants: 16,
      status: 'open'
    },
    {
      id: 2,
      name: 'Winter Championship',
      date: '2023-08-22',
      game: 'Valorant',
      participants: 8,
      status: 'open'
    },
    {
      id: 3,
      name: 'Fall Classic',
      date: '2023-09-10',
      game: 'Counter-Strike 2',
      participants: 32,
      status: 'draft'
    }
  ];
  
  recentTournaments = [
    {
      id: 101,
      name: 'Spring Invitational',
      date: '2023-05-20',
      game: 'Dota 2',
      participants: 12,
      status: 'completed',
      winner: 'Team Phoenix'
    },
    {
      id: 102,
      name: 'Regional Qualifier',
      date: '2023-06-05',
      game: 'Rocket League',
      participants: 24,
      status: 'completed',
      winner: 'Cosmic Racers'
    }
  ];
}
