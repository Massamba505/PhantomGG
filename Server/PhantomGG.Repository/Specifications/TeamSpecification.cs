using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using System.Linq.Expressions;

namespace PhantomGG.Repository.Specifications;

public class TeamSpecification
{
    public string? SearchTerm { get; set; }
    public Guid? TournamentId { get; set; }
    public TeamRegistrationStatus? Status { get; set; }
    public bool? IsPublic { get; set; }
    public Guid? UserId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public Expression<Func<Team, bool>> ToExpression()
    {
        return t =>
            (string.IsNullOrEmpty(SearchTerm) ||
                ((t.Name != null && t.Name.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)) ||
                 (t.ShortName != null && t.ShortName.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)))) &&

            (!TournamentId.HasValue ||
                t.TournamentTeams.Any(tt => tt.TournamentId == TournamentId.Value)) &&

            (!Status.HasValue ||
                !TournamentId.HasValue ||
                t.TournamentTeams.Any(tt => tt.TournamentId == TournamentId.Value && tt.Status == (int)Status.Value)) &&

            (!UserId.HasValue || t.UserId == UserId.Value);
    }
}
