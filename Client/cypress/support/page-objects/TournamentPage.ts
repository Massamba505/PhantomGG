export class TournamentPage {
  private createTournamentButton = '[data-cy="quick-action-create-tournament"]';
  private nameInput = '[data-cy="tournament-name-input"]';
  private descriptionInput = '[data-cy="tournament-description-input"]';
  private locationInput = '[data-cy="tournament-location-input"]';
  private registrationStartInput = '[data-cy="registration-start-input"]';
  private registrationDeadlineInput = '[data-cy="registration-deadline-input"]';
  private startDateInput = '[data-cy="start-date-input"]';
  private endDateInput = '[data-cy="end-date-input"]';
  private minTeamsInput = '[data-cy="min-teams-input"]';
  private maxTeamsInput = '[data-cy="max-teams-input"]';
  private submitButton = '[data-cy="submit-button"]';
  private editButton = '[data-cy="edit-tournament-button"]';
  private deleteButton = '[data-cy="delete-tournament-button"]';

  visitList() {
    cy.visit('/organizer/tournaments');
  }

  visitCreate() {
    cy.visit('/organizer/tournaments/create');
  }

  visitDetails(id: string) {
    cy.visit(`/organizer/tournaments/${id}`);
  }

  visitEdit(id: string) {
    cy.visit(`/organizer/tournaments/${id}/edit`);
  }

  clickCreate() {
    cy.get(this.createTournamentButton).click();
  }

  fillForm(data: any) {
    cy.get(this.nameInput).type(data.name);
    cy.get(this.descriptionInput).type(data.description);
    
    if (data.location) {
      cy.get(this.locationInput).type(data.location);
    }
    
    cy.get(this.registrationStartInput).type(
      this.formatDateTime(data.registrationStartDate)
    );
    cy.get(this.registrationDeadlineInput).type(
      this.formatDateTime(data.registrationDeadline)
    );
    cy.get(this.startDateInput).type(
      this.formatDateTime(data.startDate)
    );
    cy.get(this.endDateInput).type(
      this.formatDateTime(data.endDate)
    );
    
    cy.get(this.minTeamsInput).clear().type(data.minTeams.toString());
    cy.get(this.maxTeamsInput).clear().type(data.maxTeams.toString());
  }

  submit() {
    cy.get(this.submitButton).click();
  }

  clickDelete() {
    cy.get(this.deleteButton).click();
  }

  private formatDateTime(dateTime: string): string {
    return dateTime.replace(/:\d{2}\.\d{3}Z/, '').replace('Z', '');
  }
}
