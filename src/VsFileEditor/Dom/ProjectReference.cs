using System.Xml.Linq;
using KsWare.VsFileEditor.Dom.Base;

namespace KsWare.VsFileEditor.Dom;

/// <summary>
/// Represents a &lt;ProjectReference&gt; element.
/// </summary>
/// <seealso cref="XElementWrapper" />
/// <seealso href="https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items?view=vs-2022#projectreference"/>
public class ProjectReference : ProjectElement {

	private ProjFile? _includeProject;
	
	public ProjectReference(XElement element, ProjFile proj) 
		: base(element, proj) {
	}

	/// <summary>
	/// Gets or sets the Include attribute value.
	/// </summary>
	/// <value>The Include attribute value.</value>
	public string? Include {
		get => GetAttribute();
		set { 
			SetAttribute(value);
			_includeProject = null;
		}
	}

	public string? IncludeˑFullName =>  !string.IsNullOrWhiteSpace(Include) ? Path.GetFullPath(Include, Path.GetDirectoryName(Project.FullName)) : null;
	
	public string? IncludeˑName => Include != null ? Path.GetFileNameWithoutExtension(Include) : null;

	public ProjFile? IncludeˑProject {
		get {
			if (_includeProject == null && !string.IsNullOrWhiteSpace(Include))
				_includeProject = ProjFile.Load(IncludeˑFullName!);
			return _includeProject;
		}
	}

}