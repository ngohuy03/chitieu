using GroupExpenseManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses.Commands
{
    public class DeleteExpenseCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteExpenseCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Expense with id {request.Id} not found.");
            }

            _context.Expenses.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
