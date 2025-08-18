using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Hash; 

namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityController : ControllerBase
    {
        private readonly PasswordMigrationService _migrationService;

        public UtilityController(PasswordMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        [HttpPost("hash-passwords")]
        public IActionResult HashPasswords()
        {
            _migrationService.MigratePasswords();
            return Ok("Şifreler başarıyla hash'lendi.");
        }
    }
}
