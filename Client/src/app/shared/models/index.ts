export * from './api.models';

export * from './auth.models';
export * from './tournament.models';
export * from './team.models';
export * from './player.models';
export * from './match.models';

export type { ApiResponse as LegacyApiResponse } from './ApiResponse';
export type { LoginRequest as LegacyLoginRequest, SignUpRequest, AuthResponse as LegacyAuthResponse } from './Authentication';
export type { 
  Tournament as LegacyTournament, 
  Team as LegacyTeam,
  CreateTournamentRequest as LegacyCreateTournamentRequest,
  UpdateTournamentRequest as LegacyUpdateTournamentRequest,
  TournamentSearchRequest as LegacyTournamentSearchRequest,
  CreateTeamRequest as LegacyCreateTeamRequest,
  UpdateTeamRequest as LegacyUpdateTeamRequest,
  TeamSearchRequest as LegacyTeamSearchRequest
} from './tournament';
export type { User as LegacyUser, UpdateProfileRequest, ChangePasswordRequest as LegacyChangePasswordRequest } from './User';
