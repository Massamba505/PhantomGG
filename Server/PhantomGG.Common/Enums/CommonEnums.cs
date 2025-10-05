namespace PhantomGG.Common.Enums;

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

public enum TournamentScope
{
    Public,
    My,
    All
}

public enum TeamScope
{
    Public,
    My,
    All
}

public enum TeamAction
{
    Register = 0,
    Withdraw = 1,
    Approve = 3,
    Reject = 4
}