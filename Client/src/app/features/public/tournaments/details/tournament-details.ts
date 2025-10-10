import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Tournament } from '@/app/api/models/tournament.models';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamRole } from "@/app/shared/components/cards/team-card/team-card";
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';

@Component({
  selector: 'app-tournament-details',
  templateUrl: './tournament-details.html',
  styleUrls: ['./tournament-details.css'],
  imports: [
    CommonModule,
    LucideAngularModule,
    TeamCard,
    LineBreaksPipe
],
})
export class TournamentDetails implements OnInit {
  tournament = signal<Tournament | null>(null);
  teams = signal<TournamentTeam[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);

  readonly icons = LucideIcons;

  tournamentStatus = computed(() => {
    const t = this.tournament();
    if (!t) return 'Unknown';
    
    const now = new Date();
    const startDate = new Date(t.startDate);
    const endDate = new Date(t.endDate);
    
    if (now < startDate) return 'Upcoming';
    if (now >= startDate && now <= endDate) return 'In Progress';
    if (now > endDate) return 'Completed';
    
    return t.status;
  });

  daysUntilStart = computed(() => {
    const t = this.tournament();
    if (!t) return null;
    
    const now = new Date();
    const startDate = new Date(t.startDate);
    const diffTime = startDate.getTime() - now.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    return diffDays > 0 ? diffDays : null;
  });

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private tournamentService: TournamentService
  ) {}

  ngOnInit() {
    const tournamentId = this.route.snapshot.paramMap.get('id');
    if (tournamentId) {
      this.loadTournamentDetails(tournamentId);
    } else {
      this.router.navigate(['/public/tournaments']);
    }
  }

  convertToTeam(tournamentTeam: TournamentTeam) {
    return {
      id: tournamentTeam.id,
      name: tournamentTeam.name,
      shortName: tournamentTeam.shortName || '',
      logoUrl: tournamentTeam.logoUrl,
      userId: tournamentTeam.managerId || '',
      createdAt: tournamentTeam.registeredAt,
      updatedAt: undefined,
      countPlayers: tournamentTeam.countPlayers,
      players: tournamentTeam.players
    };
  }
  
  getTeamRole(): TeamRole {
    return 'Public';
  }

  async loadTournamentDetails(tournamentId: string) {
    this.loading.set(true);
    this.error.set(null);
    
    try {
      const [tournament, teams] = await Promise.all([
        this.tournamentService.getTournament(tournamentId).toPromise(),
        this.tournamentService.getTournamentTeams(tournamentId).toPromise()
      ]);
      
      this.tournament.set(tournament || null);
      this.teams.set(teams || []);
    } catch (error) {
      this.loading.set(false);
    } finally {
      this.loading.set(false);
    }
  }

  onTeamView(team: TournamentTeam) {
    console.log('View team:', team);
  }

  onBackToTournaments() {
    this.router.navigate(['/public/tournaments']);
  }

  onViewStatistics() {
    const tournamentId = this.route.snapshot.paramMap.get('id');
    if (tournamentId) {
      this.router.navigate(['/public/tournaments', tournamentId, 'statistics']);
    }
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'RegistrationOpen':
      case 'Upcoming':
        return 'status-badge status-success';
      case 'InProgress':
        return 'status-badge status-warning';
      case 'Completed':
        return 'status-badge status-secondary';
      default:
        return 'status-badge status-primary';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    
    const date = new Date(dateString);
    if (isNaN(date.getTime())) return dateString;
    
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  }

  getProgressPercentage(): number {
    const t = this.tournament();
    if (!t) return 0;
    return Math.min((t.teamCount / t.maxTeams) * 100, 100);
  }

  trackByTeamId(index: number, team: TournamentTeam): string {
    return team.id;
  }

  getTeamInitials(name: string): string {
    return name
      .split(' ')
      .map(word => word.charAt(0))
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  retryLoad() {
    const tournamentId = this.route.snapshot.paramMap.get('id');
    if (tournamentId) {
      this.loadTournamentDetails(tournamentId);
    }
  }
}