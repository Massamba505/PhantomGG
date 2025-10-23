import { defineConfig } from "cypress";

export default defineConfig({
  // Simple configuration focused on essential settings
  video: false, // Disable videos to save space
  screenshotOnRunFailure: true,
  
  viewportWidth: 1280,
  viewportHeight: 720,
  
  // Reasonable timeouts
  defaultCommandTimeout: 10000,
  requestTimeout: 10000,
  responseTimeout: 20000,
  pageLoadTimeout: 20000,
  
  // No retries for simpler debugging
  retries: {
    runMode: 1,
    openMode: 0
  },

  e2e: {
    baseUrl: 'http://localhost:4200',
    specPattern: 'cypress/e2e/**/*.cy.{js,jsx,ts,tsx}',
    supportFile: 'cypress/support/e2e.ts',
    
    setupNodeEvents(on, config) {
      // Minimal setup
      on('task', {
        log(message) {
          console.log(message);
          return null;
        }
      });
      
      return config;
    }
  },
});
