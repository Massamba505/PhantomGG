namespace PhantomGG.Common.Enums;

public enum UserRoles
{
    Admin = 1,
    Organizer = 2,
    User = 3
}

public enum ImageType
{
    ProfilePicture = 1,
    TournamentBanner = 2,
    TournamentLogo = 3,
    TeamLogo = 4,
    TeamPhoto = 5,
    PlayerPhoto = 6
}

public enum TournamentStatus
{
    Draft = 1,
    RegistrationOpen = 2,
    RegistrationClosed = 3,
    InProgress = 4,
    Completed = 5,
    Cancelled = 6
}

public enum TournamentFormats
{
    SingleElimination = 1,
    RoundRobin = 2
}

public enum TeamRegistrationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Withdrawn = 4
}

public enum MatchStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Postponed = 4,
    Cancelled = 5
}

public enum MatchEventType
{
    Goal = 1,
    Assist = 2,
    YellowCard = 3,
    RedCard = 4,
    Foul = 5,
    Substitution = 6
}

public enum PlayerPosition
{
    Goalkeeper = 1,
    Defender = 2,
    Midfielder = 3,
    Forward = 4
}

public enum TeamScope
{
    Public = 1,
    My = 2,
    All = 3
}

public enum TeamAction
{
    Register = 1,
    Withdraw = 2,
    Approve = 3,
    Reject = 4
}