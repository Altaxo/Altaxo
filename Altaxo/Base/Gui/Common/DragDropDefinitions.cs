using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common
{
	/// <summary>
	/// Designates the position relative to the target item in a drop operation.
	/// </summary>
	[Flags]
	public enum DragDropRelativeInsertPosition
	{
		/// <summary>
		/// The drop should be inserted before the target item.
		/// </summary>
		BeforeTargetItem = 0,

		/// <summary>
		/// The drop should be inserted just after the target item.
		/// </summary>
		AfterTargetItem = 1,

		/// <summary>
		/// The drop should be inserted in the target item.
		/// </summary>
		TargetItemCenter = 2
	}
}