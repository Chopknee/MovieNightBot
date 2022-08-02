using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("servers")]
	public class Server {

		[Column("id")][Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; }

		[Column("channel")]
		public long Channel { get; set; }

		[Column("movie_time")]
		public DateTime MovieTime { get; set; }

		[Column("admin_role")]
		public string AdminRole { get; set; }

		[Column("tie_option")]
		public string TieOption { get; set; }

		[Column("num_movies_per_vote")]
		public long MovieCountPerVote { get; set; }

		[Column("num_votes_per_user")]
		public long VoteCountPerUser { get; set; }

		[Column("block_suggestions")]
		public bool SuggestionsBlocked { get; set; }

		[Column("check_movie_names")]
		public bool MovieNamesChecked { get; set; }

		[Column("message_timeout")]
		public long MessageTimeout { get; set; }

		[Column("allow_tv_shows")]
		public bool TVShowsAllowed { get; set; }
	}
}