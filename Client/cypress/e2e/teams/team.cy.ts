/// <reference types="cypress" />
import { TeamPage } from '../../support/page-objects/TeamPage';

describe('Team Operations', () => {
  const teamPage = new TeamPage();
  const users = require('../../fixtures/users.json');
  const teamData = require('../../fixtures/data/teams.json');
  const playerData = require('../../fixtures/data/players.json');
  
  let createdTeamId: string;

  beforeEach(() => {
    cy.clearLocalStorage();
    cy.clearCookies();
  });

  describe('Create Team', () => {
    beforeEach(() => {
      cy.login(users.user.email, users.user.password);
      cy.url().should('include', '/user');
    });

    afterEach(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/teams\/([^\/]+)/);
          if (match) {
            const teamId = match[1];
            cy.deleteTeam(teamId);
          }
        }
      });
    });

    it('should create a team with valid data', () => {
      const teamName = `${teamData.valid.name} ${Date.now()}`;
      const data = { ...teamData.valid, name: teamName };
      
      cy.createTeam(data);
      
      cy.url().should('include', '/user/teams');
      cy.contains(teamName).should('be.visible');
    });

    it('should validate required fields', () => {
      teamPage.visitCreate();
      teamPage.submitCreate();
      
      cy.get('[data-cy="team-name-input"]').should('have.class', 'error');
    });

    it('should validate short name length', () => {
      teamPage.visitCreate();
      cy.get('[data-cy="team-name-input"]').type('Valid Team Name');
      cy.get('[data-cy="team-short-name-input"]').type('TOOLONG');
      teamPage.submitCreate();
      
      cy.contains(/short name|length/i).should('be.visible');
    });

    it('should create team without short name (optional)', () => {
      const teamName = `No Short Name Team ${Date.now()}`;
      
      teamPage.visitCreate();
      cy.get('[data-cy="team-name-input"]').type(teamName);
      teamPage.submitCreate();
      
      cy.url().should('include', '/user/teams');
    });
  });

  describe('Read Team', () => {
    before(() => {
      cy.login(users.user.email, users.user.password);
      const teamName = `Read Test Team ${Date.now()}`;
      const data = { ...teamData.valid, name: teamName };
      cy.createTeam(data);
      
      cy.url().should("not.include","/create").then((url) => {
        const match = url.match(/teams\/([^\/]+)/);
        if (match) createdTeamId = match[1];
      });
    });

    after(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/teams\/([^\/]+)/);
          if (match) {
            const teamId = match[1];
            cy.deleteTeam(teamId);
          }
        }
      });
    });

    beforeEach(() => {
      cy.login(users.user.email, users.user.password);
    });

    it('should view team list', () => {
      teamPage.visitList();
      cy.get('[data-cy="user-teams-container"]').should('be.visible');
    });

    it('should view team details', () => {
      if (createdTeamId) {
        teamPage.visitDetails(createdTeamId);
        cy.get('[data-cy="team-details-section"]').should('be.visible');
      }
    });

    it('should display team information correctly', () => {
      if (createdTeamId) {
        teamPage.visitDetails(createdTeamId);
        cy.get('[data-cy="team-players-section"]').should('be.visible');
        cy.contains(teamData.valid.shortName).should('be.visible');
      }
    });

    it('should display team players', () => {
      if (createdTeamId) {
        teamPage.visitDetails(createdTeamId);
        cy.get('[data-cy="team-players-section"]').should('be.visible');
      }
    });
  });

  describe('Update Team', () => {
    before(() => {
      cy.login(users.user.email, users.user.password);
      const teamName = `Update Test Team ${Date.now()}`;
      const data = { ...teamData.valid, name: teamName };
      cy.createTeam(data);
      
      cy.url().should("not.include","/create").then((url) => {
        const match = url.match(/teams\/([^\/]+)/);
        if (match) createdTeamId = match[1];
      });
    });

    beforeEach(() => {
      cy.login(users.user.email, users.user.password);
    });

    after(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/teams\/([^\/]+)/);
          if (match) {
            const teamId = match[1];
            cy.deleteTeam(teamId);
          }
        }
      });
    });

    it('should update team details', () => {
      if (createdTeamId) {
        const teamName = `${teamData.update.name} ${Date.now()}`;
        const data = { ...teamData.update, name: teamName };
        cy.editTeam(createdTeamId, data);
        
        cy.url().should('include', `/teams/${createdTeamId}`);
        cy.contains(teamName).should('be.visible');
      }
    });

    it('should update team short name', () => {
      if (createdTeamId) {
        const updateData = { shortName: 'NEW' };
        cy.editTeam(createdTeamId, updateData);
        
        cy.contains(updateData.shortName).should('be.visible');
      }
    });

    it('should not update with invalid data', () => {
      if (createdTeamId) {
        teamPage.visitEdit(createdTeamId);
        cy.get('[data-cy="team-name-input"]').clear();
        teamPage.submitUpdate();
        
        cy.get('[data-cy="team-name-input"]').should('have.class', 'error');
      }
    });
  });

  describe('Delete Team', () => {
    beforeEach(() => {
      cy.login(users.user.email, users.user.password);
    });

    afterEach(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/teams\/([^\/]+)/);
          if (match) {
            const teamId = match[1];
            cy.deleteTeam(teamId);
          }
        }
      });
    });

    it('should delete a team', () => {
      const teamName = `Delete Test Team ${Date.now()}`;
      const data = { ...teamData.valid, name: teamName };
      cy.createTeam(data);
      
      cy.url().should('not.include', '/create').then((url) => {
        const match = url.match(/teams\/([^\/]+)/);
        if (match) {
          const teamId = match[1];
          cy.deleteTeam(teamId);
          
          cy.url().should('include', '/user/teams');
          cy.contains(teamName).should('not.exist');
        }
      });
    });

    it('should cancel delete operation', () => {
      const teamName = `Cancel Delete Team ${Date.now()}`;
      const data = { ...teamData.valid, name: teamName };
      cy.createTeam(data);
      
      cy.url().should('not.include', '/create').then((url) => {
        const match = url.match(/teams\/([^\/]+)/);
        if (match) {
          const teamId = match[1];
          teamPage.visitDetails(teamId);
          teamPage.clickDelete();
          cy.get('[data-cy="cancel-delete-button"]').click();
          
          cy.contains(teamName).should('be.visible');
        }
      });
    });
  });

  describe('Team Player Management', () => {
    before(() => {
      cy.login(users.user.email, users.user.password);
      const teamName = `Player Management Team ${Date.now()}`;
      const data = { ...teamData.withPlayers, name: teamName };
      cy.createTeam(data);
      
      cy.url().should('not.include', '/create').should("not.include","/create").then((url) => {
        const match = url.match(/teams\/([^\/]+)/);
        if (match) createdTeamId = match[1];
      });
    });

    after(() => {
      cy.wait(1000);
      cy.url().then((url) => {
        if (!url.includes('/create')) {
          const match = url.match(/teams\/([^\/]+)/);
          if (match) {
            const teamId = match[1];
            cy.deleteTeam(teamId);
          }
        }
      });
    });

    beforeEach(() => {
      cy.login(users.user.email, users.user.password);
    });

    it('should add a player to team', () => {
      if (createdTeamId) {
        cy.addPlayer(createdTeamId, playerData.valid[0]);
        
        cy.contains(playerData.valid[0].firstName).should('be.visible');
        cy.contains(playerData.valid[0].lastName).should('be.visible');
      }
    });

    it('should add multiple players', () => {
      if (createdTeamId) {
        teamPage.visitDetails(createdTeamId);
        
        playerData.valid.slice(1, 3).forEach((player: any) => {
          cy.addPlayer(createdTeamId, player);
          cy.wait(500);
        });
        
        teamPage.visitDetails(createdTeamId);
        cy.get('[data-cy="players-list"]').should('contain', playerData.valid[1].firstName);
      }
    });
  });
});
