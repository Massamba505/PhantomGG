import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { TournamentService } from '../../../core/services/tournament.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tournament } from '../../../shared/models/tournament';
import { UserLayout } from '../../../shared/components/layouts/user-layout/user-layout';

// Extended interface for user-specific tournament data
interface UserTournament extends Tournament {
  game?: string;
  format?: string;
  prizePool: number;
  participantCount: number;
  maxParticipants: number;
  currentRound?: number;
  userPlacement?: number;
}

@Component({
  selector: 'app-my-tournaments',
  standalone: true,
  imports: [CommonModule, RouterModule, UserLayout],
  templateUrl: './my-tournaments.component.html',
  styleUrl: './my-tournaments.component.css'
})
export class MyTournaments implements OnInit {
  private tournamentService = inject(TournamentService);
  private authService = inject(AuthService);
  private router = inject(Router);

  isLoading = signal(true);
  myTournaments = signal<UserTournament[]>([]);

  // Computed properties for different tournament states
  activeTournaments = computed(() => 
    this.myTournaments().filter(t => t.status === 'Active')
  );

  upcomingTournaments = computed(() => 
    this.myTournaments().filter(t => t.status === 'Upcoming')
  );

  completedTournaments = computed(() => 
    this.myTournaments().filter(t => t.status === 'Completed')
  );

  totalPrizeWinnings = computed(() => 
    this.completedTournaments().reduce((total, tournament) => {
      // Calculate winnings based on placement (simplified logic)
      const placement = tournament.userPlacement || 999;
      if (placement === 1) return total + tournament.prizePool * 0.5;
      if (placement === 2) return total + tournament.prizePool * 0.3;
      if (placement === 3) return total + tournament.prizePool * 0.2;
      return total;
    }, 0)
  );

  ngOnInit() {
    this.loadMyTournaments();
  }

  private async loadMyTournaments() {
    try {
      this.isLoading.set(true);
      
      // For now, we'll simulate user's tournaments with mock data
      // In a real app, this would call an API endpoint like getUserTournaments()
      await this.simulateApiCall();
      
      const mockTournaments: UserTournament[] = [
        {
          id: '1',
          name: 'Spring Championship 2024',
          description: 'Annual spring tournament featuring top teams',
          game: 'League of Legends',
          format: 'Single Elimination',
          prizePool: 5000,
          prize: 5000,
          startDate: '2024-03-15',
          endDate: '2024-03-17',
          status: 'Active',
          maxTeams: 32,
          teamCount: 32,
          participantCount: 32,
          maxParticipants: 32,
          currentRound: 3,
          userPlacement: undefined,
          organizer: 'org1',
          organizerName: 'Gaming League Pro',
          createdAt: '2024-01-15T00:00:00Z'
        },
        {
          id: '2',
          name: 'Weekly Valorant Cup #12',
          description: 'Weekly competitive Valorant tournament',
          game: 'Valorant',
          format: 'Swiss System',
          prizePool: 1500,
          prize: 1500,
          startDate: '2024-02-28',
          endDate: '2024-02-28',
          status: 'Completed',
          maxTeams: 16,
          teamCount: 16,
          participantCount: 16,
          maxParticipants: 16,
          currentRound: undefined,
          userPlacement: 2,
          organizer: 'org2',
          organizerName: 'Valorant Community',
          createdAt: '2024-02-20T00:00:00Z'
        },
        {
          id: '3',
          name: 'CS:GO Masters Tournament',
          description: 'Professional CS:GO championship',
          game: 'Counter-Strike 2',
          format: 'Double Elimination',
          prizePool: 10000,
          prize: 10000,
          startDate: '2024-04-01',
          endDate: '2024-04-03',
          status: 'Upcoming',
          maxTeams: 32,
          teamCount: 24,
          participantCount: 24,
          maxParticipants: 32,
          currentRound: undefined,
          userPlacement: undefined,
          organizer: 'org3',
          organizerName: 'Counter-Strike League',
          createdAt: '2024-03-01T00:00:00Z'
        }
      ];

      this.myTournaments.set(mockTournaments);
    } catch (error) {
      console.error('Error loading tournaments:', error);
      this.myTournaments.set([]);
    } finally {
      this.isLoading.set(false);
    }
  }

  private simulateApiCall(): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, 1000));
  }

  getProgressPercentage(tournament: UserTournament): number {
    if (!tournament.currentRound || !tournament.maxParticipants) return 0;
    
    // Calculate progress based on elimination rounds
    const totalRounds = Math.ceil(Math.log2(tournament.maxParticipants));
    return (tournament.currentRound / totalRounds) * 100;
  }

  viewTournament(id: string) {
    this.router.navigate(['/public/tournaments', id]);
  }

  viewSchedule(id: string) {
    this.router.navigate(['/user/schedule'], { queryParams: { tournament: id } });
  }

  manageTournament(id: string) {
    this.router.navigate(['/user/tournaments', id, 'manage']);
  }

  browseTournaments() {
    this.router.navigate(['/public/tournaments']);
  }
}
