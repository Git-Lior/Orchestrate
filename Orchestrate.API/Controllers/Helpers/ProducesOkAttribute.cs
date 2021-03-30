using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Orchestrate.API.Controllers
{
    public class ProducesOkAttribute : ProducesResponseTypeAttribute
    {
        public ProducesOkAttribute() : base(StatusCodes.Status200OK) { }

        public ProducesOkAttribute(Type type) : base(type, StatusCodes.Status200OK) { }
    }
}