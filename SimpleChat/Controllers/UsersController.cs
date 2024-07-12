using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleChat.DTOs;
using SimpleChat.Services;

namespace SimpleChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            if (user.UserId <= 0)
            {
                return BadRequest($"{nameof(user.UserId)} field is required and must be greater than 0");
            }
            if (string.IsNullOrEmpty(user.NickName))
            {
                return BadRequest($"{nameof(user.NickName)} field is required");
            }
            var createdUser = await _userService.CreateUser(user);
            return CreatedAtAction(nameof(CreateUser), createdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest($"{nameof(id)} must be greater than 0");
            }
            await _userService.DeleteUser(id);
            return Ok("User successfully deleted");
        }
    }
}
