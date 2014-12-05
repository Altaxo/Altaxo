using Altaxo.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public class TableDataSourceBase :
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

		#region DisposableBase

		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="DisposableBase"/> class.
		/// </summary>
		~TableDataSourceBase()
		{
			Dispose(false);
		}

		#endregion DisposableBase

		public override string Name
		{
			get { return this.GetType().Name; }
			set
			{
				throw new InvalidOperationException("The name is fixed and cannot be set");
			}
		}
	}
}