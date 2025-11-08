import {
  Component,
  input,
  signal,
  OnInit,
  inject,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { TournamentService } from '@/app/api/services/tournament.service';
import { ToastService } from '@/app/shared/services/toast.service';
import {
  Tournament,
  TournamentStandingDto,
  PlayerGoalStandingDto,
  PlayerAssistStandingDto,
} from '@/app/api/models/tournament.models';

export type StatsTab = 'table' | 'goals' | 'assists';

@Component({
  selector: 'app-tournament-stats',
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './tournament-stats.html',
  styleUrl: './tournament-stats.css',
})
export class TournamentStatsComponent implements OnInit {
  tournamentId = input<string>('');
  tournament = input<Tournament | null>(null);
  showBackButton = input<boolean>(true);
  backRoute = input<string>('');
  private readonly _tournamentId = signal<string>('');

  getTournamentId = computed(() => {
    return this.tournamentId() || this._tournamentId();
  });

  private readonly tournamentService = inject(TournamentService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly icons = LucideIcons;

  activeTab = signal<StatsTab>('table');
  isLoading = signal(false);
  standings = signal<TournamentStandingDto[]>([]);
  goalStandings = signal<PlayerGoalStandingDto[]>([]);
  assistStandings = signal<PlayerAssistStandingDto[]>([]);

  tournamentName = computed(() => {
    return this.tournament()?.name || 'Tournament Statistics';
  });

  goBack() {
    if (this.backRoute()) {
      this.router.navigate([this.backRoute()]);
    } else {
      globalThis.history.back();
    }
  }

  topScorer = computed(() => {
    const goals = this.goalStandings();
    return goals.length > 0 ? goals[0] : null;
  });

  topAssister = computed(() => {
    const assists = this.assistStandings();
    return assists.length > 0 ? assists[0] : null;
  });

  totalGoals = computed(() => {
    return this.goalStandings().reduce(
      (total, player) => total + player.goals,
      0
    );
  });

  totalMatches = computed(() => {
    return (
      this.standings().reduce((total, team) => total + team.matchesPlayed, 0) /
      2
    );
  });

  ngOnInit() {
    const routeTournamentId = this.route.snapshot.paramMap.get('id');
    if (routeTournamentId) {
      this._tournamentId.set(routeTournamentId);
    }

    this.loadAllStatistics();
  }

  loadAllStatistics() {
    this.isLoading.set(true);
    Promise.all([
      this.loadStandings(),
      this.loadGoalStandings(),
      this.loadAssistStandings(),
    ]).finally(() => {
      this.isLoading.set(false);
    });
  }

  private loadStandings(): Promise<void> {
    return new Promise((resolve) => {
      this.tournamentService
        .getTournamentStandings(this.getTournamentId())
        .subscribe({
          next: (data) => {
            this.standings.set(data);
            resolve();
          },
          error: (error) => {
            resolve();
          },
        });
    });
  }

  private loadGoalStandings(): Promise<void> {
    return new Promise((resolve) => {
      this.tournamentService
        .getPlayerGoalStandings(this.getTournamentId())
        .subscribe({
          next: (data) => {
            this.goalStandings.set(data);
            resolve();
          },
          error: (error) => {
            resolve();
          },
        });
    });
  }

  private loadAssistStandings(): Promise<void> {
    return new Promise((resolve) => {
      this.tournamentService
        .getPlayerAssistStandings(this.getTournamentId())
        .subscribe({
          next: (data) => {
            this.assistStandings.set(data);
            resolve();
          },
          error: (error) => {
            resolve();
          },
        });
    });
  }

  onTabChange(tab: StatsTab) {
    this.activeTab.set(tab);
  }

  getTabClass(tab: StatsTab): string {
    const baseClass =
      'px-2 py-1 font-semibold border-b-2 cursor-pointer sm:text-md text-xs ';
    const activeClass = 'border-primary text-primary';
    const inactiveClass = 'border-transparent text-muted';

    return baseClass + (this.activeTab() === tab ? activeClass : inactiveClass);
  }

  getTeamInitials(teamName: string): string {
    return teamName
      .split(' ')
      .map((word) => word.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 3);
  }

  getPlayerInitials(playerName: string): string {
    const names = playerName.split(' ');
    if (names.length >= 2) {
      return `${names[0].charAt(0)}${names[names.length - 1].charAt(
        0
      )}`.toUpperCase();
    }
    return playerName.slice(0, 2).toUpperCase();
  }

  getPositionClass(position: number): string {
    switch (position) {
      case 1:
        return 'text-yellow-600 font-bold';
      case 2:
        return 'text-muted font-bold';
      case 3:
        return 'text-orange-600 font-bold';
      default:
        return 'text-muted';
    }
  }

  getPositionIcon(position: number): any {
    switch (position) {
      case 1:
        return this.icons.Trophy;
      case 2:
        return this.icons.Crown;
      case 3:
        return this.icons.Target;
      default:
        return null;
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  refresh() {
    this.loadAllStatistics();
  }
}
