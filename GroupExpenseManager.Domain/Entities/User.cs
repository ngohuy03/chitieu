using System;
using System.Collections.Generic;

namespace GroupExpenseManager.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Soft delete: 0 = active, 1 = deleted
        public int IsDelete { get; set; } = 0;

        // Navigation properties
        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
