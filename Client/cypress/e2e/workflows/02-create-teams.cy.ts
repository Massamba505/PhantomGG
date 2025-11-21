/// <reference types="cypress" />

/**
 * Workflow 2: Login Users and Create Teams
 * 
 * This workflow logs in each user and creates their teams from the test-data.json fixture.
 * Each user will create all their assigned teams with logos.
 */

describe('Workflow 2: Login Users and Create Teams', () => {
  const testData = require('../../fixtures/workflow/test-data.json');
  const createdTeams: any[] = [];
  
  // Group teams by userId for easier processing
  const teamsByUser = testData.teams.reduce((acc: any, team: any) => {
    if (!acc[team.userId]) {
      acc[team.userId] = [];
    }
    acc[team.userId].push(team);
    return acc;
  }, {});

  before(() => {
    cy.log('Starting team creation workflow');
    cy.log(`Total teams to create: ${testData.teams.length}`);
    cy.log(`Users with teams: ${Object.keys(teamsByUser).length}`);
  });

  after(() => {
    cy.log('Team creation workflow completed');
    cy.log(`Total teams created: ${createdTeams.length}`);
  });

  testData.users.forEach((user: any) => {
    const userTeams = teamsByUser[user.id] || [];
    
    if (userTeams.length === 0) {
      return; // Skip users with no teams
    }

    describe(`${user.firstName} ${user.lastName} - Create Teams (${userTeams.length} teams)`, () => {
      
      beforeEach(() => {
        cy.clearLocalStorage();
        cy.clearCookies();
        
        // Login
        cy.log(`Logging in as: ${user.email}`);
        cy.visit('/auth/login');
        cy.get('[data-cy="email-input"]', { timeout: 10000 }).should('be.visible').type(user.email);
        cy.get('[data-cy="password-input"]').should('be.visible').type(user.password);
        cy.get('[data-cy="login-button"]').click();
        
        // Wait for redirect to user dashboard
        cy.url({ timeout: 15000 }).should('include', '/user');
        cy.log(`✅ Logged in successfully: ${user.email}`);
      });

      userTeams.forEach((team: any, index: number) => {
        it(`should create team: ${team.name} (${index + 1}/${userTeams.length})`, () => {
          // Navigate to create team page
          cy.visit('/user/teams/create');
          cy.get('[data-cy="team-name-input"]', { timeout: 10000 }).should('be.visible');

          // Upload team logo if available
          if (team.logoUrl) {
            cy.get('[data-cy="team-image-input"]').attachFile({
              filePath: team.logoUrl,
              encoding: 'base64',
              mimeType: 'image/jpeg'
            });
            
            // Wait a bit for file to upload and preview to show
            cy.wait(500);
          }

          // Fill in team name
          cy.get('[data-cy="team-name-input"]')
            .should('be.visible')
            .clear()
            .type(team.name);

          // Fill in short name
          cy.get('[data-cy="team-short-name-input"]')
            .should('be.visible')
            .clear()
            .type(team.shortName);

          // Submit the form
          cy.get('[data-cy="team-submit-button"]').click();

          // Wait for redirect - should go to team details or teams list
          cy.url({ timeout: 15000 }).should('include', '/user/teams');
          cy.url().should('not.include', '/create');

          // Verify team appears in the list or details
          cy.contains(team.name, { timeout: 5000 }).should('be.visible');

          createdTeams.push({
            userId: user.id,
            teamId: team.id,
            teamName: team.name,
            userEmail: user.email
          });

          cy.log(`✅ Created team: ${team.name} for ${user.email}`);
        });
      });

      afterEach(() => {
        // Logout after creating all teams for this user
        cy.get('[data-cy="profile-dropdown-button"]', { timeout: 5000 }).should('be.visible').click();
        cy.get('[data-cy="logout-button"]', { timeout: 5000 }).should('be.visible').click();
        cy.url({ timeout: 10000 }).should('not.include', '/user');
        cy.log(`✅ Logged out: ${user.email}`);
      });
    });
  });

  describe('Verify Team Creation Summary', () => {
    it('should have created all teams', () => {
      expect(createdTeams).to.have.length(testData.teams.length);
      
      cy.log('=== Team Creation Summary ===');
      testData.users.forEach((user: any) => {
        const userTeamsCreated = createdTeams.filter(t => t.userId === user.id);
        if (userTeamsCreated.length > 0) {
          cy.log(`${user.firstName} ${user.lastName}: ${userTeamsCreated.length} teams`);
        }
      });
      cy.log(`Total Teams Created: ${createdTeams.length}`);
      cy.log('=============================');
    });
  });
});
