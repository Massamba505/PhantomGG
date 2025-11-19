import { TournamentPage } from '../../support/page-objects/TournamentPage';

describe('Tournament CRUD Operations', () => {
  const tournamentPage = new TournamentPage();
  const users = require('../../fixtures/users.json');
  const tournamentData = require('../../fixtures/data/tournaments.json');
  
  let createdTournamentId: string;
  let tournamentName: string;

  beforeEach(() => {
    cy.clearLocalStorage();
    cy.clearCookies();
  });

  describe('Create Tournament', () => {
    beforeEach(() => {
      cy.login(users.organizer.email, users.organizer.password);
      cy.url().should('include', '/organizer');
    });

    afterEach(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/tournaments\/([^\/]+)/);
          if (match) {
            const tournamentId = match[1];
            cy.deleteTournament(tournamentId);
          }
        }
      });
    });

    it('should create a tournament with valid data', () => {
      tournamentName = `${tournamentData.valid.name} ${Date.now()}`;
      const data = { ...tournamentData.valid, name: tournamentName };
      
      cy.createTournament(data);
      
      // Verify redirect and success message
      cy.url().should('include', '/organizer/tournaments');
      cy.contains(tournamentName).should('be.visible');
    });

    it('should create a draft tournament', () => {
      tournamentName = `${tournamentData.draft.name} ${Date.now()}`;
      const data = { ...tournamentData.draft, name: tournamentName };
      
      cy.createTournament(data);
      
      cy.url().should('include', '/organizer/tournaments');
      cy.contains(tournamentName).should('be.visible');
    });

    it('should validate required fields', () => {
      tournamentPage.clickCreate();
      tournamentPage.submit();

      cy.get('[data-cy="tournament-name-input"]').should('have.class', 'error');
      cy.get('[data-cy="tournament-description-input"]').should('have.class', 'error');
    });

    it('should validate date constraints', () => {
      tournamentPage.clickCreate();
      const invalidData = {
        ...tournamentData.valid,
        name: `Invalid Date Tournament ${Date.now()}`,
        startDate: '2025-12-01T08:00:00',
        endDate: '2025-11-01T08:00:00'
      };
      
      tournamentPage.fillForm(invalidData);
      tournamentPage.submit();
      
      cy.contains(/date|invalid/i).should('be.visible');
    });
  });

  describe('Read Tournament', () => {
    before(() => {
      cy.login(users.organizer.email, users.organizer.password);
      tournamentName = `Read Test Tournament ${Date.now()}`;
      const data = { ...tournamentData.valid, name: tournamentName };
      cy.createTournament(data);
      
      // Store tournament ID from URL
      cy.url().should('not.include', '/create').should('not.include', '/create').then((url) => {
        const match = url.match(/tournaments\/([^\/]+)/);
        if (match) createdTournamentId = match[1];
      });
    });

    beforeEach(() => {
      cy.login(users.organizer.email, users.organizer.password);
    });

    after(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/tournaments\/([^\/]+)/);
          if (match) {
            const tournamentId = match[1];
            cy.deleteTournament(tournamentId);
          }
        }
      });
    });

    it('should view tournament details', () => {
      tournamentPage.visitList();
      cy.contains(tournamentName).should('be.visible');
    });

    it('should display tournament information correctly', () => {
      if (createdTournamentId) {
        tournamentPage.visitDetails(createdTournamentId);
        cy.get('[data-cy="tournament-details"]').should('be.visible');
        cy.contains(tournamentData.valid.description).should('be.visible');
      }
    });

    it('should view tournament teams', () => {
      if (createdTournamentId) {
        tournamentPage.visitDetails(createdTournamentId);
        cy.get('[data-cy="tournament-teams"]').should('be.visible');
      }
    });

    it('should view tournament matches', () => {
      if (createdTournamentId) {
        tournamentPage.visitDetails(createdTournamentId);
        cy.get('[data-cy="tournament-matches"]').should('be.visible');
      }
    });
  });

  describe('Update Tournament', () => {
    before(() => {
      cy.login(users.organizer.email, users.organizer.password);
      tournamentName = `Update Test Tournament ${Date.now()}`;
      const data = { ...tournamentData.valid, name: tournamentName };
      cy.createTournament(data);
      
      cy.url().should('not.include', '/create').then((url) => {
        const match = url.match(/tournaments\/([^\/]+)/);
        if (match) createdTournamentId = match[1];
      });
    });

    beforeEach(() => {
      cy.login(users.organizer.email, users.organizer.password);
    });
    
    after(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/tournaments\/([^\/]+)/);
          if (match) {
            const tournamentId = match[1];
            cy.deleteTournament(tournamentId);
          }
        }
      });
    });

    it('should update tournament details', () => {
      if (createdTournamentId) {
        cy.editTournament(createdTournamentId, tournamentData.update);
        
        cy.url().should('include', `/tournaments/${createdTournamentId}`);
        cy.contains(tournamentData.update.name).should('be.visible');
      }
    });

    it('should update tournament location', () => {
      if (createdTournamentId) {
        const updateData = { location: 'Updated Location Arena' };
        cy.editTournament(createdTournamentId, updateData);
        
        cy.contains(updateData.location).should('be.visible');
      }
    });

    it('should not update with invalid data', () => {
      if (createdTournamentId) {
        tournamentPage.visitEdit(createdTournamentId);
        cy.get('[data-cy="tournament-name-input"]').clear();
        tournamentPage.submit();
        
        cy.get('[data-cy="tournament-name-input"]').should('have.class', 'error');
      }
    });
  });

  describe('Delete Tournament', () => {
    beforeEach(() => {
      cy.login(users.organizer.email, users.organizer.password);
    });

    it('should delete a tournament', () => {
      tournamentName = `Delete Test Tournament ${Date.now()}`;
      const data = { ...tournamentData.valid, name: tournamentName };
      cy.createTournament(data);
      
      cy.url().should('not.include', '/create').then((url) => {
        const match = url.match(/tournaments\/([^\/]+)/);
        if (match) {
          const tournamentId = match[1];
          cy.deleteTournament(tournamentId);
          
          cy.url().should('include', '/organizer/tournaments');
          cy.contains(tournamentName).should('not.exist');
        }
      });
    });

    it('should cancel delete operation', () => {
      tournamentName = `Cancel Delete Tournament ${Date.now()}`;
      const data = { ...tournamentData.valid, name: tournamentName };
      cy.createTournament(data);
      
      cy.url().should('not.include', '/create').then((url) => {
        const match = url.match(/tournaments\/([^\/]+)/);
        if (match) {
          const tournamentId = match[1];
          tournamentPage.visitDetails(tournamentId);
          tournamentPage.clickDelete();
          cy.get('[data-cy="cancel-delete-button"]').click();
          
          // Verify tournament still exists
          cy.contains(tournamentName).should('be.visible');
        }
      });
    });
  });
});
