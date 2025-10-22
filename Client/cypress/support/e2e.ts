import './commands';

beforeEach(() => {
  cy.clearLocalStorage();
  cy.clearCookies();
  
  cy.viewport(1280, 720);
});

Cypress.on('uncaught:exception', (err, runnable) => {
  if (err.message.includes('ResizeObserver loop limit exceeded')) {
    return false;
  }
  return true;
});