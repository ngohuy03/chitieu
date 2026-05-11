using GroupExpenseManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses.Queries
{
    public class GetExpensesByDateQuery : IRequest<List<ExpenseDto>>
    {
        public Guid GroupId { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaidByName { get; set; } = string.Empty;
    }

    public class GetExpensesByDateQueryHandler : IRequestHandler<GetExpensesByDateQuery, List<ExpenseDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetExpensesByDateQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExpenseDto>> Handle(GetExpensesByDateQuery request, CancellationToken cancellationToken)
        {
            return await _context.Expenses
                .Where(x => x.GroupId == request.GroupId && x.Date.Date == request.Date.Date)
                .Select(x => new ExpenseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Amount = x.Amount,
                    Date = x.Date,
                    PaidByName = x.PaidBy.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
