import { Injectable } from '@angular/core';
import { Tournament } from '../models/tournament';

@Injectable({
  providedIn: 'root',
})
export class TournamentService {
  private tournaments: Tournament[] = [];

  getTournaments() {
    return this.tournaments;
  }

  createTournament(tournament: Tournament) {
    this.tournaments.push(tournament);
  }

  updateTournament(updatedTournament: Tournament) {
    const index = this.tournaments.findIndex(
      (t) => t.id === updatedTournament.id
    );
    if (index !== -1) {
      this.tournaments[index] = updatedTournament;
    }
  }

  deleteTournament(id: string) {
    this.tournaments = this.tournaments.filter((t) => t.id !== id);
  }
}
