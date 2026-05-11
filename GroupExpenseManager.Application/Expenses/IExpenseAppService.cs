using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses
{
    public interface IExpenseAppService
    {
        Task<Guid> CreateExpenseAsync(CreateExpenseDto dto);
        Task<List<ExpenseDto>> GetExpensesByDateAsync(Guid groupId, DateTime date);
        Task DeleteExpenseAsync(Guid id);
        Task<List<SettlementDto>> GetSettlementsAsync(Guid groupId);
    }

    public class CreateExpenseDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid GroupId { get; set; }
        public Guid PaidById { get; set; }
        
        // List of user IDs who participated in this expense
        public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
    }

    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaidByName { get; set; } = string.Empty;
        public List<ExpenseSplitDto> Splits { get; set; } = new List<ExpenseSplitDto>();
    }

    public class ExpenseSplitDto
    {
        public string UserName { get; set; } = string.Empty;
        public decimal OwedAmount { get; set; }
    }

    public class SettlementDto
    {
        public string FromUserName { get; set; } = string.Empty;
        public string ToUserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
