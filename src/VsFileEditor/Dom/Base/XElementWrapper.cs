using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace KsWare.VsFileEditor.Dom.Base;

public class XElementWrapper {

	public XElementWrapper(XElement element) {
		Element = element;
	}

	public XElement Element { get; }

	public string? GetAttribute([CallerMemberName] string? name = null) {
		return Element.Attribute(name)?.Value;
	}

	public void SetAttribute(string? value, [CallerMemberName] string? name = null) {
		Element.SetAttributeValue(name, value);
	}
}