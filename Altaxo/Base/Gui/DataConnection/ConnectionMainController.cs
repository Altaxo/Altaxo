using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IConnectionMainView
	{
	}

	[ExpectedTypeOfView(typeof(IConnectionMainView))]
	public class ConnectionMainController : IMVCAController
	{
		private IConnectionMainView _view;

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IConnectionMainView;
			}
		}

		public object ModelObject
		{
			get { return null; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			return true;
		}
	}
}