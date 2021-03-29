using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orchestrate.Data.Models
{
    public record CompositionIdentifier(int GroupId, int CompositionId);

    public class CompositionPayload
    {
        [Required, StringLength(50, MinimumLength = 1)]
        public string Title { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string Composer { get; set; }

        [Required, StringLength(20, MinimumLength = 1)]
        public string Genre { get; set; }
    }

    public class CompositionFields : CompositionPayload
    {
        public int? UploaderId { get; set; }
        public int GroupId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class Composition : CompositionFields
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Group Group { get; set; }

        public User Uploader { get; set; }

        public ICollection<SheetMusic> SheetMusics { get; set; }

        public ICollection<Concert> Concerts { get; set; }
    }
}
