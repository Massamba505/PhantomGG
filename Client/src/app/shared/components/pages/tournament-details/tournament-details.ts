import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Tournament } from '@/app/api/models/tournament.models';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TeamCard, TeamRole } from "@/app/shared/components/cards/team-card/team-card";
import { LineBreaksPipe } from '@/app/shared/pipe/LineBreaks.pipe';
import { TournamentStatus } from '@/app/api/models';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';

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

  getStatus(){
    return getEnumLabel(TournamentStatus, this.tournament()!.status);
  }

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
    if (window.history.length >= 1) {
      window.history.back();
    } else {
      this.router.navigate(['/public/tournaments']);
    }
  }

  onViewStatistics() {
    const tournamentId = this.route.snapshot.paramMap.get('id');
    if (tournamentId) {
      this.router.navigate(['statistics'], { relativeTo: this.route });
    }
  }
}