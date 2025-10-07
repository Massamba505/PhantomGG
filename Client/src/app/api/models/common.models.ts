export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
}

export interface SearchParams extends PaginationParams {
  searchTerm?: string;
  filters?: Record<string, any>;
}

export type UserRole = 'Admin' | 'Organizer' | 'User';

export type ImageType = 
  | 'ProfilePicture'
  | 'TournamentBanner'
  | 'TournamentLogo'
  | 'TeamLogo'
  | 'TeamPhoto'
  | 'PlayerPhoto';

export type TournamentStatus = 
  | 'Draft'
  | 'RegistrationOpen'
  | 'RegistrationClosed'
  | 'InProgress'
  | 'Completed'
  | 'Cancelled';

export type TeamRegistrationStatus = 
  | 'Pending'
  | 'Approved'
  | 'Rejected'
  | 'Withdrawn';

export type MatchStatus = 
  | 'Scheduled'
  | 'InProgress'
  | 'Completed'
  | 'Postponed'
  | 'Cancelled';

export type PlayerPosition = 
  | 'Goalkeeper'
  | 'Defender'
  | 'Midfielder'
  | 'Forward';
