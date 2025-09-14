export const API_ENDPOINTS = {
  AUTH: {
    REGISTER: 'auth/register',
    LOGIN: 'auth/login',
    REFRESH: 'auth/refresh',
    ME: 'auth/me',
    LOGOUT: 'auth/logout'
  },

  ORGANIZERS: {
    PROFILE: 'organizers/profile',
    DASHBOARD: 'organizers/dashboard',
    STATISTICS: 'organizers/statistics',

    TOURNAMENTS: {
      LIST: 'organizers/tournaments',
      GET: (id: string) => `organizers/tournaments/${id}`,
      CREATE: 'organizers/tournaments',
      UPDATE: (id: string) => `organizers/tournaments/${id}`,
      DELETE: (id: string) => `organizers/tournaments/${id}`,
      SEARCH: 'organizers/tournaments/search',
      PUBLISH: (id: string) => `organizers/tournaments/${id}/publish`,
      UNPUBLISH: (id: string) => `organizers/tournaments/${id}/unpublish`,
      SETTINGS: (id: string) => `organizers/tournaments/${id}/settings`,
      STATISTICS: (id: string) => `organizers/tournaments/${id}/statistics`,
      UPLOAD_BANNER: (id: string) => `organizers/tournaments/${id}/banner`,
      UPLOAD_LOGO: (id: string) => `organizers/tournaments/${id}/logo`,
      DASHBOARD_DATA: (id: string) => `organizers/tournaments/${id}/dashboard`
    },

    TEAMS: {
      LIST: (tournamentId: string) => `organizers/tournaments/${tournamentId}/teams`,
      GET: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}`,
      APPROVE: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/approve`,
      REJECT: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/reject`,
      REMOVE: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}`,
      UPDATE: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}`,
      SEARCH: (tournamentId: string) => `organizers/tournaments/${tournamentId}/teams/search`,
      BY_STATUS: (tournamentId: string, status: string) => `organizers/tournaments/${tournamentId}/teams/status/${status}`,
      PENDING: (tournamentId: string) => `organizers/tournaments/${tournamentId}/teams/pending`,
      APPROVED: (tournamentId: string) => `organizers/tournaments/${tournamentId}/teams/approved`,
      REJECTED: (tournamentId: string) => `organizers/tournaments/${tournamentId}/teams/rejected`
    },

    PLAYERS: {
      LIST: (tournamentId: string) => `organizers/tournaments/${tournamentId}/players`,
      BY_TEAM: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/players`,
      GET: (tournamentId: string, teamId: string, playerId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/players/${playerId}`,
      ADD: (tournamentId: string, teamId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/players`,
      UPDATE: (tournamentId: string, teamId: string, playerId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/players/${playerId}`,
      REMOVE: (tournamentId: string, teamId: string, playerId: string) => `organizers/tournaments/${tournamentId}/teams/${teamId}/players/${playerId}`,
      SEARCH: (tournamentId: string) => `organizers/tournaments/${tournamentId}/players/search`,
      STATISTICS: (tournamentId: string, playerId: string) => `organizers/tournaments/${tournamentId}/players/${playerId}/statistics`,
      TOP_SCORERS: (tournamentId: string) => `organizers/tournaments/${tournamentId}/top-scorers`,
      TOP_ASSISTS: (tournamentId: string) => `organizers/tournaments/${tournamentId}/top-assists`
    },

    MATCHES: {
      LIST: (tournamentId: string) => `organizers/tournaments/${tournamentId}/matches`,
      GET: (tournamentId: string, matchId: string) => `organizers/tournaments/${tournamentId}/matches/${matchId}`,
      CREATE: (tournamentId: string) => `organizers/tournaments/${tournamentId}/matches`,
      UPDATE: (tournamentId: string, matchId: string) => `organizers/tournaments/${tournamentId}/matches/${matchId}`,
      DELETE: (tournamentId: string, matchId: string) => `organizers/tournaments/${tournamentId}/matches/${matchId}`,
      UPDATE_RESULT: (tournamentId: string, matchId: string) => `organizers/tournaments/${tournamentId}/matches/${matchId}/result`,
      GENERATE_FIXTURES: (tournamentId: string) => `organizers/tournaments/${tournamentId}/matches/generate`,
      AUTO_GENERATE: (tournamentId: string) => `organizers/tournaments/${tournamentId}/matches/auto-generate`,
      FIXTURE_STATUS: (tournamentId: string) => `organizers/tournaments/${tournamentId}/matches/fixture-status`
    }
  },

  USERS: {
    PROFILE: 'users/profile',
    UPDATE_PROFILE: 'users/profile',
    CHANGE_PASSWORD: 'users/change-password',
    UPLOAD_PROFILE_PICTURE: 'users/profile-picture',
    DASHBOARD: 'users/dashboard',
    STATISTICS: 'users/statistics',

    TEAMS: {
      LIST: 'users/teams',
      GET: (id: string) => `users/teams/${id}`,
      CREATE: 'users/teams',
      UPDATE: (id: string) => `users/teams/${id}`,
      DELETE: (id: string) => `users/teams/${id}`,
      SEARCH: 'users/teams/search',
      UPLOAD_LOGO: (id: string) => `users/teams/${id}/logo`,
      STATISTICS: (id: string) => `users/teams/${id}/statistics`,
      MATCHES: (id: string) => `users/teams/${id}/matches`,
      UPCOMING_MATCHES: (id: string) => `users/teams/${id}/matches/upcoming`,
      MATCH_HISTORY: (id: string) => `users/teams/${id}/matches/history`
    },

    PLAYERS: {
      LIST: (teamId: string) => `users/teams/${teamId}/players`,
      GET: (teamId: string, playerId: string) => `users/teams/${teamId}/players/${playerId}`,
      ADD: (teamId: string) => `users/teams/${teamId}/players`,
      UPDATE: (teamId: string, playerId: string) => `users/teams/${teamId}/players/${playerId}`,
      REMOVE: (teamId: string, playerId: string) => `users/teams/${teamId}/players/${playerId}`,
      SEARCH: (teamId: string) => `users/teams/${teamId}/players/search`,
      PROFILE: (playerId: string) => `users/players/${playerId}/profile`,
      STATISTICS: (playerId: string) => `users/players/${playerId}/statistics`
    },

    TOURNAMENTS: {
      REGISTER: 'users/tournaments/register',
      APPLICATIONS: 'users/tournaments/applications',
      UPCOMING: 'users/tournaments/upcoming',
      HISTORY: 'users/tournaments/history',
      CURRENT: 'users/tournaments/current',
      WITHDRAW: (tournamentId: string, teamId: string) => `users/tournaments/${tournamentId}/teams/${teamId}/withdraw`,
      APPLICATION_STATUS: (tournamentId: string, teamId: string) => `users/tournaments/${tournamentId}/teams/${teamId}/status`
    }
  },

  TOURNAMENTS: {
    LIST: 'tournaments',
    SEARCH: 'tournaments/search',
    FEATURED: 'tournaments/featured',
    UPCOMING: 'tournaments/upcoming',
    LIVE: 'tournaments/live',
    RECENT: 'tournaments/recent',
    BY_LOCATION: (location: string) => `tournaments/location/${location}`,
    BY_FORMAT: (formatId: string) => `tournaments/format/${formatId}`,
    FORMATS: 'tournaments/formats',

    GET: (id: string) => `tournaments/${id}`,
    DETAILS: (id: string) => `tournaments/${id}/details`,
    RULES: (id: string) => `tournaments/${id}/rules`,
    STATISTICS: (id: string) => `tournaments/${id}/statistics`,
    
    TEAMS: (id: string) => `tournaments/${id}/teams`,
    STANDINGS: (id: string) => `tournaments/${id}/standings`,
    SCHEDULE: (id: string) => `tournaments/${id}/schedule`,
    MATCHES: (id: string) => `tournaments/${id}/matches`,
    RESULTS: (id: string) => `tournaments/${id}/results`,
    FIXTURES: (id: string) => `tournaments/${id}/fixtures`,
    
    TOP_SCORERS: (id: string) => `tournaments/${id}/players/top-scorers`,
    TOP_ASSISTS: (id: string) => `tournaments/${id}/players/top-assists`,
    BEST_PLAYERS: (id: string) => `tournaments/${id}/players/best`,
    TEAM_STATS: (id: string) => `tournaments/${id}/teams/statistics`,
    
    TEAM_DETAILS: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}`,
    TEAM_ROSTER: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}/players`,
    TEAM_MATCHES: (tournamentId: string, teamId: string) => `tournaments/${tournamentId}/teams/${teamId}/matches`,
    PLAYER_DETAILS: (tournamentId: string, playerId: string) => `tournaments/${tournamentId}/players/${playerId}`,
    PLAYER_STATS: (tournamentId: string, playerId: string) => `tournaments/${tournamentId}/players/${playerId}/statistics`,
    
    MATCH_DETAILS: (tournamentId: string, matchId: string) => `tournaments/${tournamentId}/matches/${matchId}`,
    LIVE_MATCHES: (id: string) => `tournaments/${id}/matches/live`,
    UPCOMING_MATCHES: (id: string) => `tournaments/${id}/matches/upcoming`,
    COMPLETED_MATCHES: (id: string) => `tournaments/${id}/matches/completed`
  }
} as const;