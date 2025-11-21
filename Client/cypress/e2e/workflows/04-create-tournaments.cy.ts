/// <reference types="cypress" />

/**
 * Workflow 4: Login Organizers and Create Tournaments
 * 
 * This workflow logs in each organizer and creates their tournaments from the test-data.json fixture.
 * Each organizer will create all their assigned tournaments with logos and banners.
 */

describe('Workflow 4: Login Organizers and Create Tournaments', () => {
  const testData = require('../../fixtures/workflow/test-data.json');
  const createdTournaments: any[] = [];
  
  // Group tournaments by organizerId for easier processing
  const tournamentsByOrganizer = testData.tournaments.reduce((acc: any, tournament: any) => {
    if (!acc[tournament.organizerId]) {
      acc[tournament.organizerId] = [];
    }
    acc[tournament.organizerId].push(tournament);
    return acc;
  }, {});

  before(() => {
    cy.log('Starting tournament creation workflow');
    cy.log(`Total tournaments to create: ${testData.tournaments.length}`);
    cy.log(`Organizers with tournaments: ${Object.keys(tournamentsByOrganizer).length}`);
  });

  after(() => {
    cy.log('Tournament creation workflow completed');
    cy.log(`Total tournaments created: ${createdTournaments.length}`);
  });

  testData.organizers.forEach((organizer: any) => {
    const organizerTournaments = tournamentsByOrganizer[organizer.id] || [];
    
    if (organizerTournaments.length === 0) {
      return; // Skip organizers with no tournaments
    }

    describe(`${organizer.firstName} ${organizer.lastName} - Create Tournaments (${organizerTournaments.length} tournaments)`, () => {
      
      // Login ONCE per organizer (not per tournament)
      beforeEach(() => {
        cy.clearLocalStorage();
        cy.clearCookies();
        
        // Login
        cy.log(`Logging in as: ${organizer.email}`);
        cy.visit('/auth/login');
        cy.get('[data-cy="email-input"]', { timeout: 10000 }).should('be.visible').type(organizer.email);
        cy.get('[data-cy="password-input"]').should('be.visible').type(organizer.password);
        cy.get('[data-cy="login-button"]').click();
        
        // Wait for redirect to organizer dashboard
        cy.url({ timeout: 15000 }).should('include', '/organizer');
        cy.log(`✅ Logged in successfully: ${organizer.email}`);
      });

      // Process all tournaments for this organizer
      organizerTournaments.forEach((tournament: any, index: number) => {
        it(`should create tournament: ${tournament.name} (${index + 1}/${organizerTournaments.length})`, () => {
          // Navigate to create tournament page
          cy.visit('/organizer/tournaments/create');
          cy.get('[data-cy="tournament-form"]', { timeout: 10000 }).should('be.visible');
          cy.get('[data-cy="tournament-name-input"]').should('be.visible');

          // Upload tournament banner if available
          if (tournament.bannerUrl) {
            cy.get('[data-cy="tournament-banner-input"]').attachFile({
              filePath: tournament.bannerUrl,
              encoding: 'base64',
              mimeType: 'image/jpeg'
            });
            
            // Wait for file to upload
            cy.wait(500);
            cy.get('[data-cy="banner-preview-container"]', { timeout: 5000 }).should('be.visible');
          }

          // Upload tournament logo if available
          if (tournament.logoUrl) {
            cy.get('[data-cy="tournament-logo-input"]').attachFile({
              filePath: tournament.logoUrl,
              encoding: 'base64',
              mimeType: 'image/jpeg'
            });
            
            // Wait for file to upload
            cy.wait(500);
            cy.get('[data-cy="logo-preview-container"]', { timeout: 5000 }).should('be.visible');
          }

          // Fill in tournament name
          cy.get('[data-cy="tournament-name-input"]')
            .should('be.visible')
            .clear()
            .type(tournament.name);

          // Fill in description
          cy.get('[data-cy="tournament-description-input"]')
            .should('be.visible')
            .clear()
            .type(tournament.description);

          // Fill in location
          cy.get('[data-cy="tournament-location-input"]')
            .should('be.visible')
            .clear()
            .type(tournament.location);

          // Fill in registration start date
          const registrationStartFormatted = new Date(tournament.registrationStartDate)
            .toISOString()
            .slice(0, 16);
          cy.get('[data-cy="registration-start-input"]')
            .should('be.visible')
            .clear()
            .type(registrationStartFormatted);

          // Fill in registration deadline
          const registrationDeadlineFormatted = new Date(tournament.registrationDeadline)
            .toISOString()
            .slice(0, 16);
          cy.get('[data-cy="registration-deadline-input"]')
            .should('be.visible')
            .clear()
            .type(registrationDeadlineFormatted);

          // Fill in start date
          const startDateFormatted = new Date(tournament.startDate)
            .toISOString()
            .slice(0, 16);
          cy.get('[data-cy="start-date-input"]')
            .should('be.visible')
            .clear()
            .type(startDateFormatted);

          // Fill in end date
          const endDateFormatted = new Date(tournament.endDate)
            .toISOString()
            .slice(0, 16);
          cy.get('[data-cy="end-date-input"]')
            .should('be.visible')
            .clear()
            .type(endDateFormatted);

          // Fill in min teams
          cy.get('[data-cy="min-teams-input"]')
            .should('be.visible')
            .clear()
            .type(tournament.minTeams.toString());

          // Fill in max teams
          cy.get('[data-cy="max-teams-input"]')
            .should('be.visible')
            .clear()
            .type(tournament.maxTeams.toString());

          // Set public/private status
          if (tournament.isPublic) {
            cy.get('[data-cy="public-tournament-checkbox"]').check();
          } else {
            cy.get('[data-cy="public-tournament-checkbox"]').uncheck();
          }

          // Submit the form
          cy.get('[data-cy="submit-button"]').click();

          // Wait for redirect - should go to tournament details
          cy.url({ timeout: 15000 }).should('include', '/organizer/tournaments');
          cy.url().should('not.include', '/create');

          // Verify tournament appears in the details or list
          cy.contains(tournament.name, { timeout: 5000 }).should('be.visible');

          createdTournaments.push({
            organizerId: organizer.id,
            tournamentId: tournament.id,
            tournamentName: tournament.name,
            organizerEmail: organizer.email
          });

          cy.log(`✅ Created tournament: ${tournament.name} for ${organizer.email}`);
        });
      });

      // Logout ONCE after processing all tournaments for this organizer
      afterEach(() => {
        cy.get('[data-cy="profile-dropdown-button"]', { timeout: 5000 }).should('be.visible').click();
        cy.get('[data-cy="logout-button"]', { timeout: 5000 }).should('be.visible').click();
        cy.url({ timeout: 10000 }).should('not.include', '/organizer');
        cy.log(`✅ Logged out: ${organizer.email}`);
      });

    });
  });

  // GLOBAL SUMMARY
  describe('Verify Tournament Creation Summary', () => {
    it('should have created all tournaments', () => {
      expect(createdTournaments).to.have.length(testData.tournaments.length);
      
      cy.log('=== Tournament Creation Summary ===');
      testData.organizers.forEach((organizer: any) => {
        const organizerTournamentsCreated = createdTournaments.filter(t => t.organizerId === organizer.id);
        if (organizerTournamentsCreated.length > 0) {
          cy.log(`${organizer.firstName} ${organizer.lastName}: ${organizerTournamentsCreated.length} tournaments`);
        }
      });
      cy.log(`Total Tournaments Created: ${createdTournaments.length}`);
      cy.log('===================================');
    });
  });
});
