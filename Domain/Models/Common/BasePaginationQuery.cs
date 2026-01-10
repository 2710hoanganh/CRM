using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Common
{
	public abstract class BasePaginationQuery : BaseFields
    {
        [Required]
        public required ulong PageNumber { get; set; }

        [Required]
        public required ulong PageSize { get; set; }

        public BasePaginationQuery()
        {
            this.PageNumber = 0;
            this.PageSize = 20;
        }
    }
}

