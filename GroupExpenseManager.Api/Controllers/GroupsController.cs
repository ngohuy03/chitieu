using GroupExpenseManager.Application.Groups;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupAppService _groupAppService;

        public GroupsController(IGroupAppService groupAppService)
        {
            _groupAppService = groupAppService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateGroupDto dto)
        {
            var id = await _groupAppService.CreateGroupAsync(dto);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetAll([FromQuery] string? search)
        {
            var result = await _groupAppService.GetAllGroupsAsync(search);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, UpdateGroupDto dto)
        {
            await _groupAppService.UpdateGroupAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _groupAppService.DeleteGroupAsync(id);
            return NoContent();
        }
    }
}
