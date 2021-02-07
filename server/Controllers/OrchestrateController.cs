using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Orchestrate.API.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Orchestrate.API.Controllers
{
    [Authorize]
    [ApiController]
    public class OrchestrateController : ControllerBase
    {
        protected int UserId => int.Parse(User.Identity.Name);
    }
}
