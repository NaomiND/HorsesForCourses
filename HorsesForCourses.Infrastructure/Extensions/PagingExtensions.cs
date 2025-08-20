using HorsesForCourses.Application;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Infrastructure.Extensions;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PageRequest request)
    {
        if (!query.Expression.ToString().Contains("OrderBy"))
        {
            // In een productie-app kun je hier een default sortering forceren, 
            // maar een exception is explicieter en dwingt de ontwikkelaar tot een bewuste keuze.
            throw new InvalidOperationException("Apply an OrderBy before paging to ensure stable results.");
        }

        int skip = (request.Page - 1) * request.Size;
        return query.Skip(skip).Take(request.Size);
    }
}

public static class PagingExecution
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PageRequest request,
        CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var pageItems = await query
            .ApplyPaging(request)
            .ToListAsync(ct);

        return new PagedResult<T>(pageItems, total, request.Page, request.Size);
    }
}
