using System;

namespace Altaxo.Main.GUI
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
