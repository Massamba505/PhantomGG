import { Player } from '../models/player.models';

export class PlayerUtils {
  static getFullName(player: Player | null | undefined): string {
    if (!player) return 'Unknown Player';
    return player.fullName || `${player.firstName} ${player.lastName}`.trim() || 'Unnamed Player';
  }

  static getDisplayName(player: Player | null | undefined): string {
    if (!player) return 'Unknown';
    if (player.lastName) {
      return `${player.firstName} ${player.lastName.charAt(0)}.`;
    }
    return player.firstName || 'Unknown';
  }

  static getInitials(player: Player | null | undefined): string {
    if (!player) return 'P';
    const firstInitial = player.firstName?.charAt(0)?.toUpperCase() || '';
    const lastInitial = player.lastName?.charAt(0)?.toUpperCase() || '';
    return `${firstInitial}${lastInitial}` || 'P';
  }

  static getDisplayPosition(player: Player | null | undefined): string {
    return player?.position || 'No position';
  }

  static hasPhoto(player: Player | null | undefined): boolean {
    return !!(player?.photoUrl && player.photoUrl.trim() !== '');
  }

  static hasEmail(player: Player | null | undefined): boolean {
    return !!(player?.email && player.email.trim() !== '');
  }

  static getContactInfo(player: Player | null | undefined): string {
    return player?.email || 'No contact';
  }

  static isActive(player: Player | null | undefined): boolean {
    return player?.isActive === true;
  }

  static getPositionColor(position: string | undefined): string {
    switch (position?.toLowerCase()) {
      case 'goalkeeper': return 'purple';
      case 'defender': return 'blue';
      case 'midfielder': return 'green';
      case 'forward': return 'red';
      case 'substitute': return 'orange';
      default: return 'gray';
    }
  }

  static getPositionAbbreviation(position: string | undefined): string {
    switch (position?.toLowerCase()) {
      case 'goalkeeper': return 'GK';
      case 'defender': return 'DEF';
      case 'midfielder': return 'MID';
      case 'forward': return 'FWD';
      case 'substitute': return 'SUB';
      default: return position?.substring(0, 3).toUpperCase() || 'N/A';
    }
  }

  static sortByPosition(players: Player[]): Player[] {
    const positionOrder = ['goalkeeper', 'defender', 'midfielder', 'forward', 'substitute'];
    return [...players].sort((a, b) => {
      const aIndex = positionOrder.indexOf(a.position?.toLowerCase() || '');
      const bIndex = positionOrder.indexOf(b.position?.toLowerCase() || '');
      if (aIndex === -1 && bIndex === -1) return 0;
      if (aIndex === -1) return 1;
      if (bIndex === -1) return -1;
      return aIndex - bIndex;
    });
  }

  static groupByPosition(players: Player[]): Record<string, Player[]> {
    return players.reduce((groups, player) => {
      const position = player.position || 'Unknown';
      if (!groups[position]) {
        groups[position] = [];
      }
      groups[position].push(player);
      return groups;
    }, {} as Record<string, Player[]>);
  }
}
