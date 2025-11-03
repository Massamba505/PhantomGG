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
    CHANGE_PASSWORD: 'users/password',
    UPLOAD_PROFILE_PICTURE: 'users/profile-picture'
  },

  TOURNAMENTS: {
    LIST: 'tournaments',
    GET: (tournamentId: string) => `tournaments/${tournamentId}`,
    CREATE: 'tournaments',
    UPDATE: (tournamentId: string) => `tournaments/${tournamentId}`,
    DELETE: (tournamentId: string) => `tournaments/${tournamentId}`,
    
    TEAMS: (tournamentId: string) => `tournaments/${tournamentId}/teams`,
    REGISTER_TEAM: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}`,
    MANAGE_TEAM: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}`,
    
    MATCHES: (tournamentId: string) => `tournaments/${tournamentId}/matches`,
    STANDINGS: (tournamentId: string) => `tournaments/${tournamentId}/standings`,
    GOAL_STANDINGS: (tournamentId: string) => `tournaments/${tournamentId}/standings/goals`,
    ASSIST_STANDINGS: (tournamentId: string) => `tournaments/${tournamentId}/standings/assists`,
    
    GENERATE_FIXTURES: (tournamentId: string) => `tournaments/${tournamentId}/fixtures`
  },

  TEAMS: {
    LIST: 'teams',
    GET: (teamId: string) => `teams/${teamId}`,
    CREATE: 'teams',
    UPDATE: (teamId: string) => `teams/${teamId}`,
    DELETE: (teamId: string) => `teams/${teamId}`,
    
    PLAYERS: (teamId: string) => `teams/${teamId}/players`,
    ADD_PLAYER: (teamId: string) => `teams/${teamId}/players`,
    UPDATE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`,
    REMOVE_PLAYER: (teamId: string, playerId: string) => `teams/${teamId}/players/${playerId}`
  },

  MATCHES: {
    LIST: 'matches',
    GET: (matchId: string) => `matches/${matchId}`,
    CREATE: 'matches',
    UPDATE: (matchId: string) => `matches/${matchId}`,
    DELETE: (matchId: string) => `matches/${matchId}`,
    UPDATE_RESULT: (matchId: string) => `matches/${matchId}/result`,
    
    EVENTS: {
      LIST_BY_MATCH: (matchId: string) => `matches/${matchId}/events`,
      CREATE: (matchId: string) => `matches/${matchId}/events`,
      GET: (matchId: string, eventId: string) => `matches/${matchId}/events/${eventId}`,
      UPDATE: (matchId: string, eventId: string) => `matches/${matchId}/events/${eventId}`,
      DELETE: (matchId: string, eventId: string) => `matches/${matchId}/events/${eventId}`,
      PLAYER_EVENTS: (playerId: string) => `matches/player/${playerId}/events`,
      TEAM_EVENTS: (teamId: string) => `matches/team/${teamId}/events`
    }
  }
} as const;