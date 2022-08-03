
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
	}
}