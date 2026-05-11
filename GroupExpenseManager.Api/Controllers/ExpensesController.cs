using GroupExpenseManager.Application.Expenses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupExpenseManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseAppService _expenseAppService;

        public ExpensesController(IExpenseAppService expenseAppService)
        {
            _expenseAppService = expenseAppService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateExpenseDto dto)
        {
            var id = await _expenseAppService.CreateExpenseAsync(dto);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<List<ExpenseDto>>> GetByDate([FromQuery] Guid groupId, [FromQuery] DateTime date)
        {
            var result = await _expenseAppService.GetExpensesByDateAsync(groupId, date);
            return Ok(result);
        }

        [HttpGet("settlements")]
        public async Task<ActionResult<List<SettlementDto>>> GetSettlements([FromQuery] Guid groupId)
        {
            var result = await _expenseAppService.GetSettlementsAsync(groupId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _expenseAppService.DeleteExpenseAsync(id);
            return NoContent();
        }
    }
}
