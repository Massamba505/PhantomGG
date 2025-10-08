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


public enum TournamentFormats
{
    SingleElimination,
    RoundRobin,
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
    Assist,
    YellowCard,
    RedCard,
    Foul,
    Substitution
}

public enum PlayerPosition
{
    Goalkeeper,
    Defender,
    Midfielder,
    Forward
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
    Approve = 2,
    Reject = 3
}