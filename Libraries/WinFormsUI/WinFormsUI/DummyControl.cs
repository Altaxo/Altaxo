using System;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	public class DummyControl : Control
	{
		public DummyControl()
		{
			SetStyle(ControlStyles.Selectable, false);
		}
	}
}
