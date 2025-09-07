namespace PhantomGG.API.Common;

public enum UserRoles
{
    Admin,
    Organizer,
    User,
}

public enum ImageType
{
    ProfilePicture,
    TournamentBanner,
    TournamentLogo,
    TeamLogo,
    TeamPhoto,
    PlayerPhoto
}

public enum TournamentStatus
{
    Draft,
    Published,
    RegistrationOpen,
    RegistrationClosed,
    InProgress,
    Completed,
    Cancelled
}

public enum TeamRegistrationStatus
{
    Pending,
    Approved,
    Rejected,
    Withdrawn
}

public enum MatchStatus
{
    Scheduled,
    InProgress,
    Completed,
    Postponed,
    Cancelled,
}

public enum MatchEventType
{
    Goal,
    YellowCard,
    RedCard,
    Substitution,
    CornerKick,
    Offside,
    Foul,
    PenaltyMiss,
    OwnGoal
}

public enum PlayerPosition
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward
}