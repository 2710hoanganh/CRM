namespace Domain.Models.Common
{
	public abstract class PaginationFilter
	{
		public required ulong PageNumber { get; set; }
		public required ulong PageSize { get; set; }

		public PaginationFilter()
		{
			this.PageNumber = 0;
			this.PageSize = 20;
		}
	}
}

