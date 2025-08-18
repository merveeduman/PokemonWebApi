using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionController(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        // GET: api/Permission
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<PermissionDto>))]
        public IActionResult GetPermissions()
        {
            var permissions = _mapper.Map<List<PermissionDto>>(_permissionRepository.GetPermissions());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(permissions);
        }

        // GET: api/Permission/{permissionId}
        [HttpGet("{permissionId}")]
        [ProducesResponseType(200, Type = typeof(PermissionDto))]
        [ProducesResponseType(404)]
        public IActionResult GetPermission(int permissionId)
        {
            var permission = _permissionRepository.GetPermissionById(permissionId);
            if (permission == null)
                return NotFound();

            var permissionDto = _mapper.Map<PermissionDto>(permission);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(permissionDto);
        }

        // POST: api/Permission
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public IActionResult CreatePermission([FromBody] PermissionDto permissionCreate)
        {
            if (permissionCreate == null)
                return BadRequest(ModelState);

            var existingPermission = _permissionRepository.GetPermissions()
                .FirstOrDefault(p => p.Name.Trim().ToUpper() == permissionCreate.Name.Trim().ToUpper());

            if (existingPermission != null)
            {
                ModelState.AddModelError("", "Permission already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var permission = _mapper.Map<Permission>(permissionCreate);

            if (!_permissionRepository.CreatePermission(permission))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        // PUT: api/Permission/{permissionId}
        [HttpPut("{permissionId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePermission(int permissionId, [FromBody] PermissionDto updatedPermission)
        {
            if (updatedPermission == null)
                return BadRequest(ModelState);

            if (permissionId != updatedPermission.Id)
                return BadRequest("Permission ID mismatch");

            var existingPermission = _permissionRepository.GetPermissionById(permissionId);
            if (existingPermission == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var permission = _mapper.Map<Permission>(updatedPermission);

            if (!_permissionRepository.UpdatePermission(permission))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE: api/Permission/soft/{permissionId}
        [HttpDelete("soft/{permissionId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult SoftDeletePermission(int permissionId)
        {
            var existingPermission = _permissionRepository.GetPermissionById(permissionId);
            if (existingPermission == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_permissionRepository.SoftDeletePermission(permissionId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
