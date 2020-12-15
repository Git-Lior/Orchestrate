using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestrate.API.Models
{
    [Table("composition")]
    public class Composition
    {
        public int GroupID { get; set; }
        public int CompositionID { get; set; }
        public string Title { get; set; }
        public string Composer { get; set; }
        public string Genre { get; set; }

        public int UploaderID { get; set; }
        public User Uploader { get; set; }
        public Group Group { get; set; }
    }
}
