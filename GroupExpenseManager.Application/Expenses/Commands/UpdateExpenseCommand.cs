using GroupExpenseManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses.Commands
{
    public class UpdateExpenseCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Expense with id {request.Id} not found.");
            }

            entity.Title = request.Title;
            entity.Amount = request.Amount;
            entity.Date = request.Date;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
