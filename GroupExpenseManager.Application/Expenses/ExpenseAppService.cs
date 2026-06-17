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
            if (dto.Splits == null || dto.Splits.Count == 0)
            {
                throw new Exception("Phải có ít nhất một người tham gia để chia tiền.");
            }

            var entity = new Expense
            {
                Title = dto.Title,
                Amount = dto.Amount,
                Date = dto.Date,
                GroupId = dto.GroupId,
                PaidById = dto.PaidById,
                Splits = dto.Splits.Select(s => new ExpenseSplit
                {
                    UserId = s.UserId,
                    OwedAmount = s.OwedAmount
                }).ToList()
            };

            _context.Expenses.Add(entity);
            await _context.SaveChangesAsync(default);

            return entity.Id;
        }

        public async Task<List<ExpenseDto>> GetExpensesAsync(Guid groupId, DateTime? date = null)
        {
            var query = _context.Expenses
                .AsNoTracking()
                .Include(x => x.PaidBy)
                .Include(x => x.Splits)
                    .ThenInclude(s => s.User)
                .Where(x => x.GroupId == groupId);

            if (date.HasValue)
            {
                query = query.Where(x => x.Date.Date == date.Value.Date);
            }

            return await query
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
                .Where(x => x.GroupId == groupId && !x.IsSettled)
                .ToListAsync();

            // 3. Tính toán nợ trực tiếp giữa các cặp (Pairwise Debts)
            var debts = new Dictionary<Guid, Dictionary<Guid, decimal>>();

            foreach (var exp in expenses)
            {
                Guid payerId = exp.PaidById;
                if (payerId == Guid.Empty || !activeUserIds.Contains(payerId)) continue;

                foreach (var split in exp.Splits)
                {
                    Guid debtorId = split.UserId;
                    if (!activeUserIds.Contains(debtorId)) continue;
                    if (debtorId == payerId) continue; // Không tự nợ chính mình

                    if (!debts.ContainsKey(debtorId))
                    {
                        debts[debtorId] = new Dictionary<Guid, decimal>();
                    }

                    if (!debts[debtorId].ContainsKey(payerId))
                    {
                        debts[debtorId][payerId] = 0;
                    }

                    debts[debtorId][payerId] += split.OwedAmount;
                }
            }

            // 4. Cấn trừ nợ giữa các cặp (A nợ B vs B nợ A)
            var settlements = new List<SettlementDto>();
            var processedPairs = new HashSet<(Guid, Guid)>();

            foreach (var debtorId in debts.Keys.ToList())
            {
                foreach (var creditorId in debts[debtorId].Keys.ToList())
                {
                    if (processedPairs.Contains((debtorId, creditorId)) || processedPairs.Contains((creditorId, debtorId)))
                    {
                        continue;
                    }

                    decimal aOwesB = debts[debtorId][creditorId];
                    decimal bOwesA = 0;

                    if (debts.ContainsKey(creditorId) && debts[creditorId].ContainsKey(debtorId))
                    {
                        bOwesA = debts[creditorId][debtorId];
                    }

                    if (aOwesB > bOwesA)
                    {
                        decimal netDebt = aOwesB - bOwesA;
                        if (netDebt > 0.01m)
                        {
                            settlements.Add(new SettlementDto
                            {
                                FromUserName = activeUsers.ContainsKey(debtorId) ? activeUsers[debtorId] : "N/A",
                                ToUserName = activeUsers.ContainsKey(creditorId) ? activeUsers[creditorId] : "N/A",
                                Amount = netDebt
                            });
                        }
                    }
                    else if (bOwesA > aOwesB)
                    {
                        decimal netDebt = bOwesA - aOwesB;
                        if (netDebt > 0.01m)
                        {
                            settlements.Add(new SettlementDto
                            {
                                FromUserName = activeUsers.ContainsKey(creditorId) ? activeUsers[creditorId] : "N/A",
                                ToUserName = activeUsers.ContainsKey(debtorId) ? activeUsers[debtorId] : "N/A",
                                Amount = netDebt
                            });
                        }
                    }

                    processedPairs.Add((debtorId, creditorId));
                }
            }

            return settlements;
        }

        public async Task SettleAllExpensesAsync(Guid groupId)
        {
            var expenses = await _context.Expenses
                .Where(x => x.GroupId == groupId && !x.IsSettled)
                .ToListAsync();

            foreach (var expense in expenses)
            {
                expense.IsSettled = true;
            }

            await _context.SaveChangesAsync(default);
        }
    }
}
