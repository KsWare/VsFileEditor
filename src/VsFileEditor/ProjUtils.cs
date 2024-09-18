using System.Text.RegularExpressions;
using System.Xml.Linq;
using KsWare.VsFileEditor.Dom;

namespace KsWare.VsFileEditor;

public static class ProjUtils {

	/// <summary>
	/// Hashset of known project extensions: csproj, vbproj, fsproj
	/// </summary>
	/// <remarks>If necessary you can add more.</remarks>
	public static readonly HashSet<string> ProjectExtensions = new(StringComparer.OrdinalIgnoreCase) {".csproj", ".vbproj", ".fsproj"};
	
	private static string GetAbsolutePath(string csprojPath, string relativePath) {
		if (csprojPath == null) throw new ArgumentNullException(nameof(csprojPath));
		if (relativePath == null) throw new ArgumentNullException(nameof(relativePath));
		var projectDirectory = Path.GetDirectoryName(csprojPath) ?? throw new NotSupportedException();
		return Path.GetFullPath(Path.Combine(projectDirectory, relativePath));
	}

	private static bool PathEquals(string a, string b) {
		return a.Equals(b, StringComparison.OrdinalIgnoreCase);
	}

	//TODO public static string? FindProject(IEnumerable<string> projectRoots, string name, bool mustBeUnique = false) 
	
	public static string? FindProject(string projectRoot, string name, bool mustBeUnique = false) {
		var ext = Path.GetExtension(name);
		name = ProjectExtensions.Contains(ext) ? Path.GetFileNameWithoutExtension(name) : name;
		var search = ProjectExtensions.Contains(ext) ? $"{name}{ext}" : $"{name}.??proj";
		var files =  Directory.EnumerateFiles(projectRoot, search, SearchOption.AllDirectories)
			.Where(f=>string.Equals(Path.GetFileNameWithoutExtension(f),name,StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (files.Length == 0) return null;
		if (files.Length == 1) return files[0];
		if(mustBeUnique) throw new ArgumentException("Ambiguous projects found!");
		return files[0]; // TODO return newest
	}

}