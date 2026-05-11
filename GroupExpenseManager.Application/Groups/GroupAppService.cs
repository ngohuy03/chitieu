using GroupExpenseManager.Application.Interfaces;
using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Groups
{
    public class GroupAppService : IGroupAppService
    {
        private readonly IApplicationDbContext _context;

        public GroupAppService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateGroupAsync(CreateGroupDto dto)
        {
            var entity = new Group
            {
                Code = dto.Code,
                Name = dto.Name,
                Description = dto.Description,
                Amount = dto.Amount,
                ContributorId = dto.ContributorId,
                IsDelete = 0,
                TextSearch = $"{dto.Code} {dto.Name} {dto.Description}".ToLower()
            };

            _context.Groups.Add(entity);
            await _context.SaveChangesAsync(default);

            return entity.Id;
        }

        public async Task<List<GroupDto>> GetAllGroupsAsync(string? search)
        {
            var query = _context.Groups
                .Where(x => x.IsDelete == 0);

            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLower();
                query = query.Where(x => x.TextSearch.Contains(searchLower));
            }

            return await query
                .Select(x => new GroupDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    Amount = x.Amount,
                    ContributorName = x.Contributor != null ? x.Contributor.Name : "N/A"
                })
                .ToListAsync();
        }

        public async Task UpdateGroupAsync(Guid id, UpdateGroupDto dto)
        {
            var entity = await _context.Groups
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (entity == null)
            {
                throw new Exception($"Group with id {id} not found.");
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Amount = dto.Amount;
            entity.TextSearch = $"{entity.Code} {dto.Name} {dto.Description}".ToLower();

            await _context.SaveChangesAsync(default);
        }

        public async Task DeleteGroupAsync(Guid id)
        {
            var entity = await _context.Groups
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (entity == null)
            {
                throw new Exception($"Group with id {id} not found.");
            }

            // Soft delete
            entity.IsDelete = 1;

            await _context.SaveChangesAsync(default);
        }
    }
}
