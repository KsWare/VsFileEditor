using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.VsFileEditor.Dom;

internal class SlnProject {

	public SlnProject(string kind, string name, string path, string guid) {
		Kind = kind;
		Name = name;
		Path = path;
		Guid = guid;
	}

	public string Kind { get; private set; }
	public string Name { get; private set; }
	public string Path { get; private set; }
	public string Guid { get; private set; }
}

// Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Backup", "Backup\Backup.csproj", "{773E00C8-FBCD-4BF3-95A4-FFAEAFA8279A}"
// EndProject
