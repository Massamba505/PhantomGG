export interface Team {
  id: string;
  name: string;
  shortName?: string;
  managerName: string;
  managerEmail: string;
  managerPhone?: string;
  logoUrl?: string;
  teamPhotoUrl?: string;
  tournamentId: string;
  tournamentName: string;
  registrationStatus: string;
  registrationDate: string;
  approvedDate?: string | null;
  numberOfPlayers: number;
  createdAt: string;
  updatedAt?: string | null;
  isActive: boolean;
}

export interface CreateTeam {
  name: string;
  shortName?: string;
  managerName: string;
  managerEmail: string;
  managerPhone?: string;
  logoUrl?: string;
  teamPhotoUrl?: string;
  tournamentId: string;
}

export interface UpdateTeam {
  name?: string;
  shortName?: string;
  managerName?: string;
  managerEmail?: string;
  managerPhone?: string;
  logoUrl?: string;
  teamPhotoUrl?: string;
}

export interface TeamSearch {
  searchTerm?: string;
  tournamentId?: string;
  registrationStatus?: string;
  isActive?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
