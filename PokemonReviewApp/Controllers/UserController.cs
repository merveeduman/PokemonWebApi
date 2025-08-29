using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserController(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }


        // GET: api/User
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserDto>))]
        [Authorize]  // korumak istediğimiz end pointleri [Authorize] bununla işaretleriz
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(users);
        }

        // GET: api/User/{userId}
        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        public IActionResult GetUser(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserDto>(user);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(userDto);
        }

        [HttpGet("{userId}/permissions")] //kullanıcı izinlerini döndürecek istek
        [ProducesResponseType(200, Type = typeof(IEnumerable<PermissionDto>))]
        public IActionResult GetUserPermissions(int userId)
        {
            var permissions = _userRepository.GetUserPermissions(userId);

            if (permissions == null || !permissions.Any())
                return NotFound();

            var permissionDtos = _mapper.Map<List<PermissionDto>>(permissions);

            return Ok(permissionDtos);
        }


        [HttpPost]
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Permission:KullanıcıEkleme")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userCreate, [FromQuery] int roleId)
        {
            if (userCreate == null)
                return BadRequest("User bilgisi boş olamaz.");

            // Aynı email varsa 422 dön
            var existingUser = _userRepository.GetUsers()
                .FirstOrDefault(u => u.Email.Trim().ToUpper() == userCreate.Email.Trim().ToUpper());

            if (existingUser != null)
                return StatusCode(422, "User with this email already exists");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(userCreate);

            try
            {
                // Transaction ile user + role + log kaydı
                var result = await _userRepository.CreateUserWithLogAsync(user, roleId);

                if (!result)
                    return StatusCode(500, "Kullanıcı oluşturulurken hata oluştu (role/log ile birlikte).");

                // CreatedAtAction yerine direkt 201 + user dönüyoruz
                return StatusCode(201, user);
            }
            catch (Exception ex)
            {
                // Hata detayını sadece loglamak istersen: Console veya ILogger kullanabilirsin
                Console.WriteLine($"Hata: {ex.Message} | StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Beklenmedik bir hata oluştu.");
            }
        }





        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(int userId, [FromBody] UserDto updatedUser)// useri veri tabanındna alıp güncelledik dikkat et
        {
            if (updatedUser == null)
                return BadRequest(ModelState);

            if (userId != updatedUser.Id)
                return BadRequest("User ID mismatch");

            var existingUser = _userRepository.GetUserById(userId);
            if (existingUser == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();


            existingUser.Name = updatedUser.Name;
            existingUser.Surname = updatedUser.Surname;
            existingUser.Email = updatedUser.Email;
            existingUser.Password = updatedUser.Password;

            if (!_userRepository.UpdateUser(existingUser))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        // DELETE: api/User/soft/{userId}
        [HttpDelete("soft/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult SoftDeleteUser(int userId)
        {
            var existingUser = _userRepository.GetUserById(userId);
            if (existingUser == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (!_userRepository.SoftDeleteUser(userId))
            {
                ModelState.AddModelError("", "Something went wrong during soft delete");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }
}
