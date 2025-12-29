namespace Chat.Common.Extensions
{
    public class PagedList<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public List<T> Items { get; set; }

        public PagedList(List<T> items, int currentPage, int pageSize, int totalItems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            Items = items;
        }

    }

    public static class PagingHelper
    {
        public static PagedList<T> Empty<T>(int page, int pageSize) => new PagedList<T>(new List<T>(), page, pageSize, 0);

        public static async Task<PagedList<T>> GetPaged<T>(IQueryable<T> query, int page, int pageSize)
        {
            int totalItems = await query.CountAsync();
            List<T> items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, page, pageSize, totalItems);
        }
    }

    public static class PaginationValidator
    {
        public static (bool isValid, int Page, int PageSize, bool Failure) Validate(int page, int pageSize)
        {
            var isValid = page >= 0 && pageSize >= 0 && pageSize <= 50;
            return (isValid, isValid ? page : 0, isValid ? pageSize : 0, !isValid);
        }
    }
}
