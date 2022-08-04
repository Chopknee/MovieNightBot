using IMDbApiLib;

namespace MovieNightBot {
	public class Util {
		//If a relative filepath is provided, 
		public static string GetFilePath(string path) {

			if (!System.IO.Path.IsPathRooted(path))// Attempt to use it as a relative path to the working directory
				path = System.IO.Directory.GetCurrentDirectory() + @"\" + @path;
				
			return path;
		}

		public static bool FileExists(string path) {
			path = GetFilePath(path);
			return System.IO.File.Exists(path);
		}

		public static string CapitalizeMovieName(string name) {
			List<string> cleanedName = new List<string>();
			string[] split = name.Trim().Split(' ');
			for (int i = 0; i < split.Length; i++) {
				cleanedName.Add(CapitalizeWord(split[i]));
			}

			string correctedName = string.Empty;

			for (int i = 0; i < cleanedName.Count; i++) {
				correctedName += cleanedName[i];
				if (i < cleanedName.Count - 1)
					correctedName += " ";
			}

			return correctedName;
		}

		public static string CapitalizeWord(string name) {
			if (string.IsNullOrEmpty(name))
				return string.Empty;
			char[] a = name.ToCharArray();
			a[0] = char.ToUpper(a[0]);
			return new string(a);
		}

		private static ApiLib _imdbAPI = null;
		private static ApiLib imdbApi {
			get {
				if (_imdbAPI == null)
					_imdbAPI = new ApiLib(Application.config.imdb_api_key);
				return _imdbAPI;
			}
		}

		// This function should attempt to find the IMDB info based on the movie name.
		// Initially checks the cache to see of the exact match has been found before
		// If the exact match hasn't been cached perform a basic search?
		public static async Task<Database.Models.IMDBInfo> SearchIMDBByTitle(string movie_name, bool bIncludeTVShows) {
			try {
				using (Database.Controller controller = Database.Controller.GetDBController()) {
					// Firstly, search for a matching movie name in the cached data.
					Database.Models.IMDBInfo cachedInfo = null;
					try {
						cachedInfo = controller.IMDBInfo.Single(entry => entry.Title == movie_name);
					} catch { }

					if (cachedInfo != null) {
						//Found an exact match, return the info.
						return cachedInfo;
					}
				}

				List<IMDbApiLib.Models.SearchResult> results = new List<IMDbApiLib.Models.SearchResult>();

				var moviesResult = await imdbApi.SearchMovieAsync(movie_name);
				if (moviesResult.Results != null)
					results.AddRange(moviesResult.Results);

				if (bIncludeTVShows) {
					var showsResult = await imdbApi.SearchSeriesAsync(movie_name);
					if (showsResult.Results != null)
						results.AddRange(showsResult.Results);
				}

				if (results.Count == 0)// No search results found
					return null;

				IMDbApiLib.Models.SearchResult match = null;
				try {
					match = results.Single(row => row.Title == movie_name);
				} catch { }

				if (match == null)// No exact match found
					return null;

				return await GetIMDBInfo(match.Id, false);
			} catch (Exception ex) {
				Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
			}

			return null;
		}

		public static async Task<Database.Models.IMDBInfo> GetIMDBInfo(string imdbId, bool checkCache = true) {

			if (checkCache) {
				using (Database.Controller controller = Database.Controller.GetDBController()) {
					// Firstly, search for a matching movie name in the cached data.
					Database.Models.IMDBInfo cachedInfo = null;
					try {
						cachedInfo = controller.IMDBInfo.Single(entry => entry.Id == imdbId);
					} catch { }

					if (cachedInfo != null) {
						//Found an exact match, return the info.
						return cachedInfo;
					}
				}
			}

			IMDbApiLib.Models.TitleData titleData = await imdbApi.TitleAsync(imdbId);

			if (titleData == null)
				return null;

			string fullSizePosterURL = "";
			if (titleData.Posters != null && titleData.Posters.Posters.Count > 0)
				fullSizePosterURL = titleData.Posters.Posters[0].Link;

			Database.Models.IMDBInfo info = null;

			using (Database.Controller controller = Database.Controller.GetDBController()) {
				info = new Database.Models.IMDBInfo {
					Id = titleData.Id,
					Title = titleData.Title,
					CononicalTitle = titleData.FullTitle,
					ReleaseYear = long.Parse(titleData.Year),
					ThumbnailPosterURL = titleData.Image,
					FullSizePosterURL = fullSizePosterURL
				};

				controller.IMDBInfo.Add(info);
				await controller.SaveChangesAsync();
			}

			return info;
		}
	}
}