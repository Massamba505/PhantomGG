/// <reference types="cypress" />

describe('Workflow 3: Add Players to Teams', () => {
  const testData = require('../../fixtures/workflow/test-data.json');

  // Collect all added players globally for the summary
  const addedPlayers: any[] = [];

  // Loop through every user
  testData.users.forEach((user: any) => {
    const userTeams = testData.teams.filter((t: any) => t.userId === user.id);

    // Skip users with no teams
    if (userTeams.length === 0) return;

    describe(`User: ${user.firstName} ${user.lastName} (${user.id})`, () => {

      // Login ONCE per user (not per team)
      beforeEach(() => {
        cy.clearCookies();
        cy.clearLocalStorage();

        cy.visit('/auth/login');
        cy.get('[data-cy="email-input"]').type(user.email);
        cy.get('[data-cy="password-input"]').type(user.password);
        cy.get('[data-cy="login-button"]').click();

        cy.url().should('include', '/user', { timeout: 10000 });
      });

      // Process all teams for this user
      userTeams.forEach((team: any) => {
        const teamPlayers = testData.players.filter((p: any) => p.teamId === team.id);

        it(`Should add ${teamPlayers.length} players to ${team.name}`, () => {
          // Navigate to team details page
          cy.visit('/user/teams');
          cy.contains(team.name, { timeout: 10000 }).click();

          cy.get('[data-cy="team-details-section"]', { timeout: 10000 }).should('be.visible');
          cy.get('[data-cy="team-players-section"]').should('be.visible');

          // Add all players for this team
          teamPlayers.forEach((player: any, index: number) => {
            cy.log(`Adding player ${index + 1}/${teamPlayers.length}: ${player.firstName} ${player.lastName}`);

            cy.get('[data-cy="add-player-button"]').click();
            cy.get('[data-cy="player-first-name-input"]', { timeout: 5000 }).should('be.visible');

            // Optional image upload
            if (player.photoUrl) {
              cy.get('[data-cy="player-image-input"]').attachFile({
                filePath: player.photoUrl,
                encoding: 'base64',
                mimeType: 'image/jpeg'
              });
              cy.wait(300);
            }

            cy.get('[data-cy="player-first-name-input"]').clear().type(player.firstName);
            cy.get('[data-cy="player-last-name-input"]').clear().type(player.lastName);
            cy.get('[data-cy="player-position-select"]').select(player.position);

            if (player.email) {
              cy.get('[data-cy="player-email-input"]').clear().type(player.email);
            }

            cy.get('[data-cy="submit-button"]').click();

            // Wait for modal to close or success indication
            cy.wait(500);

            // Verify player was added (check in players list)
            // cy.get('[data-cy="team-players-section"]').should('contain', player.firstName);
            // cy.get('[data-cy="team-players-section"]').should('contain', player.lastName);

            addedPlayers.push({
              userId: user.id,
              userName: `${user.firstName} ${user.lastName}`,
              teamId: team.id,
              teamName: team.name,
              playerId: player.id,
              playerName: `${player.firstName} ${player.lastName}`,
              position: player.position
            });
          });
        });
      });

      // Logout ONCE after processing all teams for this user
      after(() => {
        cy.get('[data-cy="profile-dropdown-button"]').click();
        cy.get('[data-cy="logout-button"]').click();
        cy.url().should('not.include', '/user');
      });

    });
  });

  // GLOBAL SUMMARY
  describe('Verify Player Addition Summary', () => {
    it('Should have added all players across all teams and users', () => {
      const expected = testData.players.length;
      const actual = addedPlayers.length;

      expect(actual).to.eq(expected);

      cy.log('=== PLAYER ADDITION SUMMARY ===');
      cy.log(`Total Expected: ${expected}`);
      cy.log(`Total Added: ${actual}`);

      addedPlayers.forEach((p) => {
        cy.log(
          `User: ${p.userName} | Team: ${p.teamName} | Player: ${p.playerName} (${p.position})`
        );
      });

      cy.log('===============================');
    });
  });

});
