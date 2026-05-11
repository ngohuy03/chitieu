using System.Threading;
using System.Threading.Tasks;
using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroupExpenseManager.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Group> Groups { get; }
        DbSet<Expense> Expenses { get; }
        DbSet<ExpenseSplit> ExpenseSplits { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
