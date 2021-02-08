using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orchestrate.API.Services.Interfaces;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "AdministratorOnly")]
    public class UsersController : OrchestrateController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserData data)
        {
            return CreatedAtAction(nameof(CreateUser), await _userService.Create(data));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserData data)
        {
            await _userService.Update(id, data);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            await _userService.Delete(id);
            return Ok();
        }
    }
}
