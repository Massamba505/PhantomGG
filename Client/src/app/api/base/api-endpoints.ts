export const API_ENDPOINTS = {
  AUTH: {
    REGISTER: 'auth/register',
    LOGIN: 'auth/login',
    REFRESH: 'auth/refresh',
    ME: 'auth/me',
    LOGOUT: 'auth/logout',
    VERIFY_EMAIL: 'auth/verify-email',
    RESEND_VERIFICATION: 'auth/resend-verification',
    FORGOT_PASSWORD: 'auth/forgot-password',
    RESET_PASSWORD: 'auth/reset-password'
  },

  USERS: {
    PROFILE: 'users/profile',
    UPDATE_PROFILE: 'users/profile',
    CHANGE_PASSWORD: 'users/change-password',
    UPLOAD_PROFILE_PICTURE: 'users/profile-picture'
  },

  TOURNAMENTS: {
    LIST: 'tournaments',
    GET: (tournamentId: string) => `tournaments/${tournamentId}`,
    TEAMS: (tournamentId: string) => `tournaments/${tournamentId}/teams`,
    MATCHES: (tournamentId: string) => `tournaments/${tournamentId}/matches`,
    STATISTICS: (tournamentId: string) => `tournaments/${tournamentId}/statistics`,
    STANDINGS: (tournamentId: string) => `tournaments/${tournamentId}/standings`,
    CREATE: 'tournaments',
    UPDATE: (tournamentId: string) => `tournaments/${tournamentId}`,
    DELETE: (tournamentId: string) => `tournaments/${tournamentId}`,
    MANAGE_TEAM: (tournamentId: string, teamId?: string) => 
      teamId ? `tournaments/${tournamentId}/teams/${teamId}` : `tournaments/${tournamentId}/teams`,
    CREATE_BRACKET: (tournamentId: string) => `tournaments/${tournamentId}/bracket`
  },

  TEAMS: {
    LIST: 'teams',
    GET: (teamId: string) => `teams/${teamId}`,
    PLAYERS: (teamId: string) => `teams/${teamId}/players`,
    CREATE: 'teams',
    UPDATE: (teamId: string) => `teams/${teamId}`,
    DELETE: (teamId: string) => `teams/${teamId}`,
    ADD_PLAYER: (teamId: string) => `teams/${teamId}/players`,
    UPDATE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`,
    REMOVE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`
  }
} as const;