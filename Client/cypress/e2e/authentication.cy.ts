describe("Authentication", () => {
  const users = require("../fixtures/users.json");

  beforeEach(() => {
    cy.clearLocalStorage();
    cy.clearCookies();
  });

  it("can login with valid credentials", () => {
    cy.login(users.user.email, users.user.password);
    cy.url().should("not.include", "/auth/login");
    cy.url().should("include", "/user");
  });

  it("can login as organizer", () => {
    cy.login(users.organizer.email, users.organizer.password);
    cy.url().should("not.include", "/auth/login");
    cy.url().should("include", "/organizer");
  });

  it("shows error for invalid credentials", () => {
    cy.visit("/auth/login");
    cy.get("[data-cy=email-input]").type("invalid@email.com");
    cy.get("[data-cy=password-input]").type("wrongpassword");
    cy.get("[data-cy=login-button]").click();
    
    cy.url().should("include", "/auth/login");
    cy.contains("Invalid").should("be.visible");
  });

  it("can logout", () => {
    cy.login(users.user.email, users.user.password);
    
    cy.logout();
    cy.get("[data-cy=sign-in-button]").should("be.visible");
  });

  it("redirects to login when accessing protected routes", () => {
    cy.visit("/user/dashboard");
    cy.url().should("include", "/auth/login");
    
    cy.visit("/organizer/dashboard");
    cy.url().should("include", "/auth/login");
  });
});
