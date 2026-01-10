namespace Domain.Models.Common
{
	public class AdminAccount
	{
		public required ulong Id { get; set; }
		public required string Email { get; set; }
        public required string Password { get; set; }
	}
}

