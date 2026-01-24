using Domain.Entities;
using Persistence.Contexts;
using Persistence.Repositories.Base;
using Application.Repositories;

namespace Persistence.Repositories
{
    public class UserRepaymentRepository : Repository<UserRepayment>, IUserRepaymentRepository
    {
        public UserRepaymentRepository(AppDbContext context) : base(context)
        {
        }
    }
}