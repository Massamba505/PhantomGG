export interface Team {
  id: string;
  name: string;
  shortName?: string;
  logoUrl?: string;
  userId: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateTeam {
  name: string;
  shortName?: string;
  managerName: string;
  logoUrl?: File;
  teamPhotoUrl?: File;
  tournamentId: string;
}

export interface UpdateTeam {
  name?: string;
  shortName?: string;
  logoUrl?: string;
}

export interface TeamSearch {
  searchTerm?: string;
  pageNumber?: number;
  pageSize?: number;
}
