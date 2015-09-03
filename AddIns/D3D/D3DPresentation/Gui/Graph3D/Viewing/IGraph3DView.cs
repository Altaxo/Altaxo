using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	public interface IGraph3DView
	{
		object GuiInitiallyFocusedElement { get; }
	}
}