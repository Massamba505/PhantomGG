# E2E Workflow Tests

This directory contains end-to-end workflow tests that simulate real-world user journeys through the PhantomGG application.

## Overview

The workflow tests are designed to run sequentially and create a complete test environment with:
- User and organizer accounts
- Teams with logos
- Players with photos
- Tournaments

## Test Files

### 01-create-accounts.cy.ts
**Purpose:** Create all user and organizer accounts

**What it does:**
- Creates 10 user accounts from test-data.json
- Creates 3 organizer accounts from test-data.json
- Tests the complete signup flow including:
  - Form validation
  - Password strength checking
  - Role selection
  - Email verification flow

**Duration:** ~2-3 minutes

**Key Test Data:**
- Users: 10 accounts (user-001 to user-010)
- Organizers: 3 accounts (org-001 to org-003)
- All accounts use password: `Password123!`

---

### 02-create-teams.cy.ts
**Purpose:** Login each user and create their teams

**What it does:**
- Logs in as each user who has teams
- Creates all teams assigned to that user
- Uploads team logos from test images
- Tests the team creation flow including:
  - Authentication
  - Form validation
  - File uploads
  - Navigation

**Duration:** ~5-7 minutes

**Key Test Data:**
- Total teams: 40
- Teams per user vary (user-001 has 4 teams, user-002 has 4 teams, etc.)
- Each team has a unique logo

---

### 03-add-players.cy.ts
**Purpose:** Add players to a sample team

**What it does:**
- Logs in as the first user (Thabo Mokoena)
- Navigates to their first team (Johannesburg Lions)
- Adds 7 players to the team with photos
- Tests the player management flow including:
  - Modal/form interactions
  - File uploads
  - Position selection
  - Email validation

**Duration:** ~2-3 minutes

**Key Test Data:**
- Target user: user-001 (Thabo Mokoena)
- Target team: Johannesburg Lions
- Players: 7 (Goalkeeper, Defenders, Midfielders, Forwards)

---

## Running the Workflows

### Run All Workflows in Sequence
```bash
npm run cypress:open
```
Then select the workflow files in order (01, 02, 03)

### Run from Command Line
```bash
# Run all workflow tests
npx cypress run --spec "cypress/e2e/workflows/*.cy.ts"

# Run specific workflow
npx cypress run --spec "cypress/e2e/workflows/01-create-accounts.cy.ts"
npx cypress run --spec "cypress/e2e/workflows/02-create-teams.cy.ts"
npx cypress run --spec "cypress/e2e/workflows/03-add-players.cy.ts"
```

### Run in Sequence (Recommended)
```bash
npx cypress run --spec "cypress/e2e/workflows/01-create-accounts.cy.ts,cypress/e2e/workflows/02-create-teams.cy.ts,cypress/e2e/workflows/03-add-players.cy.ts"
```

---

## Test Data

All test data is located in `cypress/fixtures/workflow/test-data.json`

### Test Images
Test images are located in `cypress/fixtures/workflow/images/`:
- `profilepictures/` - User and organizer profile pictures (13 images)
- `playerpictures/` - Player photos (7 images)
- `teamlogos/` - Team logos (40 images)
- `tournamentlogos/` - Tournament logos (12 images)
- `banners/` - Tournament banners (12 images)

---

## Data-cy Attributes Used

### Authentication
- `signup-container`, `signup-form`, `signup-submit-button`
- `first-name-input`, `last-name-input`, `email-input`
- `password-input`, `confirm-password-input`
- `password-strength`
- `role-card-User`, `role-card-Organizer`
- `login-button`, `logout-button`
- `profile-dropdown-button`

### Teams
- `team-name-input`, `team-short-name-input`
- `team-image-input`, `team-submit-button`
- `team-details-section`, `team-players-section`
- `delete-team-button`, `confirm-delete-button`
- `add-player-button`, `players-list`

### Players
- `player-first-name-input`, `player-last-name-input`
- `player-position-select`, `player-email-input`
- `player-image-input`, `submit-button`

---

## Expected Results

After running all three workflows, you should have:
- ✅ 13 user/organizer accounts created
- ✅ 40 teams created with logos
- ✅ 7 players added to the Johannesburg Lions team
- ✅ All test data properly associated

---

## Troubleshooting

### Common Issues

**Issue:** "Email already exists" error
**Solution:** Clear the database before running the workflows

**Issue:** File upload failures
**Solution:** Ensure `cypress-file-upload` plugin is installed and images exist in the correct paths

**Issue:** Tests timing out
**Solution:** Increase timeout values in cypress.config.ts or check API server is running

**Issue:** Login redirects not working
**Solution:** Verify email verification is disabled for test accounts or manually verify emails

---

## Notes

- These tests create real data in the database
- Run them against a test/development environment only
- Database should be cleared between test runs for consistency
- Tests are designed to be idempotent when possible
- Some tests may need adjustment based on email verification settings

---

## Future Workflows

Planned additional workflows:
- 04-create-tournaments.cy.ts - Create tournaments as organizers
- 05-register-teams.cy.ts - Register teams for tournaments
- 06-approve-teams.cy.ts - Approve team registrations
- 07-create-matches.cy.ts - Generate tournament fixtures
- 08-record-results.cy.ts - Record match results and events

---

## Maintenance

When updating these tests:
1. Keep test data in sync with test-data.json
2. Update data-cy attributes if UI changes
3. Adjust timeouts if performance changes
4. Document any new workflows added
5. Keep README updated with current state

Last Updated: November 20, 2025
