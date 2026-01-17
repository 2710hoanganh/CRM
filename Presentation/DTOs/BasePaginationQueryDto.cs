using System.ComponentModel.DataAnnotations;

namespace Presentation.DTOs
{
    public class BasePaginationQueryDto : BaseFieldsDto
    {
        [Required]
        public required ulong PageNumber { get; set; }

        [Required]
        public required ulong PageSize { get; set; }

        public BasePaginationQueryDto()
        {
            this.PageNumber = 0;
            this.PageSize = 20;
        }
    }
}

