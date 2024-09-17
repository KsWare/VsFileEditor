using EnvDTE80;
using JetBrains.Annotations;

namespace KsWare.VsFileEditor;

internal class Program {

	private static DTE2? dte;

	// D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation.Themes.Aero2Dark\KsWare.Presentation.Themes.Aero2Dark.sln
	// D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation.Resources.Core\src\KsWare.Presentation.Resources.Core\KsWare.Presentation.Resources.Core.csproj
	// KsWare.Presentation.Resources.Core

	public static void Main(string[] args) {
		try {
			Console.WriteLine(string.Join(" ", args));
			dte = DteUtils.FindDevEnv(args[0]);
			if (dte == null) Error("DTE not found");
			//SlnUtils.CreateSolutionFolder(dte!, "Foo/Bar");
			var projectRoot = @"D:\Develop\Extern\GitHub.KsWare";
			var csproj = @"D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation.Themes.Aero2Dark\src\KsWare.Presentation.Themes.Aero2Dark\KsWare.Presentation.Themes.Aero2Dark.csproj";
			var csref = @"D:\Develop\Extern\GitHub.KsWare\KsWare.Presentation.Resources.Core\src\KsWare.Presentation.Resources.Core\KsWare.Presentation.Resources.Core.csproj";
//			ProjUtils.AddConditions(csproj);
			var proj =  ProjUtils.FindProject(projectRoot, "KsWare.Presentation.Resources.Core");
			Console.WriteLine($"found {proj}");
		}
		catch (Exception ex) {
			Console.Error.WriteLine(ex);
			Environment.Exit(1);
		}
	}

	[ContractAnnotation("=> halt")]
	private static void Error(string msg) {
		Console.Error.WriteLine(msg);
		Environment.Exit(1);
	}

}
