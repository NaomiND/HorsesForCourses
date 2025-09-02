using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Application.Paging;

public static class PagingExecution
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest request,
        CancellationToken ct = default) where T : class
    {
        var total = await query.CountAsync(ct);
        var pageItems = await query
            .ApplyPaging(request)
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedResult<T>(pageItems, total, request.Page, request.Size);
    }
}

//We splitsen Add en Save op. 
//Dit is een veelgebruikt patroon (Unit of Work) waarbij je meerdere wijzigingen kunt groeperen en in één transactie kunt opslaan door SaveChangesAsync aan te roepen.
