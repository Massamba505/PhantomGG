/// <reference types="cypress" />

/**
 * Workflow 1: Create User and Organizer Accounts
 * 
 * This workflow creates all test accounts (users and organizers) from the test-data.json fixture.
 * It tests the complete signup flow including role selection.
 */

describe('Workflow 1: Create Accounts', () => {
  const testData = require('../../fixtures/workflow/test-data.json');
  const createdAccounts: string[] = [];

  before(() => {
    cy.log('Starting account creation workflow');
    cy.log(`Users to create: ${testData.users.length}`);
    cy.log(`Organizers to create: ${testData.organizers.length}`);
  });

  after(() => {
    cy.log('Account creation workflow completed');
    cy.log(`Total accounts created: ${createdAccounts.length}`);
  });

  describe('Create User Accounts', () => {
    testData.users.forEach((user: any, index: number) => {
      it(`should create user account: ${user.firstName} ${user.lastName} (${index + 1}/${testData.users.length})`, () => {
        cy.visit('/auth/signup');
        
        cy.get('[data-cy="signup-container"]', { timeout: 10000 }).should('be.visible');
        cy.get('[data-cy="signup-form"]').should('be.visible');

        cy.get('[data-cy="first-name-input"]')
          .should('be.visible')
          .clear()
          .type(user.firstName);
        
        cy.get('[data-cy="last-name-input"]')
          .should('be.visible')
          .clear()
          .type(user.lastName);
        
        cy.get('[data-cy="email-input"]')
          .should('be.visible')
          .clear()
          .type(user.email);
        
        cy.get('[data-cy="password-input"]')
          .should('be.visible')
          .clear()
          .type(user.password);
        
        cy.get('[data-cy="confirm-password-input"]')
          .should('be.visible')
          .clear()
          .type(user.password);

        cy.get('[data-cy="password-strength"]', { timeout: 5000 }).should('be.visible');

        cy.get('[data-cy="signup-submit-button"]').click();

        cy.url({ timeout: 10000 }).should('include', '/auth/role-selection');
        
        cy.get('[data-cy="role-card-3"]', { timeout: 5000 }).should('be.visible').click();
        
        cy.get('button').contains('Create Account').should('not.be.disabled').click();

        cy.url({ timeout: 15000 }).should('include', '/auth/verify-email-sent');
        cy.contains(/verification|email/i, { timeout: 5000 }).should('be.visible');

        createdAccounts.push(user.email);
        cy.log(`✅ Created user account: ${user.email}`);
      });
    });
  });

  describe('Create Organizer Accounts', () => {
    testData.organizers.forEach((organizer: any, index: number) => {
      it(`should create organizer account: ${organizer.firstName} ${organizer.lastName} (${index + 1}/${testData.organizers.length})`, () => {
        cy.visit('/auth/signup');
        
        cy.get('[data-cy="signup-container"]', { timeout: 10000 }).should('be.visible');
        cy.get('[data-cy="signup-form"]').should('be.visible');

        cy.get('[data-cy="first-name-input"]')
          .should('be.visible')
          .clear()
          .type(organizer.firstName);
        
        cy.get('[data-cy="last-name-input"]')
          .should('be.visible')
          .clear()
          .type(organizer.lastName);
        
        cy.get('[data-cy="email-input"]')
          .should('be.visible')
          .clear()
          .type(organizer.email);
        
        cy.get('[data-cy="password-input"]')
          .should('be.visible')
          .clear()
          .type(organizer.password);
        
        cy.get('[data-cy="confirm-password-input"]')
          .should('be.visible')
          .clear()
          .type(organizer.password);

        cy.get('[data-cy="password-strength"]', { timeout: 5000 }).should('be.visible');

        cy.get('[data-cy="signup-submit-button"]').click();

        cy.url({ timeout: 10000 }).should('include', '/auth/role-selection');
        
        cy.get('[data-cy="role-card-2"]', { timeout: 5000 }).should('be.visible').click();
        
        cy.get('button').contains('Create Account').should('not.be.disabled').click();

        cy.url({ timeout: 15000 }).should('include', '/auth/verify-email-sent');
        cy.contains(/verification|email/i, { timeout: 5000 }).should('be.visible');

        createdAccounts.push(organizer.email);
        cy.log(`✅ Created organizer account: ${organizer.email}`);
      });
    });
  });

  describe('Verify Account Creation Summary', () => {
    it('should have created all accounts', () => {
      const expectedTotal = testData.users.length + testData.organizers.length;
      expect(createdAccounts).to.have.length(expectedTotal);
      
      cy.log('=== Account Creation Summary ===');
      cy.log(`Total Users Created: ${testData.users.length}`);
      cy.log(`Total Organizers Created: ${testData.organizers.length}`);
      cy.log(`Total Accounts: ${createdAccounts.length}`);
      cy.log('================================');
    });
  });
});
