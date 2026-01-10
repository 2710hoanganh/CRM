namespace Domain.Models.Common
{
	public class Config
	{
		public required string Pepper { get; set; }
		public required string BrandName { get; set; }
		public required string BrandEmail { get; set; }
		public required string BrandPhone { get; set; }
        public required string BaseUrl { get; set; }
		public required bool EnableQueue { get; set; }
        public required bool EnableLLMQueue { get; set; }
        public required string Version { get; set; }
    }
}