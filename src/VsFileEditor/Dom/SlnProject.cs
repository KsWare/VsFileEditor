using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsWare.VsFileEditor.Dom;

internal class SlnProject {
	public string Kind { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }
	public string Guid { get; set; }
}

// Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Backup", "Backup\Backup.csproj", "{773E00C8-FBCD-4BF3-95A4-FFAEAFA8279A}"
// EndProject
