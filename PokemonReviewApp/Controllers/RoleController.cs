using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class RoleController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleController(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        // GET: api/Role
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<RoleDto>))]
        

        public IActionResult GetRoles()
        {
            var roless = _mapper.Map<List<RoleDto>>(_roleRepository.GetRoles());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(roless);
        }

        // GET: api/Role/{roleId}
        [HttpGet("{roleId}")]
        [ProducesResponseType(200, Type = typeof(RoleDto))]
        [ProducesResponseType(404)]
        
        //[Authorize(Roles = "Admin")]
        public IActionResult GetRole(int roleId)
        {
            var role = _roleRepository.GetRoleById(roleId);
            if (role == null)
                return NotFound();

            var roleDto = _mapper.Map<RoleDto>(role);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(roleDto);
        }

        // POST: api/Role
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize(Policy = "Permission:Yazma İzni")]

        public IActionResult CreateRole([FromBody] RoleDto roleCreate)
        {
            if (roleCreate == null)
                return BadRequest(ModelState);

            var existingRole = _roleRepository.GetRoles()
                .FirstOrDefault(r => r.Name.Trim().ToUpper() == roleCreate.Name.Trim().ToUpper());

            if (existingRole != null)
            {
                ModelState.AddModelError("", "Role already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = _mapper.Map<Role>(roleCreate);

            if (!_roleRepository.CreateRole(role))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        // PUT: api/Role/{roleId}
        [HttpPut("{roleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateRole(int roleId, [FromBody] RoleDto updatedRole)
        {
            if (updatedRole == null)
                return BadRequest(ModelState);

            if (roleId != updatedRole.Id)
                return BadRequest("Role ID mismatch");

            var existingRole = _roleRepository.GetRoleById(roleId);
            if (existingRole == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var role = _mapper.Map<Role>(updatedRole);

            if (!_roleRepository.UpdateRole(role))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // DELETE: api/Role/soft/{roleId}
        [HttpDelete("soft/{roleId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult SoftDeleteRole(int roleId)
        {
            var existingRole = _roleRepository.GetRoleById(roleId);
            if (existingRole == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_roleRepository.SoftDeleteRole(roleId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
