using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orchestrate.API.Models
{
    [Table("composition")]
    public class Composition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string Composer { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string Genre { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int? UploaderId { get; set; }
        public User Uploader { get; set; }

        public ICollection<SheetMusic> SheetMusics { get; set; }
        public ICollection<ConcertComposition> ConcertCompositions { get; set; }
    }
}
