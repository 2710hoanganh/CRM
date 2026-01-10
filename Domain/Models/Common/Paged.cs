
namespace Domain.Models.Common
{
	public class Paged<T>
    {
        public ulong PageNumber { get; set; }
        public ulong PageSize { get; set; }
        public ulong FirstPage { get; set; }
        public ulong LastPage { get; set; }
        public ulong TotalPages { get; set; }
        public ulong TotalRecords { get; set; }
        public bool NextPage { get; set; }
        public bool PreviousPage { get; set; }
        public T Data { get; set; }

        public Paged(T data, ulong pageNumber, ulong pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
        }

        public Paged(T data, ulong pageNumber, ulong pageSize, ulong totalRecords)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.TotalRecords = totalRecords;
            this.SetData();
        }

        private void SetData()
        {
            var totalPages = this.TotalRecords == 0 ? 0 : ((double)this.TotalRecords / (double)this.PageSize);
            ulong roundedTotalPages = Convert.ToUInt64(Math.Ceiling(totalPages));

            this.FirstPage = 0;
            this.LastPage = roundedTotalPages == 0 ? 0 : (roundedTotalPages - 1);

            this.NextPage = this.PageNumber < this.LastPage;
            this.PreviousPage = this.PageNumber != 0;

            this.TotalPages = roundedTotalPages;
        }
    }
}

