using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE80;

namespace KsWare.VsFileEditor;
internal static class DteUtils {

	public static DTE2? FindDevEnv(string solutionPath) {
		var rot = GetRunningObjectTable();
		var monikers = GetRunningObjectTableMonikers(rot);

		foreach (var moniker in monikers) {
			string displayName = GetDisplayName(moniker);

			// Nur nach Visual Studio Instanzen suchen
			if (!displayName.StartsWith("!VisualStudio.DTE")) continue;
			var dteInstance = GetObjectFromRot<DTE2>(rot, moniker);
			if (dteInstance == null || dteInstance.Solution == null ||
			    string.IsNullOrEmpty(dteInstance.Solution.FullName)) continue;
			// Prüfen, ob die Solution der aktuellen Instanz die gewünschte ist
			if (!string.Equals(dteInstance.Solution.FullName, solutionPath,
				    StringComparison.OrdinalIgnoreCase)) continue;
			return dteInstance;
		}

		return null;
	}

	private static IRunningObjectTable GetRunningObjectTable() {
		Marshal.ThrowExceptionForHR(GetRunningObjectTable(0, out var rot));
		return rot;
	}

	private static IMoniker[] GetRunningObjectTableMonikers(IRunningObjectTable rot) {
		rot.EnumRunning(out var enumMoniker);
		enumMoniker.Reset();

		var monikers = new System.Collections.Generic.List<IMoniker>();
		var monikerArray = new IMoniker[1];
		while (enumMoniker.Next(1, monikerArray, IntPtr.Zero) == 0) {
			monikers.Add(monikerArray[0]);
		}

		return monikers.ToArray();
	}

	private static string GetDisplayName(IMoniker moniker) {
		CreateBindCtx(0, out var bindCtx);

		moniker.GetDisplayName(bindCtx, null, out var displayName);
		return displayName;
	}

	private static T GetObjectFromRot<T>(IRunningObjectTable rot, IMoniker moniker) where T : class {
		rot.GetObject(moniker, out var comObject);
		return comObject as T;
	}

	[DllImport("ole32.dll")]
	private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

	[DllImport("ole32.dll")]
	private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
}
