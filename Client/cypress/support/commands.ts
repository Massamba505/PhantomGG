
declare global {
  namespace Cypress {
    interface Chainable {
      login(email: string, password: string): Chainable<void>;
      logout(): Chainable<void>;
      createTournament(name: string): Chainable<void>;
      createTeam(name: string): Chainable<void>;
      generateEmail(): Chainable<string>;
    }
  }
}

Cypress.Commands.add('login', (email: string, password: string) => {
  cy.visit('/auth/login');
  cy.get('[data-cy="email-input"]').type(email);
  cy.get('[data-cy="password-input"]').type(password);
  cy.get('[data-cy="login-button"]').click();
  cy.url().should('not.include', '/auth/login');
});

Cypress.Commands.add('logout', () => {
  cy.get('[data-cy="profile-dropdown-button"]').click();
  cy.get('[data-cy="profile-actions"]').should("be.visible");
  cy.get('[data-cy="logout-button"]').click();
});

Cypress.Commands.add('createTournament', (name: string) => {
  cy.get('[data-cy="quick-action-create-tournament"]').click()
  cy.get('[data-cy="tournament-name-input"]').type(name);
  cy.get('[data-cy="tournament-description-input"]').type('Test tournament description');
  cy.get('[data-cy="tournament-location-input"]').type('Test location');
  cy.get('[data-cy="registration-start-input"]').type('2025-12-01T08:30');
  cy.get('[data-cy="registration-deadline-input"]').type('2025-12-03T08:30');
  cy.get('[data-cy="start-date-input"]').type('2025-12-08T08:30');
  cy.get('[data-cy="end-date-input"]').type('2025-12-09T08:30');
  cy.get('[data-cy="max-teams-input"]').clear().type('16');
  cy.get('[data-cy="min-teams-input"]').clear().type('4');
  cy.get('[data-cy="submit-button"]').click();
});

Cypress.Commands.add('createTeam', (name: string) => {
  cy.visit('/user/teams/create');
  cy.get('[data-cy="team-name-input"]').type(name);
  cy.get('[data-cy="team-short-name-input"]').type(name.substring(0, 3).toUpperCase());
  cy.get('[data-cy="create-team-submit-button"]').click();
});

Cypress.Commands.add('generateEmail', () => {
  const timestamp = Date.now();
  return cy.wrap(`test${timestamp}@phantomgg.com`);
});

export {};