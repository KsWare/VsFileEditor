using System.Xml.Linq;
using KsWare.VsFileEditor.Dom.Base;

namespace KsWare.VsFileEditor.Dom;

public class PackageReference : XElementWrapper {

	public PackageReference(XElement element) : base(element) { }

	public string? Include { get => GetAttribute(); set => SetAttribute(value); }
		
	public string? Version { get => GetAttribute(); set => SetAttribute(value); }

	public string? Condition { get => GetAttribute(); set => SetAttribute(value); }

}