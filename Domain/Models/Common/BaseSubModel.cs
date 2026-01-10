namespace Domain.Models.Common
{
	public class BaseSubModel
	{
        public ulong Id { get; set; }
        public int Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public ulong CreateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public ulong UpdateUser { get; set; }
        public DateTime? DeleteDate { get; set; }
        public ulong DeleteUser { get; set; }
    }
}