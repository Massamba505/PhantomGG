/// <reference types="cypress" />
import 'cypress-file-upload';

declare global {
  namespace Cypress {
    interface Chainable {
      login(email: string, password: string): Chainable<void>;
      logout(): Chainable<void>;
      
      createTournament(tournamentData: any): Chainable<void>;
      editTournament(tournamentId: string, updateData: any): Chainable<void>;
      deleteTournament(tournamentId: string): Chainable<void>;
      viewTournament(tournamentId: string): Chainable<void>;
      
      createTeam(teamData: any): Chainable<void>;
      editTeam(teamId: string, updateData: any): Chainable<void>;
      deleteTeam(teamId: string): Chainable<void>;
      viewTeam(teamId: string): Chainable<void>;
      
      addPlayer(teamId: string, playerData: any): Chainable<void>;
      editPlayer(teamId: string, playerId: string, updateData: any): Chainable<void>;
      removePlayer(teamId: string, playerId: string): Chainable<void>;
      
      createMatch(matchData: any): Chainable<void>;
      editMatch(matchId: string, updateData: any): Chainable<void>;
      deleteMatch(matchId: string): Chainable<void>;
      addMatchEvent(matchId: string, eventData: any): Chainable<void>;
      
      registerTeamForTournament(tournamentId: string, teamId: string): Chainable<void>;
      approveTeam(tournamentId: string, teamId: string): Chainable<void>;
      rejectTeam(tournamentId: string, teamId: string): Chainable<void>;
      generateFixtures(tournamentId: string, format: string): Chainable<void>;
      
      generateEmail(): Chainable<string>;
      generateTimestamp(): Chainable<number>;
      waitForApiResponse(alias: string): Chainable<void>;
    }
  }
}

Cypress.Commands.add('login', (email: string, password: string) => {
  cy.visit('/auth/login');
  cy.get('[data-cy="email-input"]').should('be.visible').type(email);
  cy.get('[data-cy="password-input"]').should('be.visible').type(password);
  cy.get('[data-cy="login-button"]').click();
  cy.url().should('not.include', '/auth/login');
});

Cypress.Commands.add('logout', () => {
  cy.get('[data-cy="profile-dropdown-button"]').should('be.visible').click();
  cy.get('[data-cy="profile-actions"]').should('be.visible');
  cy.get('[data-cy="logout-button"]').click();
  cy.url().should('include', '/');
});

Cypress.Commands.add('createTournament', (tournamentData: any) => {
  cy.get('[data-cy="quick-action-create-tournament"]').click();
  cy.get('[data-cy="tournament-name-input"]').type(tournamentData.name);
  cy.get('[data-cy="tournament-description-input"]').type(tournamentData.description);
  
  if (tournamentData.location) {
    cy.get('[data-cy="tournament-location-input"]').type(tournamentData.location);
  }
  
  cy.get('[data-cy="registration-start-input"]').type(
    tournamentData.registrationStartDate.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '')
  );
  cy.get('[data-cy="registration-deadline-input"]').type(
    tournamentData.registrationDeadline.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '')
  );
  cy.get('[data-cy="start-date-input"]').type(
    tournamentData.startDate.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '')
  );
  cy.get('[data-cy="end-date-input"]').type(
    tournamentData.endDate.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '')
  );
  
  cy.get('[data-cy="max-teams-input"]').clear().type(tournamentData.maxTeams.toString());
  cy.get('[data-cy="min-teams-input"]').clear().type(tournamentData.minTeams.toString());
  
  cy.get('[data-cy="submit-button"]').click();
});

Cypress.Commands.add('editTournament', (tournamentId: string, updateData: any) => {
  cy.visit(`/organizer/tournaments/${tournamentId}/edit`);
  
  if (updateData.name) {
    cy.get('[data-cy="tournament-name-input"]').clear().type(updateData.name);
  }
  
  if (updateData.description) {
    cy.get('[data-cy="tournament-description-input"]').clear().type(updateData.description);
  }
  
  if (updateData.location) {
    cy.get('[data-cy="tournament-location-input"]').clear().type(updateData.location);
  }
  
  if (updateData.maxTeams) {
    cy.get('[data-cy="max-teams-input"]').clear().type(updateData.maxTeams.toString());
  }
  
  cy.get('[data-cy="submit-button"]').click();
});

Cypress.Commands.add('deleteTournament', (tournamentId: string) => {
  cy.visit(`/organizer/tournaments/${tournamentId}`);
  cy.get('[data-cy="delete-tournament-button"]').click();
  cy.get('[data-cy="confirm-delete-button"]').click();
});

Cypress.Commands.add('viewTournament', (tournamentId: string) => {
  cy.visit(`/public/tournaments/${tournamentId}`);
  cy.get('[data-cy="tournament-details"]').should('be.visible');
});


Cypress.Commands.add('createTeam', (teamData: any) => {
  cy.visit('/user/teams/create');
  cy.get('[data-cy="team-image-input"]').attachFile(teamData.image);
  cy.get('[data-cy="team-name-input"]').type(teamData.name);
  cy.get('[data-cy="team-short-name-input"]').type(teamData.shortName);
  cy.get('[data-cy="team-submit-button"]').click();
});

Cypress.Commands.add('editTeam', (teamId: string, updateData: any) => {
  console.log(teamId,updateData,"welocvoneoi");
  cy.visit(`/user/teams/${teamId}/edit`);
  
  if (updateData.name) {
    cy.get('[data-cy="team-name-input"]').clear().type(updateData.name);
  }
  
  if (updateData.shortName) {
    cy.get('[data-cy="team-short-name-input"]').clear().type(updateData.shortName);
  }
  
  cy.get('[data-cy="team-submit-button"]').click();
});

Cypress.Commands.add('deleteTeam', (teamId: string) => {
  cy.visit(`/user/teams/${teamId}`);
  cy.get('[data-cy="delete-team-button"]').click();
  cy.get('[data-cy="confirm-delete-button"]').should("be.visible").click();
});

Cypress.Commands.add('viewTeam', (teamId: string) => {
  cy.visit(`/user/teams/${teamId}`);
  cy.get('[data-cy="team-details"]').should('be.visible');
});

Cypress.Commands.add('addPlayer', (teamId: string, playerData: any) => {
  cy.visit(`/user/teams/${teamId}`);
  cy.get('[data-cy="add-player-button"]').click();
  cy.get('[data-cy="player-first-name-input"]').type(playerData.firstName);
  cy.get('[data-cy="player-last-name-input"]').type(playerData.lastName);
  
  if (playerData.position) {
    cy.get('[data-cy="player-position-select"]')
    .select(`${playerData.position}`);
  }
  
  if (playerData.email) {
    cy.get('[data-cy="player-email-input"]').type(playerData.email);
  }
  
  if (playerData.image) {
    cy.get('[data-cy="player-image-input"]').attachFile(playerData.image);
  }

  cy.get('[data-cy="submit-button"]').click();
});

Cypress.Commands.add('editPlayer', (teamId: string, playerId: string, updateData: any) => {
  cy.visit(`/user/teams/${teamId}`);
  cy.get(`[data-cy="edit-player-${playerId}"]`).click();
  
  if (updateData.firstName) {
    cy.get('[data-cy="player-first-name-input"]').clear().type(updateData.firstName);
  }
  
  if (updateData.lastName) {
    cy.get('[data-cy="player-last-name-input"]').clear().type(updateData.lastName);
  }
  
  cy.get('[data-cy="update-player-submit-button"]').click();
});

Cypress.Commands.add('removePlayer', (teamId: string, playerId: string) => {
  cy.visit(`/user/teams/${teamId}`);
  cy.get(`[data-cy="remove-player-${playerId}"]`).click();
  cy.get('[data-cy="confirm-remove-button"]').click();
});

Cypress.Commands.add('createMatch', (matchData: any) => {
  cy.visit('/organizer/matches/create');
  cy.get('[data-cy="match-date-input"]').type(matchData.matchDate.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', ''));
  
  if (matchData.venue) {
    cy.get('[data-cy="match-venue-input"]').type(matchData.venue);
  }
  
  cy.get('[data-cy="create-match-submit-button"]').click();
});

Cypress.Commands.add('editMatch', (matchId: string, updateData: any) => {
  cy.visit(`/organizer/matches/${matchId}/edit`);
  
  if (updateData.matchDate) {
    cy.get('[data-cy="match-date-input"]').clear().type(
      updateData.matchDate.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '')
    );
  }
  
  if (updateData.venue) {
    cy.get('[data-cy="match-venue-input"]').clear().type(updateData.venue);
  }
  
  cy.get('[data-cy="update-match-submit-button"]').click();
});

Cypress.Commands.add('deleteMatch', (matchId: string) => {
  cy.visit(`/organizer/matches/${matchId}`);
  cy.get('[data-cy="delete-match-button"]').click();
  cy.get('[data-cy="confirm-delete-button"]').click();
});

Cypress.Commands.add('addMatchEvent', (matchId: string, eventData: any) => {
  cy.visit(`/organizer/matches/${matchId}`);
  cy.get('[data-cy="add-event-button"]').click();
  cy.get('[data-cy="event-minute-input"]').type(eventData.minute.toString());
  cy.get('[data-cy="event-type-select"]').click();
  cy.get(`[data-cy="event-type-${eventData.eventType}"]`).click();
  cy.get('[data-cy="add-event-submit-button"]').click();
});

Cypress.Commands.add('registerTeamForTournament', (tournamentId: string, teamId: string) => {
  cy.visit(`/user/tournaments/${tournamentId}`);
  cy.get('[data-cy="register-team-button"]').click();
  cy.get(`[data-cy="select-team-${teamId}"]`).click();
  cy.get('[data-cy="confirm-registration-button"]').click();
});

Cypress.Commands.add('approveTeam', (tournamentId: string, teamId: string) => {
  cy.visit(`/organizer/tournaments/${tournamentId}`);
  cy.get('[data-cy="manage-teams-tab"]').click();
  cy.get(`[data-cy="approve-team-${teamId}"]`).click();
  cy.get('[data-cy="confirm-approve-button"]').click();
});

Cypress.Commands.add('rejectTeam', (tournamentId: string, teamId: string) => {
  cy.visit(`/organizer/tournaments/${tournamentId}`);
  cy.get('[data-cy="manage-teams-tab"]').click();
  cy.get(`[data-cy="reject-team-${teamId}"]`).click();
  cy.get('[data-cy="confirm-reject-button"]').click();
});

Cypress.Commands.add('generateFixtures', (tournamentId: string, format: string) => {
  cy.visit(`/organizer/tournaments/${tournamentId}`);
  cy.get('[data-cy="generate-fixtures-button"]').click();
  cy.get('[data-cy="fixture-format-select"]').click();
  cy.get(`[data-cy="format-${format}"]`).click();
  cy.get('[data-cy="confirm-generate-button"]').click();
});

Cypress.Commands.add('generateEmail', () => {
  const timestamp = Date.now();
  return cy.wrap(`test${timestamp}@phantomgg.com`);
});

Cypress.Commands.add('generateTimestamp', () => {
  return cy.wrap(Date.now());
});

Cypress.Commands.add('waitForApiResponse', (alias: string) => {
  cy.wait(alias).its('response.statusCode').should('be.oneOf', [200, 201]);
});

export {};