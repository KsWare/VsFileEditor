﻿using System.Xml.Linq;

namespace KsWare.VsFileEditor.Dom.Extensions;

public static class XElementExtension {

	public static T As<T>(this XElement element)
		=> (T) Activator.CreateInstance(typeof(T), new object?[] {element});

}