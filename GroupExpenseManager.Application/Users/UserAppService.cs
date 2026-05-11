using GroupExpenseManager.Application.Interfaces;
using GroupExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Users
{
    public class UserAppService : IUserAppService
    {
        private readonly IApplicationDbContext _context;

        public UserAppService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateUserAsync(CreateUserDto dto)
        {
            var entity = new User
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                IsDelete = 0
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync(default);

            return entity.Id;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(x => x.IsDelete == 0)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (entity == null)
            {
                throw new Exception($"User with id {id} not found.");
            }

            entity.Name = dto.Name;
            entity.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync(default);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var entity = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id && x.IsDelete == 0);

            if (entity == null)
            {
                throw new Exception($"User with id {id} not found.");
            }

            // Soft delete
            entity.IsDelete = 1;

            await _context.SaveChangesAsync(default);
        }
    }
}
