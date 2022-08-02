using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("user_votes")]
	public class UserVote : BaseModel {

		[Column("id")][Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Column("movie_vote_id")]
		public long MovieVoteId { get; set; }
		public MovieVote MovieVote { get; set; }

		[Column("user_id")]
		public int UserId { get; set; }

		[Column("user_name")]
		public string UserName { get; set; }

		[Column("vote_rank")]
		public long VoteRank { get; set; }
	}
}