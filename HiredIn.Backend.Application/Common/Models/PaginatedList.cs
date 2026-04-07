using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyList<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    public PaginatedList(IReadOnlyList<T> items,int totalCount, int pageIndex, int pageSize)
    {
        if (pageIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(pageIndex));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex + 1 < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var totalCount = await source.CountAsync(cancellationToken);
        var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, totalCount, pageIndex, pageSize);
    }
}
