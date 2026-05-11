using System;
using System.Collections.Generic;

namespace GroupExpenseManager.Domain.Entities
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key for contributor (if a group represents a fund or contribution)
        public Guid? ContributorId { get; set; }
        public User? Contributor { get; set; }

        // Soft delete: 0 = active, 1 = deleted
        public int IsDelete { get; set; } = 0;

        // Field for searching
        public string TextSearch { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
