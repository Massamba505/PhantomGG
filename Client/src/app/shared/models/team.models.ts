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
  registrationDate: Date;
  approvedDate?: Date;
  numberOfPlayers: number;
  createdAt: Date;
  updatedAt?: Date;
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
  name: string;
  shortName?: string;
  managerName: string;
  managerEmail: string;
  managerPhone?: string;
  logoUrl?: string;
  teamPhotoUrl?: string;
  registrationStatus?: string;
}

export interface TeamSearch {
  searchTerm?: string;
  tournamentId?: string;
  registrationStatus?: string;
  managerEmail?: string;
  pageNumber?: number;
  pageSize?: number;
}
