namespace KsWare.VsFileEditor.Dom.Values;

public readonly struct FileValue {

	private readonly string _path;
	private readonly Func<string> _basePathFnc;

	public FileValue(string path, Func<string> basePathFnc) {
		_path = path;
		_basePathFnc = basePathFnc;
	}

	public bool Exists => File.Exists(FullName);

	public string FullName => Path.GetFullPath(_path,_basePathFnc());

	public override string ToString() => _path;

	public static implicit operator string(FileValue filePath) => filePath._path;
}

