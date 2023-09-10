using DabernaShow.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DabernaShow.Controllers
{
    [Route("api/Leaderboard")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("SetStats")]
        public IActionResult SetStats([FromQuery] string stat, double score, string username)
        {
            try
            {
                User user = new User()
                {
                    Score = score,
                    Username = username,
                    Stats = stat
                };
                
                _userService.UpdateOrCreateUser(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("GetTop")]
        public IActionResult GetTop([FromQuery] string stat)
        {
            try
            {
                var topUsers = _userService.GetTopUsersByStats(stat);
                return Ok(topUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
