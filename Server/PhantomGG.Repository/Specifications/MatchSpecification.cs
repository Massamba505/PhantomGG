using PhantomGG.Repository.Entities;
using PhantomGG.Common.Enums;
using System.Linq.Expressions;

namespace PhantomGG.Repository.Specifications;

public class MatchSpecification
{
    public string? SearchTerm { get; set; }
    public Guid? TournamentId { get; set; }
    public Guid? TeamId { get; set; }
    public List<Guid>? UserTeamIds { get; set; }
    public MatchStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public Expression<Func<Match, bool>> ToExpression()
    {
        return m =>
            (string.IsNullOrEmpty(SearchTerm) ||
                (m.HomeTeam.Name != null && m.HomeTeam.Name.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)) ||
                (m.AwayTeam.Name != null && m.AwayTeam.Name.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)) ||
                (m.Tournament.Name != null && m.Tournament.Name.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase))) &&

            (!TournamentId.HasValue || m.TournamentId == TournamentId.Value) &&

            (!TeamId.HasValue ||
                m.HomeTeamId == TeamId.Value ||
                m.AwayTeamId == TeamId.Value) &&

            (UserTeamIds == null || UserTeamIds.Count == 0 ||
                UserTeamIds.Contains(m.HomeTeamId) ||
                UserTeamIds.Contains(m.AwayTeamId)) &&

            (!Status.HasValue || m.Status == (int)Status) &&

            (!DateFrom.HasValue || m.MatchDate >= DateFrom.Value) &&
            (!DateTo.HasValue || m.MatchDate <= DateTo.Value);
    }
}
