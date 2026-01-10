using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Common
{
	public abstract class BaseEmailModel
	{
        [Required]
        public required string BrandName { get; set; }

        [Required]
        public required string BrandEmail { get; set; }

        [Required]
        public required string BrandPhone { get; set; }

        [Required]
        public required string ReceiverEmail { get; set; }

		[Required]
        public required string ReceiverName { get; set; }
    }
}

