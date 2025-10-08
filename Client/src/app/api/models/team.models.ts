export interface PlayerDto {
  id: string;
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: string;
  teamId: string;
  teamName: string;
  joinedAt: string;
}

export interface CreatePlayerDto {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: File;
  teamId: string;
}

export interface UpdatePlayerDto {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: File;
}

export interface TeamDto {
  id: string;
  name: string;
  shortName: string;
  logoUrl?: string;
  userId: string;
  createdAt: string;
  updatedAt?: string;
  players: PlayerDto[];
}

export interface TournamentTeamDto {
  id: string;
  name: string;
  shortName?: string;
  logoUrl?: string;
  status: string;
  registeredAt: string;
  acceptedAt?: string;
  managerName?: string;
  managerId?: string;
  players: PlayerDto[];
}

export interface CreateTeamDto {
  name: string;
  shortName?: string;
  logoUrl?: File;
  teamPhotoUrl?: File;
}

export interface UpdateTeamDto {
  name: string;
  shortName?: string;
  logoUrl?: File;
  teamPhotoUrl?: File;
}

export interface TeamQuery {
  q?: string;
  tournamentId?: string;
  status?: string;
  page: number;
  pageSize: number;
  sort?: string;
  isPublic?: boolean;
}

export type Team = TeamDto;
export type TournamentTeam = TournamentTeamDto;
export type CreateTeam = CreateTeamDto;
export type UpdateTeam = UpdateTeamDto;
export type TeamSearch = TeamQuery;
export type Player = PlayerDto;
export type CreatePlayer = CreatePlayerDto;
export type UpdatePlayer = UpdatePlayerDto;
