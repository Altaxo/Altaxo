using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public abstract class TableDataSourceBase :
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>
	{
		#region Change event handling

		/// <summary>
		/// Accumulates the change data of the child. Currently only a flag is set to signal that the table has changed.
		/// </summary>
		/// <param name="sender">The sender of the change notification (currently unused).</param>
		/// <param name="e">The change event args can provide details of the change (currently unused).</param>
		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (e is TableDataSourceChangedEventArgs) // DataSourceChangeEvent has highest priority, if this is set, no other change event is needed
				_accumulatedEventData = e;
			else if (_accumulatedEventData == null)
				_accumulatedEventData = EventArgs.Empty;
		}

		#endregion Change event handling
	}
}