using System;

namespace Altaxo.Main
{
	public interface IMVCController
	{
		object ViewObject { get; set; }
	}

	public interface IMVCView
	{
		object ControllerObject { get; set; }
	}
}
