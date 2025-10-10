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
        var searchTerm = SearchTerm?.ToLower();
        var locationTerm = Location?.ToLower();

        return t =>
            (string.IsNullOrEmpty(searchTerm) ||
                ((t.Name != null && t.Name.ToLower().Contains(searchTerm)) ||
                 (t.Description != null && t.Description.ToLower().Contains(searchTerm)))) &&

            (!Status.HasValue || t.Status == (int)Status.Value) &&

            (string.IsNullOrEmpty(locationTerm) ||
                (t.Location != null && t.Location.ToLower().Contains(locationTerm))) &&

            (!StartDateFrom.HasValue || t.StartDate >= StartDateFrom.Value) &&
            (!StartDateTo.HasValue || t.StartDate <= StartDateTo.Value) &&
            (!IsPublic.HasValue || t.IsPublic == IsPublic.Value) &&
            (!OrganizerId.HasValue || t.OrganizerId == OrganizerId.Value);
    }
}
