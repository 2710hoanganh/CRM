using Domain.Entities.Base;
using Domain.Constants.AppEnum;

namespace Domain.Entities
{
    public class UserReference : BaseEntity
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int Relationship { get; set; }
        public int Status { get; set; }
            
        public User User { get; set; } = null!;
    }
}