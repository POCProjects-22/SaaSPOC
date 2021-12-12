using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace POCRepository.Repository
{
    public interface IUnitOfWork<out TContext>
    where TContext : IdentityDbContext, new()
    {
        TContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
