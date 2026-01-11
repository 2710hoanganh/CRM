using Persistence.Contexts;
using Persistence.Repositories.Base;
using Domain.Entities;
using Application.Repositories;
namespace Persistence.Repositories
{
    public class UserReferenceRepository : Repository<UserReference>, IUserReferenceRepository
    {
        public UserReferenceRepository(AppDbContext context) : base(context)
        {
        }
    }
}