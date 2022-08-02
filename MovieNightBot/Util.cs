
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
	}
}