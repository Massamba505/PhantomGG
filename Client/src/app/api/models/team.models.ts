export interface Team {
  id: string;
  name: string;
  shortName?: string;
  logoUrl?: string;
  userId: string;
  createdAt: string;
  updatedAt?: string;
  players?:Player[]
}

export interface TournamentTeam {
  id: string;
  name: string;
  shortName?: string;
  logoUrl?: string;
  status: 'Approved' | 'Pending' | 'Rejected' | 'RegistrationOpen';
  registeredAt: string;
  managerName?: string;
  managerId?: string;
  players?:Player[]
}

export interface CreateTeam {
  name: string;
  shortName?: string;
  logoUrl?: File;
  teamPhotoUrl?: File;
}

export interface UpdateTeam {
  name: string;
  shortName?: string;
  logoUrl?: File;
  teamPhotoUrl?: File;
}

export interface TeamSearch {
  searchTerm?: string;
  scope?: 'public' | 'my' | 'all';
  pageNumber?: number;
  pageSize?: number;
}

export interface Player {
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

export interface CreatePlayer {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: File;
  teamId: string;
}

export interface UpdatePlayer {
  firstName: string;
  lastName: string;
  position?: string;
  email?: string;
  photoUrl?: File;
}
