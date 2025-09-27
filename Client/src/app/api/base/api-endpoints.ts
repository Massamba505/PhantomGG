export const API_ENDPOINTS = {
  AUTH: {
    REGISTER: 'auth/register',
    LOGIN: 'auth/login',
    REFRESH: 'auth/refresh',
    ME: 'auth/me',
    LOGOUT: 'auth/logout'
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
    MY_TOURNAMENTS: 'tournaments/my-tournaments',
    REGISTER: (tournamentId: string) => `tournaments/${tournamentId}/register`,
    WITHDRAW: (tournamentId: string) => `tournaments/${tournamentId}/withdraw`,
    CREATE: 'tournaments',
    UPDATE: (tournamentId: string) => `tournaments/${tournamentId}`,
    DELETE: (tournamentId: string) => `tournaments/${tournamentId}`,
    UPLOAD_BANNER: (tournamentId: string) => `tournaments/${tournamentId}/image/banner`,
    UPLOAD_LOGO: (tournamentId: string) => `tournaments/${tournamentId}/image/logo`,
    PENDING_TEAMS: (tournamentId: string) => `tournaments/${tournamentId}/teams/pending`,
    APPROVE_TEAM: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}/approve`,
    REJECT_TEAM: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}/reject`,
    REMOVE_TEAM: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}`,
    // Public endpoints
    PUBLIC: 'tournaments/public',
    PUBLIC_DETAILS: (tournamentId: string) => `tournaments/public/${tournamentId}`,
    PUBLIC_TEAMS: (tournamentId: string) => `tournaments/public/${tournamentId}/teams`,
    PUBLIC_STATISTICS: (tournamentId: string) => `tournaments/public/${tournamentId}/statistics`,
    PUBLIC_MATCHES: (tournamentId: string) => `tournaments/public/${tournamentId}/matches`
  },

  TEAMS: {
    LIST: 'teams',
    GET: (teamId: string) => `teams/${teamId}`,
    PLAYERS: (teamId: string) => `teams/${teamId}/players`,
    MY_TEAMS: 'teams/my-teams',
    CREATE: 'teams',
    UPDATE: (teamId: string) => `teams/${teamId}`,
    DELETE: (teamId: string) => `teams/${teamId}`,
    UPLOAD_LOGO: (teamId: string) => `teams/${teamId}/logo`,
    ADD_PLAYER: (teamId: string) => `teams/${teamId}/players`,
    UPDATE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`,
    REMOVE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`
  }
} as const;