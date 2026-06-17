using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses
{
    public interface IExpenseAppService
    {
        Task<Guid> CreateExpenseAsync(CreateExpenseDto dto);
        Task<List<ExpenseDto>> GetExpensesAsync(Guid groupId, DateTime? date = null);
        Task DeleteExpenseAsync(Guid id);
        Task<List<SettlementDto>> GetSettlementsAsync(Guid groupId);
        Task SettleAllExpensesAsync(Guid groupId);
    }

    public class CreateExpenseDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid GroupId { get; set; }
        public Guid PaidById { get; set; }
        
        public List<CreateExpenseSplitDto> Splits { get; set; } = new List<CreateExpenseSplitDto>();
    }

    public class CreateExpenseSplitDto
    {
        public Guid UserId { get; set; }
        public decimal OwedAmount { get; set; }
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
