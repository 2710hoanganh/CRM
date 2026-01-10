using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Common
{
	public class BaseDetailQuery : BaseFields
    {
        [Required]
        public required ulong Id { get; set; }
    }
}

