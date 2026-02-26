using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories.Base;
using Application.Repositories;
namespace Persistence.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}