My App
src/app/
├── app.component.ts # Root app component
├── app.component.html # Root app template
├── app.config.ts # App configuration
├── app.routes.ts # Root routing configuration
│
├── core/ # Core singleton services
│ ├── index.ts # Core module exports
│ ├── guards/
│ │ ├── auth.guard.ts # Authentication guard
│ │ ├── role.guard.ts # Role-based access guard
│ │ ├── organizer.guard.ts # Organizer-specific guard
│ │ └── team-owner.guard.ts # Team ownership guard
│ ├── interceptors/
│ │ ├── auth.interceptor.ts # JWT token interceptor
│ │ ├── error.interceptor.ts # Global error handler
│ │ └── loading.interceptor.ts # Loading state interceptor
│ ├── services/
│ │ ├── auth.service.ts # Authentication service
│ │ ├── user.service.ts # User management
│ │ ├── notification.service.ts # App notifications
│ │ ├── theme.service.ts # Light/dark theme
│ │ └── storage.service.ts # Local storage utilities
│ └── models/
│ ├── auth.models.ts # Auth-related interfaces
│ ├── api-response.models.ts # API response types
│ └── base.models.ts # Base entity models
│
├── shared/ # Shared components & utilities
│ ├── index.ts # Shared module exports
│ ├── components/
│ │ ├── ui/ # Reusable UI components
│ │ │ ├── modal/
│ │ │ │ ├── modal.component.ts
│ │ │ │ └── modal.component.html
│ │ │ ├── loading-spinner/
│ │ │ │ ├── loading-spinner.component.ts
│ │ │ │ └── loading-spinner.component.html
│ │ │ ├── confirmation-dialog/
│ │ │ │ ├── confirmation-dialog.component.ts
│ │ │ │ └── confirmation-dialog.component.html
│ │ │ ├── pagination/
│ │ │ │ ├── pagination.component.ts
│ │ │ │ └── pagination.component.html
│ │ │ ├── search-filter/
│ │ │ │ ├── search-filter.component.ts
│ │ │ │ └── search-filter.component.html
│ │ │ ├── date-picker/
│ │ │ │ ├── date-picker.component.ts
│ │ │ │ └── date-picker.component.html
│ │ │ ├── file-upload/
│ │ │ │ ├── file-upload.component.ts
│ │ │ │ └── file-upload.component.html
│ │ │ ├── toast/
│ │ │ │ ├── toast.component.ts
│ │ │ │ └── toast.component.html
│ │ │ └── breadcrumb/
│ │ │ ├── breadcrumb.component.ts
│ │ │ └── breadcrumb.component.html
│ │ ├── layouts/
│ │ │ ├── main-layout/
│ │ │ │ ├── main-layout.component.ts
│ │ │ │ └── main-layout.component.html
│ │ │ ├── auth-layout/
│ │ │ │ ├── auth-layout.component.ts
│ │ │ │ └── auth-layout.component.html
│ │ │ ├── user-layout/
│ │ │ │ ├── user-layout.component.ts
│ │ │ │ └── user-layout.component.html
│ │ │ ├── organizer-layout/
│ │ │ │ ├── organizer-layout.component.ts
│ │ │ │ └── organizer-layout.component.html
│ │ │ └── admin-layout/
│ │ │ ├── admin-layout.component.ts
│ │ │ └── admin-layout.component.html
│ │ ├── navigation/
│ │ │ ├── header/
│ │ │ │ ├── header.component.ts
│ │ │ │ └── header.component.html
│ │ │ ├── sidebar/
│ │ │ │ ├── sidebar.component.ts
│ │ │ │ └── sidebar.component.html
│ │ │ ├── navbar/
│ │ │ │ ├── navbar.component.ts
│ │ │ │ └── navbar.component.html
│ │ │ └── footer/
│ │ │ ├── footer.component.ts
│ │ │ └── footer.component.html
│ │ └── cards/
│ │ ├── tournament-card/
│ │ │ ├── tournament-card.component.ts
│ │ │ └── tournament-card.component.html
│ │ ├── team-card/
│ │ │ ├── team-card.component.ts
│ │ │ └── team-card.component.html
│ │ ├── player-card/
│ │ │ ├── player-card.component.ts
│ │ │ └── player-card.component.html
│ │ └── match-card/
│ │ ├── match-card.component.ts
│ │ └── match-card.component.html
│ ├── directives/
│ │ ├── auto-focus.directive.ts # Auto-focus form inputs
│ │ ├── click-outside.directive.ts # Click outside detection
│ │ └── lazy-load.directive.ts # Lazy loading images
│ ├── pipes/
│ │ ├── relative-time.pipe.ts # "2 hours ago" formatting
│ │ ├── truncate.pipe.ts # Text truncation
│ │ ├── safe-html.pipe.ts # Sanitized HTML
│ │ └── file-size.pipe.ts # File size formatting
│ ├── validators/
│ │ ├── custom-validators.ts # Custom form validators
│ │ └── async-validators.ts # Async validation
│ └── utils/
│ ├── date.utils.ts # Date manipulation utilities
│ ├── form.utils.ts # Form helper functions
│ ├── string.utils.ts # String manipulation
│ ├── validation.utils.ts # Validation helpers
│ └── file.utils.ts # File handling utilities
│
├── api/ # API services & models
│ ├── index.ts # API module exports
│ ├── base/
│ │ ├── api-client.service.ts # Base HTTP client
│ │ ├── api-endpoints.ts # API endpoint constants
│ │ └── api-error-handler.ts # API error handling
│ ├── services/
│ │ ├── tournament.service.ts # Tournament API operations
│ │ ├── team.service.ts # Team API operations
│ │ ├── user.service.ts # User API operations
│ │ ├── match.service.ts # Match API operations
│ │ ├── registration.service.ts # Registration API operations
│ │ ├── bracket.service.ts # Bracket API operations
│ │ ├── notification.service.ts # Notification API operations
│ │ └── file-upload.service.ts # File upload operations
│ └── models/
│ ├── tournament.models.ts # Tournament interfaces
│ ├── team.models.ts # Team interfaces
│ ├── user.models.ts # User interfaces
│ ├── match.models.ts # Match interfaces
│ ├── registration.models.ts # Registration interfaces
│ ├── bracket.models.ts # Bracket interfaces
│ └── common.models.ts # Common API models
│
├── features/ # Feature modules
│ ├── auth/ # Authentication features
│ │ ├── auth.component.ts
│ │ ├── auth.component.html
│ │ ├── auth.routes.ts
│ │ └── pages/
│ │ ├── login/
│ │ │ ├── login.component.ts
│ │ │ └── login.component.html
│ │ ├── signup/
│ │ │ ├── signup.component.ts
│ │ │ └── signup.component.html
│ │ ├── forgot-password/
│ │ │ ├── forgot-password.component.ts
│ │ │ └── forgot-password.component.html
│ │ └── reset-password/
│ │ ├── reset-password.component.ts
│ │ └── reset-password.component.html
│ │
│ ├── public/ # Public features (no auth)
│ │ ├── public.routes.ts
│ │ ├── landing/
│ │ │ ├── landing.component.ts
│ │ │ └── landing.component.html
│ │ ├── browse/
│ │ │ ├── tournaments/
│ │ │ │ ├── browse-tournaments.component.ts
│ │ │ │ └── browse-tournaments.component.html
│ │ │ ├── teams/
│ │ │ │ ├── browse-teams.component.ts
│ │ │ │ └── browse-teams.component.html
│ │ │ └── players/
│ │ │ ├── browse-players.component.ts
│ │ │ └── browse-players.component.html
│ │ ├── tournament/
│ │ │ ├── tournament-details/
│ │ │ │ ├── tournament-details.component.ts
│ │ │ │ ├── tournament-details.component.html
│ │ │ │ └── components/
│ │ │ │ ├── tournament-info/
│ │ │ │ │ ├── tournament-info.component.ts
│ │ │ │ │ └── tournament-info.component.html
│ │ │ │ ├── tournament-teams/
│ │ │ │ │ ├── tournament-teams.component.ts
│ │ │ │ │ └── tournament-teams.component.html
│ │ │ │ └── tournament-schedule/
│ │ │ │ ├── tournament-schedule.component.ts
│ │ │ │ └── tournament-schedule.component.html
│ │ │ └── tournament-brackets/
│ │ │ ├── tournament-brackets.component.ts
│ │ │ └── tournament-brackets.component.html
│ │ ├── team/
│ │ │ ├── team-details/
│ │ │ │ ├── team-details.component.ts
│ │ │ │ ├── team-details.component.html
│ │ │ │ └── components/
│ │ │ │ ├── team-info/
│ │ │ │ │ ├── team-info.component.ts
│ │ │ │ │ └── team-info.component.html
│ │ │ │ ├── team-stats/
│ │ │ │ │ ├── team-stats.component.ts
│ │ │ │ │ └── team-stats.component.html
│ │ │ │ └── team-achievements/
│ │ │ │ ├── team-achievements.component.ts
│ │ │ │ └── team-achievements.component.html
│ │ │ └── team-roster/
│ │ │ ├── team-roster.component.ts
│ │ │ └── team-roster.component.html
│ │ └── player/
│ │ └── player-profile/
│ │ ├── player-profile.component.ts
│ │ ├── player-profile.component.html
│ │ └── components/
│ │ ├── player-stats/
│ │ │ ├── player-stats.component.ts
│ │ │ └── player-stats.component.html
│ │ └── player-history/
│ │ ├── player-history.component.ts
│ │ └── player-history.component.html
│ │
│ ├── user/ # User features (authenticated)
│ │ ├── user.routes.ts
│ │ ├── dashboard/
│ │ │ ├── user-dashboard.component.ts
│ │ │ ├── user-dashboard.component.html
│ │ │ └── components/
│ │ │ ├── dashboard-stats/
│ │ │ │ ├── dashboard-stats.component.ts
│ │ │ │ └── dashboard-stats.component.html
│ │ │ ├── recent-activity/
│ │ │ │ ├── recent-activity.component.ts
│ │ │ │ └── recent-activity.component.html
│ │ │ └── upcoming-matches/
│ │ │ ├── upcoming-matches.component.ts
│ │ │ └── upcoming-matches.component.html
│ │ ├── profile/
│ │ │ ├── profile.component.ts
│ │ │ ├── profile.component.html
│ │ │ └── components/
│ │ │ ├── profile-settings/
│ │ │ │ ├── profile-settings.component.ts
│ │ │ │ └── profile-settings.component.html
│ │ │ ├── account-settings/
│ │ │ │ ├── account-settings.component.ts
│ │ │ │ └── account-settings.component.html
│ │ │ └── privacy-settings/
│ │ │ ├── privacy-settings.component.ts
│ │ │ └── privacy-settings.component.html
│ │ ├── tournaments/
│ │ │ ├── my-tournaments/
│ │ │ │ ├── my-tournaments.component.ts
│ │ │ │ └── my-tournaments.component.html
│ │ │ ├── browse-tournaments/
│ │ │ │ ├── browse-tournaments.component.ts
│ │ │ │ └── browse-tournaments.component.html
│ │ │ └── join-tournament/
│ │ │ ├── join-tournament.component.ts
│ │ │ ├── join-tournament.component.html
│ │ │ └── components/
│ │ │ ├── tournament-selector/
│ │ │ │ ├── tournament-selector.component.ts
│ │ │ │ └── tournament-selector.component.html
│ │ │ └── team-selector/
│ │ │ ├── team-selector.component.ts
│ │ │ └── team-selector.component.html
│ │ ├── teams/
│ │ │ ├── my-teams/
│ │ │ │ ├── my-teams.component.ts
│ │ │ │ └── my-teams.component.html
│ │ │ ├── create-team/
│ │ │ │ ├── create-team.component.ts
│ │ │ │ ├── create-team.component.html
│ │ │ │ └── components/
│ │ │ │ └── team-form/
│ │ │ │ ├── team-form.component.ts
│ │ │ │ └── team-form.component.html
│ │ │ ├── manage-team/
│ │ │ │ ├── manage-team.component.ts
│ │ │ │ ├── manage-team.component.html
│ │ │ │ └── components/
│ │ │ │ ├── team-overview/
│ │ │ │ │ ├── team-overview.component.ts
│ │ │ │ │ └── team-overview.component.html
│ │ │ │ ├── team-performance/
│ │ │ │ │ ├── team-performance.component.ts
│ │ │ │ │ └── team-performance.component.html
│ │ │ │ └── team-tournaments/
│ │ │ │ ├── team-tournaments.component.ts
│ │ │ │ └── team-tournaments.component.html
│ │ │ ├── team-roster/
│ │ │ │ ├── team-roster.component.ts
│ │ │ │ ├── team-roster.component.html
│ │ │ │ └── components/
│ │ │ │ ├── member-list/
│ │ │ │ │ ├── member-list.component.ts
│ │ │ │ │ └── member-list.component.html
│ │ │ │ ├── invite-player/
│ │ │ │ │ ├── invite-player.component.ts
│ │ │ │ │ └── invite-player.component.html
│ │ │ │ └── member-actions/
│ │ │ │ ├── member-actions.component.ts
│ │ │ │ └── member-actions.component.html
│ │ │ ├── team-invitations/
│ │ │ │ ├── team-invitations.component.ts
│ │ │ │ ├── team-invitations.component.html
│ │ │ │ └── components/
│ │ │ │ ├── sent-invitations/
│ │ │ │ │ ├── sent-invitations.component.ts
│ │ │ │ │ └── sent-invitations.component.html
│ │ │ │ └── received-invitations/
│ │ │ │ ├── received-invitations.component.ts
│ │ │ │ └── received-invitations.component.html
│ │ │ ├── team-settings/
│ │ │ │ ├── team-settings.component.ts
│ │ │ │ └── team-settings.component.html
│ │ │ └── browse-teams/
│ │ │ ├── browse-teams.component.ts
│ │ │ └── browse-teams.component.html
│ │ ├── schedule/
│ │ │ └── my-schedule/
│ │ │ ├── my-schedule.component.ts
│ │ │ ├── my-schedule.component.html
│ │ │ └── components/
│ │ │ ├── schedule-calendar/
│ │ │ │ ├── schedule-calendar.component.ts
│ │ │ │ └── schedule-calendar.component.html
│ │ │ └── match-details/
│ │ │ ├── match-details.component.ts
│ │ │ └── match-details.component.html
│ │ └── notifications/
│ │ ├── notifications.component.ts
│ │ ├── notifications.component.html
│ │ └── components/
│ │ ├── notification-list/
│ │ │ ├── notification-list.component.ts
│ │ │ └── notification-list.component.html
│ │ └── notification-settings/
│ │ ├── notification-settings.component.ts
│ │ └── notification-settings.component.html
│ │
│ ├── organizer/ # Organizer features
│ │ ├── organizer.routes.ts
│ │ ├── dashboard/
│ │ │ ├── organizer-dashboard.component.ts
│ │ │ ├── organizer-dashboard.component.html
│ │ │ └── components/
│ │ │ ├── tournament-overview/
│ │ │ │ ├── tournament-overview.component.ts
│ │ │ │ └── tournament-overview.component.html
│ │ │ ├── registration-stats/
│ │ │ │ ├── registration-stats.component.ts
│ │ │ │ └── registration-stats.component.html
│ │ │ └── recent-activities/
│ │ │ ├── recent-activities.component.ts
│ │ │ └── recent-activities.component.html
│ │ ├── tournaments/
│ │ │ ├── my-tournaments/
│ │ │ │ ├── organizer-tournaments.component.ts
│ │ │ │ └── organizer-tournaments.component.html
│ │ │ ├── create-tournament/
│ │ │ │ ├── create-tournament.component.ts
│ │ │ │ ├── create-tournament.component.html
│ │ │ │ └── components/
│ │ │ │ ├── tournament-basic-info/
│ │ │ │ │ ├── tournament-basic-info.component.ts
│ │ │ │ │ └── tournament-basic-info.component.html
│ │ │ │ ├── tournament-rules/
│ │ │ │ │ ├── tournament-rules.component.ts
│ │ │ │ │ └── tournament-rules.component.html
│ │ │ │ └── tournament-schedule/
│ │ │ │ ├── tournament-schedule.component.ts
│ │ │ │ └── tournament-schedule.component.html
│ │ │ ├── edit-tournament/
│ │ │ │ ├── edit-tournament.component.ts
│ │ │ │ └── edit-tournament.component.html
│ │ │ ├── tournament-management/
│ │ │ │ ├── tournament-management.component.ts
│ │ │ │ ├── tournament-management.component.html
│ │ │ │ └── components/
│ │ │ │ ├── tournament-teams/
│ │ │ │ │ ├── tournament-teams.component.ts
│ │ │ │ │ └── tournament-teams.component.html
│ │ │ │ ├── tournament-brackets/
│ │ │ │ │ ├── tournament-brackets.component.ts
│ │ │ │ │ └── tournament-brackets.component.html
│ │ │ │ └── tournament-settings/
│ │ │ │ ├── tournament-settings.component.ts
│ │ │ │ └── tournament-settings.component.html
│ │ │ └── tournament-analytics/
│ │ │ ├── tournament-analytics.component.ts
│ │ │ ├── tournament-analytics.component.html
│ │ │ └── components/
│ │ │ ├── participation-stats/
│ │ │ │ ├── participation-stats.component.ts
│ │ │ │ └── participation-stats.component.html
│ │ │ └── financial-summary/
│ │ │ ├── financial-summary.component.ts
│ │ │ └── financial-summary.component.html
│ │ ├── registration/
│ │ │ ├── team-registrations/
│ │ │ │ ├── team-registrations.component.ts
│ │ │ │ ├── team-registrations.component.html
│ │ │ │ └── components/
│ │ │ │ ├── registration-review/
│ │ │ │ │ ├── registration-review.component.ts
│ │ │ │ │ └── registration-review.component.html
│ │ │ │ └── team-verification/
│ │ │ │ ├── team-verification.component.ts
│ │ │ │ └── team-verification.component.html
│ │ │ ├── registration-settings/
│ │ │ │ ├── registration-settings.component.ts
│ │ │ │ └── registration-settings.component.html
│ │ │ └── waitlist-management/
│ │ │ ├── waitlist-management.component.ts
│ │ │ └── waitlist-management.component.html
│ │ ├── matches/
│ │ │ ├── schedule-matches/
│ │ │ │ ├── schedule-matches.component.ts
│ │ │ │ ├── schedule-matches.component.html
│ │ │ │ └── components/
│ │ │ │ ├── match-scheduler/
│ │ │ │ │ ├── match-scheduler.component.ts
│ │ │ │ │ └── match-scheduler.component.html
│ │ │ │ └── time-slot-manager/
│ │ │ │ ├── time-slot-manager.component.ts
│ │ │ │ └── time-slot-manager.component.html
│ │ │ ├── manage-brackets/
│ │ │ │ ├── manage-brackets.component.ts
│ │ │ │ ├── manage-brackets.component.html
│ │ │ │ └── components/
│ │ │ │ ├── bracket-generator/
│ │ │ │ │ ├── bracket-generator.component.ts
│ │ │ │ │ └── bracket-generator.component.html
│ │ │ │ └── seeding-manager/
│ │ │ │ ├── seeding-manager.component.ts
│ │ │ │ └── seeding-manager.component.html
│ │ │ ├── match-results/
│ │ │ │ ├── match-results.component.ts
│ │ │ │ ├── match-results.component.html
│ │ │ │ └── components/
│ │ │ │ ├── result-input/
│ │ │ │ │ ├── result-input.component.ts
│ │ │ │ │ └── result-input.component.html
│ │ │ │ └── result-verification/
│ │ │ │ ├── result-verification.component.ts
│ │ │ │ └── result-verification.component.html
│ │ │ └── dispute-resolution/
│ │ │ ├── dispute-resolution.component.ts
│ │ │ ├── dispute-resolution.component.html
│ │ │ └── components/
│ │ │ ├── dispute-list/
│ │ │ │ ├── dispute-list.component.ts
│ │ │ │ └── dispute-list.component.html
│ │ │ └── dispute-details/
│ │ │ ├── dispute-details.component.ts
│ │ │ └── dispute-details.component.html
│ │ └── communication/
│ │ ├── announcements/
│ │ │ ├── announcements.component.ts
│ │ │ ├── announcements.component.html
│ │ │ └── components/
│ │ │ ├── announcement-editor/
│ │ │ │ ├── announcement-editor.component.ts
│ │ │ │ └── announcement-editor.component.html
│ │ │ └── announcement-list/
│ │ │ ├── announcement-list.component.ts
│ │ │ └── announcement-list.component.html
│ │ ├── team-communication/
│ │ │ ├── team-communication.component.ts
│ │ │ ├── team-communication.component.html
│ │ │ └── components/
│ │ │ ├── message-composer/
│ │ │ │ ├── message-composer.component.ts
│ │ │ │ └── message-composer.component.html
│ │ │ └── message-history/
│ │ │ ├── message-history.component.ts
│ │ │ └── message-history.component.html
│ │ └── support-tickets/
│ │ ├── support-tickets.component.ts
│ │ ├── support-tickets.component.html
│ │ └── components/
│ │ ├── ticket-list/
│ │ │ ├── ticket-list.component.ts
│ │ │ └── ticket-list.component.html
│ │ └── ticket-details/
│ │ ├── ticket-details.component.ts
│ │ └── ticket-details.component.html
│ │
│ ├── admin/ # Admin features
│ │ ├── admin.routes.ts
│ │ ├── dashboard/
│ │ │ ├── admin-dashboard.component.ts
│ │ │ └── admin-dashboard.component.html
│ │ ├── user-management/
│ │ │ ├── admin-users.component.ts
│ │ │ ├── admin-users.component.html
│ │ │ └── components/
│ │ │ ├── user-list/
│ │ │ │ ├── user-list.component.ts
│ │ │ │ └── user-list.component.html
│ │ │ └── user-actions/
│ │ │ ├── user-actions.component.ts
│ │ │ └── user-actions.component.html
│ │ ├── organizer-management/
│ │ │ ├── organizer-management.component.ts
│ │ │ └── organizer-management.component.html
│ │ ├── platform-settings/
│ │ │ ├── platform-settings.component.ts
│ │ │ └── platform-settings.component.html
│ │ ├── content-moderation/
│ │ │ ├── content-moderation.component.ts
│ │ │ └── content-moderation.component.html
│ │ ├── analytics/
│ │ │ ├── platform-analytics.component.ts
│ │ │ └── platform-analytics.component.html
│ │ └── system-health/
│ │ ├── system-health.component.ts
│ │ └── system-health.component.html
│ │
│ ├── not-found/ # Error pages
│ │ ├── not-found.component.ts
│ │ └── not-found.component.html
│ └── unauthorized/
│ ├── unauthorized.component.ts
│ └── unauthorized.component.html
│
└── environments/ # Environment configurations
├── environment.ts # Production environment
└── environment.development.ts # Development environment
