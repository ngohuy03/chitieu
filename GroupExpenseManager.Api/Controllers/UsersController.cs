using GroupExpenseManager.Application.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserAppService _userAppService;

        public UsersController(IUserAppService userAppService)
        {
            _userAppService = userAppService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateUserDto dto)
        {
            var id = await _userAppService.CreateUserAsync(dto);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var result = await _userAppService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, UpdateUserDto dto)
        {
            await _userAppService.UpdateUserAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _userAppService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
