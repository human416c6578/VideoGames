using System.ComponentModel.DataAnnotations;

namespace VideoGames.Models
{
    public class Game
    {
        [Key]
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public float? Rating { get; set; }

        public string? Description { get; set; }
    }
}
