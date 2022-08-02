using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("imdb_info")]
	public class IMDBInfo {

		[Column("imdb_id")][Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
		public string Id { get; set; }

		[Column("title")]
		public string Title { get; set; }

		[Column("canonical_title")]
		public string? CononicalTitle { get; set; }

		[Column("year")]
		public long? ReleaseYear { get; set; }

		[Column("thumbnail_poster_url")]
		public string? ThumbnailPosterURL { get; set; }

		[Column("full_size_poster_url")]
		public string? FullSizePosterURL { get; set; }
	}
}