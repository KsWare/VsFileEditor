using System.Xml.Linq;

namespace KsWare.VsFileEditor.Dom.Base;

/// <summary>
/// Represents an element in a <see cref="ProjFile"/>
/// </summary>
/// <seealso cref="XElementWrapper" />
public class ProjectElement : XElementWrapper {

	public ProjectElement(XElement element, ProjFile proj) : base(element) {
		Project = proj ?? throw new ArgumentNullException(nameof(proj));
	}

	/// <summary>
	/// Gets or sets the Condition attribute value.
	/// </summary>
	/// <value>The Condition attribute value.</value>
	/// <seealso href="https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-conditions?view=vs-2022"/>
	public string? Condition { get => GetAttribute(); set => SetAttribute(value); }

	/// <summary>
	/// Gets a value indicating whether this element has a <see cref="Condition"/>.
	/// </summary>
	/// <value><c>true</c> if this element has a Condition; otherwise, <c>false</c>.</value>
	public bool HasCondition => !string.IsNullOrWhiteSpace(Condition);

	/// <summary>
	/// Gets the project in which this element is contained.
	/// </summary>
	/// <value>The project.</value>
	public ProjFile Project { get; }
}
