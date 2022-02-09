using System.ComponentModel.DataAnnotations;

namespace P320FrontToBack.Models
{
    public class Bio
    {
        public int Id { get; set; }

        [Required]
        public string Logo { get; set; }

        [StringLength(100)]
        public string FacebookUrl { get; set; }

        [StringLength(100)]
        public string LinkedinUrl { get; set; }
    }
}
