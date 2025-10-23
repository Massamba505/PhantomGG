export class LoginPage {
  private emailInput = '[data-cy="email-input"]';
  private passwordInput = '[data-cy="password-input"]';
  private loginButton = '[data-cy="login-button"]';
  private rememberMeCheckbox = '[data-cy="remember-me-checkbox"]';
  private forgotPasswordLink = '[data-cy="forgot-password-link"]';
  private signupLink = '[data-cy="signup-link"]';

  visit() {
    cy.visit('/auth/login');
  }

  enterEmail(email: string) {
    cy.get(this.emailInput).type(email);
    return this;
  }

  enterPassword(password: string) {
    cy.get(this.passwordInput).type(password);
    return this;
  }

  clickRememberMe() {
    cy.get(this.rememberMeCheckbox).click();
    return this;
  }

  clickLogin() {
    cy.get(this.loginButton).click();
  }

  clickForgotPassword() {
    cy.get(this.forgotPasswordLink).click();
  }

  clickSignup() {
    cy.get(this.signupLink).click();
  }

  login(email: string, password: string, rememberMe: boolean = false) {
    this.enterEmail(email);
    this.enterPassword(password);
    if (rememberMe) {
      this.clickRememberMe();
    }
    this.clickLogin();
  }
}
