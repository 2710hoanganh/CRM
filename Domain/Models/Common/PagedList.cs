namespace Domain.Models.Common
{
    public class PagedList<T> : Paged<List<T>>
    {
        public PagedList(List<T> items, ulong pageNumber, ulong pageSize, ulong totalCount, string? message)
            : base(items, pageNumber, pageSize, totalCount, message)
        {

        }
    }
}

