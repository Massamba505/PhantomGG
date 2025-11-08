import {
  Component,
  input,
  signal,
  OnInit,
  inject,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { TournamentTeam } from '@/app/api/models/team.models';
import { TeamRegistrationStatus } from '@/app/api/models/common.models';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import {
  TeamCard,
  TeamRole,
  TeamCardType,
} from '@/app/shared/components/cards/team-card/team-card';

type TeamTab = 'approved';

@Component({
  selector: 'app-tournament-team-management',
  imports: [CommonModule, LucideAngularModule, TeamCard],
  templateUrl: './tournament-team-management.html',
  styleUrl: './tournament-team-management.css',
})
export class TournamentTeamManagementComponent implements OnInit {
  tournamentId = input.required<string>();

  private readonly tournamentService = inject(TournamentService);
  private readonly toastService = inject(ToastService);

  readonly icons = LucideIcons;
  teamView = output<string>();

  activeTab = signal<TeamTab>('approved');
  approvedTeams = signal<TournamentTeam[]>([]);

  isLoading = signal(false);
  isActionLoading = signal<{ [key: string]: boolean }>({});

  ngOnInit() {
    this.loadTeams();
  }

  loadTeams() {
    this.isLoading.set(true);

    this.tournamentService
      .getTournamentTeams(this.tournamentId(), TeamRegistrationStatus.Approved)
      .subscribe({
        next: (teams) => {
          this.approvedTeams.set(teams);
        },
        complete: () => {
          this.isLoading.set(false);
        },
      });
  }

  setActiveTab(tab: TeamTab) {
    this.activeTab.set(tab);
  }

  getCurrentTeams(): TournamentTeam[] {
    const tab = this.activeTab();
    if(tab == 'approved'){
        return this.approvedTeams();
    }
    
    return [];
  }

  getTeamCount(tab: TeamTab): number {
    if(tab == 'approved'){
      return this.approvedTeams().length;
    }
    
    return 0;
  }

  getTabClass(tab: TeamTab): string {
    const baseClass =
      'px-2 py-1 font-semibold border-b-2 cursor-pointer sm:text-md text-xs ';
    const activeClass = 'border-primary text-primary';
    const inactiveClass = 'border-transparent text-muted';

    return baseClass + (this.activeTab() === tab ? activeClass : inactiveClass);
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
      countPlayers: tournamentTeam.players.length,
      players: tournamentTeam.players,
    };
  }

  getTeamRole(): TeamRole {
    return 'Public';
  }

  getTeamCardType(): TeamCardType {
    return 'approved';
  }

  onTeamView(team: any) {
    this.teamView.emit(team.id);
  }
}
