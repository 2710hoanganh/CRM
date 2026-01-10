namespace Domain.Models.Common
{
	public class ConnectionStrings
	{
        public required string ReadConnection { get; set; }
        public required string WriteConnection { get; set; }
        public required string BackgroundConnection { get; set; }
    }
}