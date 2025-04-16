using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceControl.API.Data;
using FinanceControl.API.Models;

namespace FinanceControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public ExpensesController(FinanceDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Expense>>> GetExpenses()
    {
        return await _context.Expenses
            .OrderByDescending(e => e.Date)  // Ordenar por data, mais recente primeiro
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        return expense;
    }

    [HttpPost]
    public async Task<ActionResult<Expense>> PostExpense(Expense expense)
    {
        if (expense.Date == default)
        {
            expense.Date = DateTime.UtcNow;
        }

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        // Retornar o objeto criado
        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
    }
} 