using System.Text.RegularExpressions;
using System.Xml.Linq;
using KsWare.VsFileEditor.Dom.Base;

namespace KsWare.VsFileEditor.Dom;

/// <summary>
/// Represents a Visual Studio project file.
/// </summary>
/// <seealso cref="XDocumentWrapper" />
public class ProjFile : XDocumentWrapper {

	public static readonly Dictionary<string, ProjFile> Cache = new Dictionary<string, ProjFile>(StringComparer.OrdinalIgnoreCase);

	public ProjFile(string file, SlnFile? slnFile = null) : base(file) {
		Solution = slnFile;
	}

	public SlnFile? Solution { get; set; }

	public ProjectReference[] ProjectReferences {
		get {
			var projectReferences = Root.Descendants(NS + "ProjectReference")
				.Where(x => x.Attribute("Include")?.Value != null);
			return projectReferences.Select(pr=>new ProjectReference(pr,this)).ToArray();
		}
	}

	public PackageReference[] PackageReferences {
		get {
			var projectReferences = Root.Descendants(NS + "PackageReference")
				.Where(x => x.Attribute("Include")?.Value != null);
			return projectReferences.Select(pr=>new PackageReference(pr,this)).ToArray();
		}
	}

	public bool GeneratePackageOnBuild => string.Equals(GetPropertyValue(), "True", StringComparison.OrdinalIgnoreCase);
	public string? PackageId => ReplaceVars(GetPropertyValue());
	public string? AssemblyName => ReplaceVars(GetPropertyValue() ?? Path.GetFileNameWithoutExtension(FullName)); // $(MSBuildProjectName)
	public string? Company => ReplaceVars(GetPropertyValue());

	public override string? ReplaceVars(string? s) {
		// only simple vars are supported. not complex functions like $(MSBuildProjectName.Replace(" ", "_")) 
		if (s == null) return null;
		s = base.ReplaceVars(s)!;
		var vars = Regex.Matches(s, @"(?<=\$\()[\p{L}0-9_.-]+(?=\))", RegexOptions.Compiled|RegexOptions.IgnoreCase);
		foreach (var name in vars.AsQueryable().Select(m=>m.Value)) {
			var v = ReplaceVars(GetPropertyValue(name))
			        ?? name switch {
				        "AssemblyName" => ReplaceVars("$(MSBuildProjectName)"),
				        "MSBuildProjectName" => Path.GetFileNameWithoutExtension(FullName),
				        "Company" => ReplaceVars("$(AssemblyName)"),
						"VersionPrefix" => "1.0.0",
				        _ => null
			        };
			s = s.Replace($"$({name})", $"{v}", StringComparison.OrdinalIgnoreCase);
		}
		return s;
	}

	public void AddProjectReference(string includePath, string? condition = null) {
		var newProjectReference = new XElement(NS + "ProjectReference",
			new XAttribute("Include", includePath));
		newProjectReference.SetAttributeValue("Condition", condition);
		var itemGroup = GetOrCreateItemGroupForChild(new ProjectReference(newProjectReference,this));
		itemGroup.Add(newProjectReference);
	}

	public void AddPackageReference(string packageName,string? version = null, string? condition = null) {
		if (version == null || version.Contains('*')) version = NuGetUtils.GetAvailableVersion(packageName)
			?? throw new ArgumentNullException(nameof(version), "Package for specified version not found.");

		var element = new XElement(NS + "PackageReference",
			new XAttribute("Include", packageName),
			new XAttribute("Version", version)
			);
		element.SetAttributeValue("Condition", condition);
		var itemGroup = GetOrCreateItemGroupForChild(new PackageReference(element,this));
		itemGroup.Add(element);
	}

	public ProjectReference? FindProjectReferenceForPackage(string packageName) {
		return ProjectReferences.FirstOrDefault(pr => ProjUtils.ProjectExtensions.Contains(Path.GetExtension(pr.Include??"")) || pr.IncludeˑProject?.PackageId==packageName);
	}

	private XElement? FindItemGroupWithChild(string childName) {
		return Root.Descendants(NS + "ItemGroup")
			.FirstOrDefault(ig => ig.Descendants(NS + childName).Any());
	}

	private XElement GetOrCreateItemGroupForChild(XElementWrapper element) {
		var itemGroup = FindItemGroupWithChild(element.GetType().Name);
		if (itemGroup == null) {
			itemGroup = new XElement(NS + "ItemGroup");
			Root.Add(itemGroup);
		}
		return itemGroup;
	}
	
	public static ProjFile Load(string path, SlnFile? sln = null) {
		var proj = new ProjFile(path, sln);
		return proj;
	}
	
	public static ProjFile LoadCached(string path) {
		if (!Cache.TryGetValue(path, out var proj)) {
			proj = new ProjFile(path);
			Cache[path] = proj;
		}
		return proj;
	}
}