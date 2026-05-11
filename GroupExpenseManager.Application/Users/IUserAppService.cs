using GroupExpenseManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Users
{
    public interface IUserAppService
    {
        Task<Guid> CreateUserAsync(CreateUserDto dto);
        Task<List<UserDto>> GetAllUsersAsync();
        Task UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task DeleteUserAsync(Guid id);
    }

    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
