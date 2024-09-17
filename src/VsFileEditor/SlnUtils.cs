using EnvDTE;
using EnvDTE80;

namespace KsWare.VsFileEditor;

public class SlnUtils {

	public static void AddProject(DTE2 dte, string projectFilePath, string? solutionFolder = null) {
		var solution = dte.Solution;
		if (dte.Solution == null) return;
		var f = GetOrCreateSolutionFolder(solution, solutionFolder);
		if (f == null) solution.AddFromFile(projectFilePath);
		else f.AddFromFile(projectFilePath);
	}

	public static void CreateSolutionFolder(DTE2 dte, string folderPath)
		=> GetOrCreateSolutionFolder(dte.Solution, folderPath);

	private static SolutionFolder? GetOrCreateSolutionFolder(Solution solution, string folderPath) {
		var folders = (folderPath ?? "").Trim('/').Trim('\\').Split('/', '\\');
		var parentFolder = (Project?)null;
		foreach (var folderName in folders) {
			if (parentFolder == null) {
				var rootFolder = solution.Projects.OfType<Project>()
					.FirstOrDefault(p => p.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder && p.Name == folderName);
				// ReSharper disable once SuspiciousTypeConversion.Global
				parentFolder = rootFolder ?? ((Solution2)solution).AddSolutionFolder(folderName);
			}
			else {
				var solutionFolder = (SolutionFolder)parentFolder.Object;
				var subFolder = parentFolder.ProjectItems.OfType<ProjectItem>()
					.FirstOrDefault(item => item.SubProject != null && item.SubProject.Name == folderName)?.SubProject;
				parentFolder = subFolder ?? solutionFolder.AddSolutionFolder(folderName);
			}
		}
		return (SolutionFolder?)parentFolder?.Object;
	}

	private const string vsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}"; // obsolete?
	private const string vsProjectKindVirtualFolder = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

	private static readonly HashSet<string> FolderProjectKinds = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
		vsProjectKindSolutionFolder,
		vsProjectKindVirtualFolder
	};

	public static string[] GetProjectFiles(DTE2 dte) {
		var projects = dte.Solution.Projects.OfType<Project>()
			.Where(p => !FolderProjectKinds.Contains(p.Kind))
			.Select(p=>Path.GetFullPath(p.FullName));
		return projects.ToArray();
	}

	public static string[] GetProjectFiles(string slnFile) {
		if (slnFile == null) throw new ArgumentNullException(nameof(slnFile));

		var basePath = Path.GetDirectoryName(slnFile);
		var lines = File.ReadAllLines(slnFile);
		var projects = new List<string>();

		foreach (var line in lines) {
			if (!line.StartsWith("Project(")) continue;
			var parts = line.Split(',');
			if (parts.Length <= 1) continue;
			var projectPath = parts[1].Trim().Trim('"');
			if (!ProjUtils.ProjectExtensions.Contains(Path.GetExtension(projectPath))) continue;
			var absolutePath = Path.GetFullPath(projectPath,basePath!);
			projects.Add(absolutePath);
		}

		return projects.ToArray();
	}

	public static string? GetRepositoryFolder(DTE2 dte) {
		var d = Path.GetDirectoryName(dte.Solution.FullName);
		while (d!=null) {
			if (Directory.Exists(Path.Combine(d,".git"))) return d;
			d = Path.GetDirectoryName(d);
		}
		return null;
	}
}
