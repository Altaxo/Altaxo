using System;

namespace Altaxo.Main
{

	public class ParentChangedEventArgs : System.EventArgs
	{
		protected object _oldParent, _newParent;

		public ParentChangedEventArgs(object oldParent, object newParent)
		{
			this._oldParent = oldParent;
			this._newParent = newParent;
		}

		public object NewParent
		{
			get { return this._newParent; }
		}

		public object OldParent
		{
			get { return this._oldParent; }
		}
	}

	public delegate void ParentChangedEventHandler(object sender, ParentChangedEventArgs e);

}
