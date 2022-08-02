using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("movie_votes")]
	public class MovieVote : BaseModel {

		[Column("id")][Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Column("vote_id")]
		public long VoteId { get; set; }
		public Vote Vote { get; set; }

		[Column("movie_id")]
		public long MovieId { get; set; }
		public Vote Movie { get; set; }

		[Column("score")]
		public float Score { get; set; }

		[Column("emoji")]
		public string Emoji { get; set; }
	}
}