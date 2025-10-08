using PhantomGG.Repository.Entities;
using PhantomGG.Common.Enums;
using System.Linq.Expressions;

namespace PhantomGG.Repository.Specifications;

public class TournamentSpecification
{
    public string? SearchTerm { get; set; }
    public TournamentStatus? Status { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? IsPublic { get; set; }
    public Guid? OrganizerId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public Expression<Func<Tournament, bool>> ToExpression()
    {
        return t =>
            (string.IsNullOrEmpty(SearchTerm) ||
                ((t.Name != null && t.Name.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)) ||
                 (t.Description != null && t.Description.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)))) &&

            (!Status.HasValue || t.Status == (int)Status.Value) &&

            (string.IsNullOrEmpty(Location) ||
                (t.Location != null && t.Location.Contains(Location, StringComparison.CurrentCultureIgnoreCase))) &&

            (!StartDateFrom.HasValue || t.StartDate >= StartDateFrom.Value) &&
            (!StartDateTo.HasValue || t.StartDate <= StartDateTo.Value) &&
            (!IsPublic.HasValue || t.IsPublic == IsPublic.Value) &&
            (!OrganizerId.HasValue || t.OrganizerId == OrganizerId.Value);
    }
}
