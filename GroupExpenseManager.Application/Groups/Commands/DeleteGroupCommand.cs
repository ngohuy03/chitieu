using GroupExpenseManager.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Groups.Commands
{
    public class DeleteGroupCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteGroupCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Groups
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Group with id {request.Id} not found.");
            }

            // Soft delete
            entity.IsDelete = 1;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
