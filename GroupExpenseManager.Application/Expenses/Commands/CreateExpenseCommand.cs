using GroupExpenseManager.Application.Interfaces;
using GroupExpenseManager.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses.Commands
{
    public class CreateExpenseCommand : IRequest<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Guid GroupId { get; set; }
        public Guid PaidById { get; set; }
        public List<ExpenseSplitDto> Splits { get; set; } = new List<ExpenseSplitDto>();
    }

    public class ExpenseSplitDto
    {
        public Guid UserId { get; set; }
        public decimal OwedAmount { get; set; }
    }

    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = new Expense
            {
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                GroupId = request.GroupId,
                PaidById = request.PaidById,
                Splits = request.Splits.Select(s => new ExpenseSplit
                {
                    UserId = s.UserId,
                    OwedAmount = s.OwedAmount
                }).ToList()
            };

            _context.Expenses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
