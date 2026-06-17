using System;
using System.Collections.Generic;

namespace GroupExpenseManager.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsSettled { get; set; } = false;

        // Foreign keys
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;

        public Guid PaidById { get; set; }
        public User PaidBy { get; set; } = null!;

        public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
    }
}
