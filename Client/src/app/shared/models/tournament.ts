export interface CreateTeamRequest {
  name: string;
  manager: string;
  numberOfPlayers: number;
  logoUrl?: string;
  tournamentId: string;
}

export interface UpdateTeamRequest {
  name: string;
  manager: string;
  numberOfPlayers: number;
  logoUrl?: string;
}

export interface TeamSearchRequest {
  searchTerm?: string;
  tournamentId?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface Team {
  id: string;
  name: string;
  manager: string;
  numberOfPlayers: number;
  logoUrl?: string;
  tournamentId: string;
  tournamentName: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTournamentRequest {
  name: string;
  description: string;
  location?: string;
  registrationDeadline?: string;
  startDate: string;
  endDate: string;
  maxTeams: number;
  entryFee?: number;
  prize?: number;
  contactEmail?: string;
  bannerUrl?: string;
}

export interface UpdateTournamentRequest {
  name: string;
  description: string;
  location?: string;
  registrationDeadline?: string;
  startDate: string;
  endDate: string;
  maxTeams: number;
  entryFee?: number;
  prize?: number;
  contactEmail?: string;
  bannerUrl?: string;
  status?: string;
}

export interface TournamentSearchRequest {
  searchTerm?: string;
  status?: string;
  location?: string;
  startDateFrom?: string;
  startDateTo?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface Tournament {
  id: string;
  name: string;
  description: string;
  location?: string;
  registrationDeadline?: string;
  startDate: string;
  endDate: string;
  maxTeams: number;
  entryFee?: number;
  prize?: number;
  contactEmail?: string;
  bannerUrl?: string;
  status: string;
  organizer: string;
  organizerName: string;
  createdAt: string;
  teamCount: number;
}

export interface TeamFormData {
  name: string;
  city: string;
  coach: string;
  players: number;
  logoUrl?: string;
}

export interface TournamentFormData {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  maxTeams: number;
  location?: string;
  registrationDeadline?: string;
  entryFee?: number;
  prizePool?: number;
  contactEmail?: string;
}
