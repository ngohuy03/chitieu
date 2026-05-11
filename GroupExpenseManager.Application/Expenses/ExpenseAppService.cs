using GroupExpenseManager.Application.Interfaces;
using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Expenses
{
    public class ExpenseAppService : IExpenseAppService
    {
        private readonly IApplicationDbContext _context;

        public ExpenseAppService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateExpenseAsync(CreateExpenseDto dto)
        {
            if (dto.ParticipantIds == null || dto.ParticipantIds.Count == 0)
            {
                throw new Exception("Phải có ít nhất một người tham gia để chia tiền.");
            }

            decimal owedAmount = dto.Amount / dto.ParticipantIds.Count;

            var entity = new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                Date = dto.Date,
                GroupId = dto.GroupId,
                PaidById = dto.PaidById,
                Splits = dto.ParticipantIds.Select(userId => new ExpenseSplit
                {
                    UserId = userId,
                    OwedAmount = owedAmount
                }).ToList()
            };

            _context.Expenses.Add(entity);
            await _context.SaveChangesAsync(default);

            return entity.Id;
        }

        public async Task<List<ExpenseDto>> GetExpensesByDateAsync(Guid groupId, DateTime date)
        {
            return await _context.Expenses
                .AsNoTracking()
                .Include(x => x.PaidBy)
                .Include(x => x.Splits)
                    .ThenInclude(s => s.User)
                .Where(x => x.GroupId == groupId && x.Date.Date == date.Date)
                .Select(x => new ExpenseDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Amount = x.Amount,
                    Date = x.Date,
                    PaidByName = x.PaidBy != null ? x.PaidBy.Name : "N/A",
                    Splits = x.Splits
                        .Where(s => s.User != null && s.User.IsDelete == 0) // Chỉ lấy người chưa xóa
                        .Select(s => new ExpenseSplitDto
                        {
                            UserName = s.User != null ? s.User.Name : "N/A",
                            OwedAmount = s.OwedAmount
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task DeleteExpenseAsync(Guid id)
        {
            var entity = await _context.Expenses
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new Exception($"Expense with id {id} not found.");
            }

            _context.Expenses.Remove(entity);
            await _context.SaveChangesAsync(default);
        }

        public async Task<List<SettlementDto>> GetSettlementsAsync(Guid groupId)
        {
            // 1. Lấy danh sách ID của các User CHƯA BỊ XÓA
            var activeUsers = await _context.Users
                .AsNoTracking()
                .Where(x => x.IsDelete == 0)
                .ToDictionaryAsync(x => x.Id, x => x.Name);

            var activeUserIds = activeUsers.Keys.ToHashSet();

            // 2. Lấy tất cả chi tiêu trong nhóm
            var expenses = await _context.Expenses
                .AsNoTracking()
                .Include(x => x.Splits)
                .Where(x => x.GroupId == groupId)
                .ToListAsync();

            // 3. Tính toán số dư ròng (Net Balance)
            var balances = new Dictionary<Guid, decimal>();

            foreach (var exp in expenses)
            {
                // Chỉ tính tiền trả nếu người trả chưa bị xóa
                if (exp.PaidById != Guid.Empty && activeUserIds.Contains(exp.PaidById))
                {
                    if (!balances.ContainsKey(exp.PaidById)) balances[exp.PaidById] = 0;
                    balances[exp.PaidById] += exp.Amount;
                }

                foreach (var split in exp.Splits)
                {
                    // Chỉ tính tiền nợ nếu người ăn chưa bị xóa
                    if (activeUserIds.Contains(split.UserId))
                    {
                        if (!balances.ContainsKey(split.UserId)) balances[split.UserId] = 0;
                        balances[split.UserId] -= split.OwedAmount;
                    }
                }
            }

            // 4. Tách ra người nợ (âm) và người chủ nợ (dương)
            var debtors = balances.Where(x => x.Value < -0.01m).Select(x => new { Id = x.Key, Amount = -x.Value }).ToList();
            var creditors = balances.Where(x => x.Value > 0.01m).Select(x => new { Id = x.Key, Amount = x.Value }).ToList();

            var settlements = new List<SettlementDto>();

            int dIdx = 0, cIdx = 0;
            var dList = debtors.Select(x => x.Amount).ToList();
            var cList = creditors.Select(x => x.Amount).ToList();

            // 5. Khớp nợ
            while (dIdx < dList.Count && cIdx < cList.Count)
            {
                decimal amount = Math.Min(dList[dIdx], cList[cIdx]);

                settlements.Add(new SettlementDto
                {
                    FromUserName = activeUsers.ContainsKey(debtors[dIdx].Id) ? activeUsers[debtors[dIdx].Id] : "N/A",
                    ToUserName = activeUsers.ContainsKey(creditors[cIdx].Id) ? activeUsers[creditors[cIdx].Id] : "N/A",
                    Amount = amount
                });

                dList[dIdx] -= amount;
                cList[cIdx] -= amount;

                if (dList[dIdx] < 0.01m) dIdx++;
                if (cList[cIdx] < 0.01m) cIdx++;
            }

            return settlements;
        }
    }
}
