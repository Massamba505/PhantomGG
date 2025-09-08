export interface Player {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  position?: string;
  email?: string;
  photoUrl?: string;
  teamId: string;
  teamName: string;
  createdAt: Date;
  updatedAt?: Date;
  isActive: boolean;
}

export interface CreatePlayer {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: string;
  teamId: string;
}

export interface UpdatePlayer {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: string;
}

export interface PlayerSearch {
  searchTerm?: string;
  teamId?: string;
  tournamentId?: string;
  position?: string;
  pageNumber?: number;
  pageSize?: number;
}
