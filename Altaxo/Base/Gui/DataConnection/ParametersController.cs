using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IParametersView
	{
		void SetParametersSource(List<System.Data.OleDb.OleDbParameter> parms);

		void ReadParameter();
	}

	[ExpectedTypeOfView(typeof(IParametersView))]
	public class ParametersController : IMVCAController
	{
		private IParametersView _view;
		private List<System.Data.OleDb.OleDbParameter> _doc;

		public ParametersController(List<System.Data.OleDb.OleDbParameter> parms)
		{
			// TODO: Complete member initialization
			this._doc = parms;
			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				_view.SetParametersSource(_doc);
			}
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IParametersView;
				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public void Dispose()
		{
			ViewObject = null;
		}

		public bool Apply()
		{
			_view.ReadParameter();
			return true;
		}
	}
}