using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("movies")]
	public class Movie {

		[Column("id")][Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }// the "IntegerField" from python peewee (or perhaps sqlite) is not a C# int. Longs must be used, or Arithmetic exceptions are generated.

		[Column("server_id")]
		public ulong ServerId { get; set; }
		[ForeignKey("ServerId")]
		public virtual Server Server { get; set; }// Automatically mapped to the server_id field by ServerId

		[Column("movie_name")]
		public string Name { get; set; }

		[Column("suggested_by")]
		public string Suggestor { get; set; }

		[Column("last_score")]
		public float? LastScore { get; set; }

		[Column("num_votes_entered")]
		public long VoteEventCount { get; set; }

		[Column("total_score")]
		public float TotalScore { get; set; }

		[Column("total_votes")]
		public long TotalLifetimeVotes { get; set; }

		[Column("suggested_on")]
		public long SuggestDate { get; set; }

		[Column("watched_on")]
		public long? WatchedDate { get; set; }

		[Column("imdb_id")]
		public string? IMDBId { get; set; }
		public IMDBInfo IMDB { get; set; }
	}
}