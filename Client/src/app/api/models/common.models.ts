export interface PaginationMeta {
  page: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PagedResult<T> {
  data: T[];
  meta: PaginationMeta;
  totalRecords?: number;
}

export interface PaginationParams {
  page?: number;
  pageSize?: number;
}

export interface SearchParams extends PaginationParams {
  q?: string;
  filters?: Record<string, any>;
}

export enum UserRoles {
  Admin = 1,
  Organizer = 2,
  User = 3
}

export enum ImageType {
  ProfilePicture = 1,
  TournamentBanner = 2,
  TournamentLogo = 3,
  TeamLogo = 4,
  TeamPhoto = 5,
  PlayerPhoto = 6
}

export enum TournamentStatus {
  Draft = 1,
  RegistrationOpen = 2,
  RegistrationClosed = 3,
  InProgress = 4,
  Completed = 5,
  Cancelled = 6
}

export enum TournamentFormats {
  SingleElimination = 1,
  RoundRobin = 2
}

export enum TeamRegistrationStatus {
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  Withdrawn = 4
}

export enum MatchStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
  Postponed = 4,
  Cancelled = 5
}

export enum MatchEventType {
  Goal = 1,
  Assist = 2,
  YellowCard = 3,
  RedCard = 4,
  Foul = 5,
  Substitution = 6
}

export enum PlayerPosition {
  Goalkeeper = 1,
  Defender = 2,
  Midfielder = 3,
  Forward = 4
}

export enum TeamAction {
  Register = 1,
  Withdraw = 2,
  Approve = 3,
  Reject = 4
}
