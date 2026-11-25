using Microsoft.EntityFrameworkCore;
using TSP.Common.Dto.Filter;

namespace TSP.Common.Extensions
{
    public static class FilterExtension
    {
        public static async Task<PaginationResponse<T>> PaginateAsync<T>(this IQueryable<T> query, PaginationRequest pagination)
        {
            var pageNumber = Math.Max(1, pagination.PageNumber);
            var pageSize = Math.Clamp(pagination.PageSize, 1, 100);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }
}
