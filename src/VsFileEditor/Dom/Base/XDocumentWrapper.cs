using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace KsWare.VsFileEditor.Dom.Base;

public class XDocumentWrapper {

	private DateTime _lastWrite;

	public XDocumentWrapper(string fullName) {
		Document = XDocument.Load(fullName);
		_lastWrite = File.GetLastWriteTime(fullName);
		FullName = fullName;
		NS = Root.GetDefaultNamespace();
	}

	public XNamespace NS { get;}

	public XDocument Document { get; }

	public XElement Root => Document.Root;

	public string FullName { get; set; }

	public bool IsExternallyChanged => File.GetLastWriteTime(FullName) != _lastWrite;

	public virtual string? ReplaceVars(string? s) {
		return s;
	}

	public virtual string? GetPropertyValue([CallerMemberName] string? propertyName = null) {
		if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
		return Root.Descendants(NS + propertyName).FirstOrDefault()?.Value;
	}

	public static XDocumentWrapper Load(string path) 
		=> new XDocumentWrapper(path);

	public void Save() {
		Document.Save(FullName);
		_lastWrite = File.GetLastWriteTime(FullName);
	}

	public void SaveAs(string newPath) {
		FullName = newPath;
		Save();
	}

	public void Export(string path) {
		Document.Save(path);
	}
}