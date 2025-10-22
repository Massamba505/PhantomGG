describe("PhantomGG Critical User Journeys", () => {
  const users = require("../fixtures/users.json");

  it("organizer can login and create tournament", () => {
    cy.login(users.organizer.email, users.organizer.password);
    
    cy.url().should("include", "/organizer");
    
    cy.generateEmail().then((email) => {
      const tournamentName = `Test Tournament ${Date.now()}`;
      cy.createTournament(tournamentName);
      
      cy.contains(tournamentName).should("be.visible");
      cy.url().should("include", "/organizer/tournaments");
    });
  });

  it("user can login and create team", () => {
    cy.login(users.user.email, users.user.password);
    
    cy.url().should("include", "/user");
    
    const teamName = `Test Team ${Date.now()}`;
    cy.createTeam(teamName);
    
    cy.contains("Team created").should("be.visible");
    cy.url().should("include", "/user/teams");
  });

  it("user can signup and login", () => {
    cy.generateEmail().then((email) => {
      const userData = {
        firstName: "Test",
        lastName: "User",
        email: email,
        password: "Password123!"
      };

      cy.visit("/auth/signup");
      cy.get('[data-cy="first-name-input"]').type(userData.firstName);
      cy.get('[data-cy="last-name-input"]').type(userData.lastName);
      cy.get('[data-cy="email-input"]').type(userData.email);
      cy.get('[data-cy="password-input"]').type(userData.password);
      cy.get('[data-cy="confirm-password-input"]').type(userData.password);
      cy.get('[data-cy="signup-submit-button"]').click();

      cy.url().should("not.include", "/auth/signup");

      cy.login(userData.email, userData.password);
      cy.url().should("not.include", "/auth/login");
    });
  });

  it("can navigate to public pages without authentication", () => {
    cy.visit("/");
    cy.contains("PhantomGG").should("be.visible");
    
    cy.visit("/public/tournaments");
    cy.get('[data-cy="tournaments-list"]').should("be.visible");
  });
});
