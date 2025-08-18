using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionsRepository _rolePermissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public RolePermissionController(
            IRolePermissionsRepository rolePermissionRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        // GET: api/RolePermission
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RolePermissionDto>))]
        [HttpGet("{roleId}/{permissionId}")]
     
        public IActionResult GetRolePermission(int roleId, int permissionId)
        {
            if (roleId <= 0 || permissionId <= 0)
                return BadRequest("RoleId and PermissionId must be positive integers.");

            var rolePermission = _rolePermissionRepository.GetRolePermissionByIds(roleId, permissionId);
            if (rolePermission == null)
                return NotFound();

            var rolePermissionDto = _mapper.Map<RolePermissionDto>(rolePermission);
            return Ok(rolePermissionDto);
        }

        // GET: api/RolePermission/{roleId}/permissions
        [HttpGet("{roleId}/permissions")]
        [ProducesResponseType(200, Type = typeof(RoleWithPermissionsDto))]
        [ProducesResponseType(404)]
        
        public IActionResult GetPermissionsByRoleId(int roleId)
        {
            var role = _roleRepository.GetRoleById(roleId);
            if (role == null || role.IsDeleted)
                return NotFound();

            var rolePermissions = _rolePermissionRepository
                .GetRolePermissions()
                .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
                .ToList();

            var permissions = rolePermissions
                .Select(rp => _permissionRepository.GetPermissionById(rp.PermissionId))
                .Where(p => p != null && !p.IsDeleted)
                .ToList();

            var result = new RoleWithPermissionsDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = _mapper.Map<List<PermissionDto>>(permissions)
            };

            return Ok(result);
        }

        // POST: api/RolePermission
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreateRolePermission([FromBody] RolePermissionCreateDto createDto)
        {
            if (createDto == null)
                return BadRequest();

            if (_rolePermissionRepository.RolePermissionExists(createDto.RoleId, createDto.PermissionId))
            {
                ModelState.AddModelError("", "Role-permission already exists.");
                return StatusCode(422, ModelState);
            }

            var rolePermission = _mapper.Map<RolePermission>(createDto);

            if (!_rolePermissionRepository.CreateRolePermission(rolePermission))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("soft/{roleId}/{permissionId}")]
        public IActionResult SoftDeleteRolePermission(int roleId, int permissionId)
        {
            if (roleId <= 0 || permissionId <= 0)
                return BadRequest("RoleId and PermissionId must be positive integers.");

            var existingRolePermission = _rolePermissionRepository.GetRolePermissionByIds(roleId, permissionId);
            if (existingRolePermission == null)
                return NotFound();

            if (!_rolePermissionRepository.SoftDeleteRolePermission(roleId, permissionId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
