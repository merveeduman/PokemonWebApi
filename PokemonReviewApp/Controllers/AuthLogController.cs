using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class AuthLogController : ControllerBase
{
    private readonly DataContext _context;

    public AuthLogController(DataContext context)
    {
        _context = context;
    }

    // GET: api/AuthLog
    [HttpGet]
    public async Task<IActionResult> GetAuthLogs()
    {
        var logs = await _context.AuthLogs
            .Include(log => log.User)
            .Select(log => new
            {
                log.LoginId,
                log.UserId,
                log.LoginDate,
                Username = log.User.Name,
                
            })
            .ToListAsync();

        return Ok(logs);
    }



}
