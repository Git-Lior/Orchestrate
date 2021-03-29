using System;
using System.ComponentModel.DataAnnotations;

namespace Orchestrate.Data.Models
{
    public record RoleIdentifier(int RoleId);

    public class RolePayload
    {
        [Required, StringLength(15, MinimumLength = 2)]
        public string Section { get; set; }

        [Range(0, 10)]
        public int Num { get; set; }
    }

    public class Role : RolePayload
    {
        public int Id { get; set; }
    }
}
