using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("movie_genre")]
	public class MovieGenre : BaseModel {

		[Column("movie_id")]
		public long Id { get; set; }

		[Column("genre")]
		public string Genre { get; set; }
	}
}
