export interface ApiResponse<T = any> {
  success: boolean;
  message: string;
  details?: string;
  data?: T;
}


export type UserRole = 'Admin' | 'Organizer' | 'User';
export type TournamentStatus = 'Draft' | 'RegistrationOpen' | 'RegistrationClosed' | 'InProgress' | 'Completed' | 'Cancelled' | 'Postponed';
export type MatchStatus = 'Scheduled' | 'InProgress' | 'Completed' | 'Cancelled' | 'Postponed';
export type TeamRegistrationStatus = 'Pending' | 'Approved' | 'Rejected' | 'Withdrawn';
