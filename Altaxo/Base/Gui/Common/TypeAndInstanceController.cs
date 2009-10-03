using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common
{
	using Altaxo.Collections;

	#region Interfaces

	public interface ITypeAndInstanceView
	{
		string TypeLabel { set; }
		void InitializeTypeNames(SelectableListNodeList list);
		void SetInstanceControl(object instanceControl);

		event EventHandler TypeChoiceChanged;
	}

	#endregion
}
