import {
  Component,
  OnInit,
  inject,
  input,
  output,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { Modal } from '@/app/shared/components/ui/modal/modal';
import { MatchService } from '@/app/api/services/match.service';
import {
  Match,
  MatchEvent,
  MatchEventType,
} from '@/app/api/models/match.models';
import { Player } from '@/app/api/models/team.models';
import { TeamService } from '@/app/api/services/team.service';
import { getEnumLabel } from '@/app/shared/utils/enumConvertor';
import { MatchStatus, PlayerPosition } from '@/app/api/models';

@Component({
  selector: 'app-match-details-modal',
  imports: [CommonModule, LucideAngularModule, Modal],
  templateUrl: './match-details-modal.html',
  styleUrl: './match-details-modal.css',
})
export class MatchDetailsModalComponent implements OnInit {
  private matchService = inject(MatchService);
  private teamService = inject(TeamService);

  isOpen = input<boolean>(false);
  matchId = input<string>('');
  close = output<void>();

  match = signal<Match | null>(null);
  matchEvents = signal<MatchEvent[]>([]);
  homeTeamPlayers = signal<Player[]>([]);
  awayTeamPlayers = signal<Player[]>([]);
  loading = signal(false);

  icons = LucideIcons;

  getPosition(position: number) {
    return getEnumLabel(PlayerPosition, position);
  }

  ngOnInit() {
    this.loadMatchDetails();
  }

  ngOnChanges() {
    if (this.isOpen() && this.matchId()) {
      this.loadMatchDetails();
    }
  }

  loadMatchDetails() {
    if (!this.matchId() || !this.isOpen()) return;

    this.loading.set(true);

    this.matchService.getMatch(this.matchId()).subscribe({
      next: (match) => {
        this.match.set(match);
        this.loadTeamPlayers();
        this.loadMatchEvents();
      },
      error: (error) => {
        console.error('Error loading match:', error);
        this.loading.set(false);
      },
    });
  }

  loadTeamPlayers() {
    const match = this.match();
    if (!match) return;

    this.teamService.getTeamPlayers(match.homeTeamId).subscribe({
      next: (players) => {
        this.homeTeamPlayers.set(players);
      },
      error: (error) =>
        console.error('Error loading home team players:', error),
    });

    this.teamService.getTeamPlayers(match.awayTeamId).subscribe({
      next: (players) => {
        this.awayTeamPlayers.set(players);
      },
      error: (error) =>
        console.error('Error loading away team players:', error),
    });
  }

  loadMatchEvents() {
    if (!this.matchId()) return;

    this.matchService.getMatchEvents(this.matchId()).subscribe({
      next: (events) => {
        this.matchEvents.set(events.sort((a, b) => a.minute - b.minute));
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading match events:', error);
        this.loading.set(false);
      },
    });
  }

  isInProgress(): boolean {
    const matchStatus = this.match()!.status;
    return matchStatus === MatchStatus.InProgress;
  }

  isCompleted(): boolean {
    const matchStatus = this.match()!.status;
    return matchStatus === MatchStatus.Completed;
  }

  getScoreDisplay(): string {
    if (this.isCompleted() || this.isInProgress()) {
      const homeScore = this.match()!.homeScore ?? 0;
      const awayScore = this.match()!.awayScore ?? 0;
      return `${homeScore} - ${awayScore}`;
    }
    return 'vs';
  }

  onClose() {
    this.close.emit();
    this.match.set(null);
    this.matchEvents.set([]);
    this.homeTeamPlayers.set([]);
    this.awayTeamPlayers.set([]);
  }

  getEventIcon(eventType: string | any): any {
    const eventTypeString =
      typeof eventType === 'string' ? eventType : eventType.toString();
    switch (eventTypeString.toLowerCase()) {
      case 'goal':
        return this.icons.Target;
      case 'yellowcard':
        return this.icons.AlertTriangle;
      case 'redcard':
        return this.icons.XCircle;
      case 'substitution':
        return this.icons.RefreshCw;
      case 'foul':
        return this.icons.AlertTriangle;
      default:
        return this.icons.CheckCircle;
    }
  }

  getEventIconClass(eventType: MatchEventType): string {
    switch (eventType) {
      case MatchEventType.Goal:
        return 'text-green-600';
      case MatchEventType.YellowCard:
        return 'text-yellow-600';
      case MatchEventType.RedCard:
        return 'text-red-600';
      case MatchEventType.Foul:
        return 'text-red-600';
      case MatchEventType.Substitution:
        return 'text-gray-600';
      default:
        return 'text-primary';
    }
  }

  formatEventType(eventType: string | number): string {
    return getEnumLabel(MatchEventType, eventType) || eventType.toString();
  }

  getEventColor(eventType: string | any): string {
    const eventTypeString =
      typeof eventType === 'string' ? eventType : eventType.toString();
    switch (eventTypeString.toLowerCase()) {
      case 'goal':
        return 'text-green-600';
      case 'yellowcard':
        return 'text-yellow-600';
      case 'redcard':
        return 'text-red-600';
      case 'substitution':
        return 'text-primary';
      case 'foul':
        return 'text-orange-600';
      default:
        return 'text-gray-600';
    }
  }

  formatPlayerName(player: Player): string {
    return `${player.firstName} ${player.lastName}`;
  }
}
