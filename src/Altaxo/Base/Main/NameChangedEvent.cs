using System;

namespace Altaxo.Main
{

	public class NameChangedEventArgs : System.EventArgs
	{
		protected string _oldName, _newName;

		public NameChangedEventArgs(string oldName, string newName)
		{
			this._oldName = oldName;
			this._newName = newName;
		}

		public string NewName
		{
			get { return this._newName; }
		}

		public string OldName
		{
			get { return this._oldName; }
		}
	}

	public delegate void NameChangedEventHandler(object sender, NameChangedEventArgs e);

}
