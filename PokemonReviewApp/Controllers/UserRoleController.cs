using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;

namespace PokemonReviewApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;

        public UserRoleController(IUserRoleRepository userRoleRepository, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        // Tüm user-role ilişkilerini getir (User ve Role detayları ile birlikte)
        [HttpGet]
        public IActionResult GetAllUserRoles()
        {
            var userRoles = _userRoleRepository.GetAllUserRolesWithDetails();

            if (userRoles == null || userRoles.Count == 0)
                return Ok(new List<UserRoleDto>());

            return Ok(userRoles);
        }

        // Belirli bir user ve role için ilişki ekle
        [HttpPost]
        public IActionResult AssignRoleToUser([FromBody] CreateUserRoleDto createUserRoleDto)
        {
            if (createUserRoleDto == null)
                return BadRequest("Invalid data.");

            // İki parametreyi de gönderiyoruz:
            var result = _userRoleRepository.AssignRoleToUser(createUserRoleDto.UserId, createUserRoleDto.RoleId);

            if (!result)
                return Conflict("Role assignment already exists or user/role not found.");

            return Ok("Role assigned successfully.");
        }

        // Kullanıcının rolleri (Role detayları ile)
        [HttpGet("user/{userId}")]
        public IActionResult GetRolesByUser(int userId)
        {
            var roles = _userRoleRepository.GetRolesByUser(userId);

            if (roles == null || roles.Count == 0)
                return NotFound($"No roles found for user with ID {userId}.");

            var rolesDto = _mapper.Map<List<RoleDto>>(roles);
            return Ok(rolesDto);
        }

        // Rolün kullanıcıları (User detayları ile)
        [HttpGet("role/{roleId}")]
        public IActionResult GetUsersByRole(int roleId)
        {
            var users = _userRoleRepository.GetUsersByRole(roleId);

            if (users == null || users.Count == 0)
                return NotFound($"No users found for role with ID {roleId}.");

            var usersDto = _mapper.Map<List<UserDto>>(users);
            return Ok(usersDto);
        }

        // Soft delete ile ilişki kaldır
        [HttpDelete("soft/{userId}/{roleId}")]
        public IActionResult SoftDeleteUserRole(int userId, int roleId)
        {
            var success = _userRoleRepository.SoftDeleteUserRole(userId, roleId);

            if (!success)
                return StatusCode(500, "Failed to soft delete user-role assignment.");

            return NoContent();
        }
    }

}
