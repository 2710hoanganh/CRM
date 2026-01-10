namespace Domain.Models.Common
{
	public class PagedFilter
	{
        public ulong PageNumber { get; set; }
        public ulong PageSize { get; set; }

        public PagedFilter()
        {
            this.PageNumber = 0;
            this.PageSize = 20;
        }

        public PagedFilter(ulong pageNumber, ulong pageSize)
        {
            this.PageNumber = pageNumber < 0 ? 0 : pageNumber;
            this.PageSize = pageSize > 100 ? 10 : pageSize;
        }
    }
}

