export interface TeamFormData {
  name: string;
  city: string;
  coach: string;
  players: number;
  logoUrl?: string
}

export interface Team extends TeamFormData {
  id: string;
  tournamentId: string;
  createdAt: string;
}

export interface TournamentFormData {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  maxTeams: number;
  location?: string;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
}

export interface Tournament extends TournamentFormData {
  id: string;
  status: 'draft' | 'active' | 'completed';
  createdAt: string;
  teams: Team[];
}
