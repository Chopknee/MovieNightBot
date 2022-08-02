using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieNightBot.Database.Models {
	[Table("votes")]
	public class Vote {

		[Column("server_id")][Key][DatabaseGenerated(DatabaseGeneratedOption.None)]
		public long ServerId { get; set; }

		[Column("message_id")]
		public long? MessageId { get; set; }

		[Column("channel_id")]
		public long? ChannelId { get; set; }
	}
}
