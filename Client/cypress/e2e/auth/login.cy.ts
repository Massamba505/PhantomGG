import { LoginPage } from '../../support/page-objects/LoginPage';

describe('Authentication - Login', () => {
  const loginPage = new LoginPage();
  const users = require('../../fixtures/users.json');

  beforeEach(() => {
    cy.clearLocalStorage();
    cy.clearCookies();
    loginPage.visit();
  });

  describe('Successful Login', () => {
    it('should login user with valid credentials', () => {
      loginPage.login(users.user.email, users.user.password);
      cy.url().should('include', '/user');
    });

    it('should login organizer with valid credentials', () => {
      loginPage.login(users.organizer.email, users.organizer.password);
      cy.url().should('include', '/organizer');
    });

    it('should remember user when remember me is checked', () => {
      loginPage.login(users.user.email, users.user.password, true);
      cy.url().should('include', '/user');
      cy.window().then((win) => {
        expect(win.localStorage.getItem('access_token')).to.exist;
      });
    });
  });

  describe('Failed Login', () => {
    it('should show error for invalid email', () => {
      loginPage.login('invalid@email.com', 'wrongpassword');
      cy.url().should('include', '/auth/login');
      cy.contains(/invalid|error/i).should('be.visible');
    });

    it('should show error for invalid password', () => {
      loginPage.login(users.user.email, 'wrongpassword');
      cy.url().should('include', '/auth/login');
      cy.contains(/invalid|error/i).should('be.visible');
    });

    it('should show error for empty credentials', () => {
      loginPage.clickLogin();
      cy.get('[data-cy="email-input"]').should('have.class', 'error');
      cy.get('[data-cy="password-input"]').should('have.class', 'error');
    });

    it('should show error for invalid email format', () => {
      loginPage.enterEmail('notanemail').enterPassword('password123');
      loginPage.clickLogin();
      cy.get('[data-cy="email-input"]').should('have.class', 'error');
    });
  });

  describe('Navigation', () => {
    it('should navigate to signup page', () => {
      loginPage.clickSignup();
      cy.url().should('include', '/auth/signup');
    });

    it('should navigate to forgot password page', () => {
      loginPage.clickForgotPassword();
      cy.url().should('include', '/auth/forgot-password');
    });
  });

  describe('Session Management', () => {
    it('should maintain session after page reload', () => {
      loginPage.login(users.user.email, users.user.password);
      cy.url().should('include', '/user');
      cy.reload();
      cy.url().should('include', '/user');
    });

    it('should logout successfully', () => {
      loginPage.login(users.user.email, users.user.password);
      cy.logout();
      cy.url().should('include', '/');
    });
  });
});
