export class TeamPage {
  private nameInput = '[data-cy="team-name-input"]';
  private shortNameInput = '[data-cy="team-short-name-input"]';
  private createSubmitButton = '[data-cy="team-submit-button"]';
  private updateSubmitButton = '[data-cy="team-submit-button"]';
  private deleteButton = '[data-cy="delete-team-button"]';

  visitList() {
    cy.visit('/user/teams');
    cy.get('[data-cy="teams-list"]').should("be.visible");
  }

  visitCreate() {
    cy.visit('/user/teams/create');
  }

  visitAnyTeamDetail() {
    this.visitList()
    cy.get('[data-cy="teams-list"]').children().first().click()
  }

  visitDetails(id: string) {
    cy.visit(`/user/teams/${id}`);
  }

  visitEdit(id: string) {
    cy.visit(`/user/teams/${id}/edit`);
  }

  fillCreateForm(data: any) {
    cy.get(this.nameInput).type(data.name);
    cy.get(this.shortNameInput).type(data.shortName);
  }

  fillEditForm(data: any) {
    if (data.name) {
      cy.get(this.nameInput).clear().type(data.name);
    }
    if (data.shortName) {
      cy.get(this.shortNameInput).clear().type(data.shortName);
    }
  }

  submitCreate() {
    cy.get(this.createSubmitButton).click();
  }

  submitUpdate() {
    cy.get(this.updateSubmitButton).click();
  }

  clickDelete() {
    cy.get(this.deleteButton).click();
  }
}
