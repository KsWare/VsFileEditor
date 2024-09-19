using System.Xml.Linq;
using KsWare.VsFileEditor.Dom.Base;

namespace KsWare.VsFileEditor.Dom;

/// <summary>
/// Represents a &lt;PackageReference&gt; element.
/// </summary>
/// <seealso cref="XElementWrapper" />
/// <seealso href="https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files"/>
public class PackageReference : ProjectElement {

	public PackageReference(XElement element, ProjFile proj) 
		: base(element, proj) {
	}

	public string? Include { get => GetAttribute(); set => SetAttribute(value); }
		
	public string? Version { get => GetAttribute(); set => SetAttribute(value); }

	public string? Condition { get => GetAttribute(); set => SetAttribute(value); }
}