using EnvDTE;
using EnvDTE80;
using KsWare.VsFileEditor.Internal;

namespace KsWare.VsFileEditor.Dom;

/// <summary>
/// Represents a Visual Studio Solution (.sln) file
/// </summary>
public class SlnFile {

	private DateTime _lastWrite;
	private readonly ObservableCollectionEx<string> _projectFiles = [];
	private readonly ObservableCollectionEx<ProjFile> _projects = [];
	private bool _isLoaded;

	private SlnFile(string path) {
		FullName = Path.GetFullPath(path);
	}

	public string FullName{ get; private set; }

	public Solution? Solution { get; private set; }

	public bool IsLoaded { get; private set; }
	
	public bool IsConnected => Solution != null;

	/// <summary>
	/// Gets all project files from this solution
	/// </summary>
	public IList<string> ProjectFiles {
		get {
			if (!_isLoaded || IsExternallyChanged) Load();
			return _projectFiles;
		}
	}

	/// <summary>
	/// Gets all projects from this solution
	/// </summary>
	public IList<ProjFile> Projects {
		get {
			if (!_isLoaded || IsExternallyChanged) Load();
			return _projects;
		}
	}

	public bool IsExternallyChanged => File.GetLastWriteTime(FullName) != _lastWrite;

	public void Load() {
		UpdateProjectFiles();
		UpdateProjects();
		_lastWrite = File.GetLastWriteTime(FullName);
		_isLoaded = true;
	}

	public void Connect() {
		var dte = DteUtils.FindDevEnv(FullName);
		Solution = dte?.Solution;
	}

	public static SlnFile Load(string path) {
		var file = new SlnFile(path);
		file.Load();
		return file;
	}

	public ProjFile? GetProjectForPackage(string packageName) {
		return Projects.FirstOrDefault(p => string.Equals(p.PackageId, packageName, StringComparison.OrdinalIgnoreCase));
	}

	private void UpdateProjects() {
		var old = _projects.ToDictionary(k => k.FullName, e => e, StringComparer.OrdinalIgnoreCase);
		_projects.ReplaceAll(_projectFiles.Select(f => old.TryGetValue(f, out var v) ? v : ProjFile.Load(f, this)));
	}

	private void UpdateProjectFiles() {
		var files = IsConnected
			? SlnUtils.GetProjectFiles((DTE2) Solution!.DTE)
			: SlnUtils.GetProjectFiles(FullName);
		_projectFiles.ReplaceAll(files);
	}

}
