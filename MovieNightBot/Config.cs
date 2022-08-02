using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace MovieNightBot {
	public class Config {

		public static Config Init(string configPath) {
			Config conf = null;
			Console.WriteLine(configPath);
			if (System.IO.File.Exists(configPath)) {
				string file = System.IO.File.ReadAllText(configPath);
				IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
				conf = deserializer.Deserialize<Config>(file);
			}

			return conf;
		}

		public string token = "";
		public string db_url = "";
		public int port = 80;
		public string base_url = "";
		public string message_identifier = "m!";

	}
}