using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestrate.API.Authorization;
using System;

namespace Orchestrate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorController : ControllerBase
    {
        [Route("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (context.Error is UserNotExistException)
                return Problem();  // TODO: redirect user to login page
            else if (context.Error is ArgumentException argEx)
                return BadRequest(new { Error = argEx.Message });
            else if (context.Error is DbUpdateException dbEx)
                return StatusCode(500, new { Error = $"Database Error ({dbEx.InnerException.Message})" });

            return Problem();
        }
    }
}
