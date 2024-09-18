using System.Xml.Linq;
using KsWare.VsFileEditor.Dom.Base;

namespace KsWare.VsFileEditor.Dom;

public class ProjectReference : XElementWrapper {

	private ProjFile _includeProject;

	public ProjectReference(XElement element, ProjFile proj) 
		: base(element) {
		Project = proj ?? throw new ArgumentNullException(nameof(proj));
	}

	public string? Include { get => GetAttribute(); set => SetAttribute(value); }
	public string? IncludeFullName => Path.GetFullPath(Include, Path.GetDirectoryName(Project.FullName));


	public string? Name => Include != null ? Path.GetFileNameWithoutExtension(Include) : null;

	public string? Condition { get => GetAttribute(); set => SetAttribute(value); }

	public ProjFile Project { get; }

	public ProjFile IncludeProject {
		get {
			if (_includeProject == null)
				_includeProject = ProjFile.LoadCached(IncludeFullName);
			return _includeProject;
		}
	}
}