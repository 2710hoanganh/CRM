using Domain.Entities.Base;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Type { get; set; }
        public int Status { get; set; }
        public bool IsRead { get; set; }
    }
}