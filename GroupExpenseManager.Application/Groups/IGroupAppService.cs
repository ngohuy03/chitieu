using GroupExpenseManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Application.Groups
{
    public interface IGroupAppService
    {
        Task<Guid> CreateGroupAsync(CreateGroupDto dto);
        Task<List<GroupDto>> GetAllGroupsAsync(string? search);
        Task UpdateGroupAsync(Guid id, UpdateGroupDto dto);
        Task DeleteGroupAsync(Guid id);
    }

    public class CreateGroupDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Guid? ContributorId { get; set; }
    }

    public class UpdateGroupDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ContributorName { get; set; } = string.Empty;
    }
}
