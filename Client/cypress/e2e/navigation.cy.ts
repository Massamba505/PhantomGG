describe("Navigation", () => {
  const users = require("../fixtures/users.json");

  it("public pages are accessible without login", () => {
    cy.visit("/");
    cy.get("body").should("be.visible");
    cy.contains("PhantomGG").should("exist");

    cy.visit("/public/tournaments");
    cy.get("body").should("be.visible");
  });

  it("user dashboard navigation works", () => {
    cy.login(users.user.email, users.user.password);
    
    cy.visit("/user/dashboard");
    cy.get("[data-cy=\"user-layout\"]").should("be.visible");
    
    cy.visit("/user/teams");
    cy.get("[data-cy=\"user-teams-container\"]").should("be.visible");
    
    cy.visit("/user/tournaments");
    cy.get("[data-cy=\"tournaments-list\"]").should("be.visible");
  });

  it("organizer dashboard navigation works", () => {
    cy.login(users.organizer.email, users.organizer.password);
    
    cy.visit("/organizer/dashboard");
    cy.get("[data-cy=\"organizer-layout\"]").should("be.visible");
    
    cy.visit("/organizer/tournaments");
    cy.get("[data-cy=\"tournaments-list\"]").should("be.visible");
  });

  it("sidebar navigation works for users", () => {
    cy.login(users.user.email, users.user.password);
    cy.visit("/user/dashboard");
    
    cy.get('[data-cy="nav-my-teams"]').click();
    cy.url().should("include", "/user/teams");
    
    cy.get('[data-cy="nav-tournaments"]').click();
    cy.url().should("include", "/user/tournaments");
  });

  it("sidebar navigation works for organizers", () => {
    cy.login(users.organizer.email, users.organizer.password);
    cy.visit("/organizer/dashboard");
    
    cy.get('[data-cy="nav-my-tournaments"]').click();
    cy.url().should("include", "/organizer/tournaments");
  });
});
