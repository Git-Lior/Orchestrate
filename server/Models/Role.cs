using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.API.Models
{
    public class RolePayload
    {
        [Required, StringLength(15, MinimumLength = 2)]
        public string Section { get; set; }

        [Range(1, 10)]
        public int? Num { get; set; }
    }

    public class Role : RolePayload
    {
        public int Id { get; set; }
    }
}
