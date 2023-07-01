using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VideoGames.Models
{
    public class Review
    {
        [Key]
        public string? Id { get; set; }
        public string? Author { get; set; }
        [ForeignKey("Game")]
        public string? GameId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
