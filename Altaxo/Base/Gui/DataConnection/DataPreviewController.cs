using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public interface IDataPreviewView
	{
		void SetTableSource(System.Data.DataTable table);
	}

	[ExpectedTypeOfView(typeof(IDataPreviewView))]
	public class DataPreviewController : IMVCAController
	{
		private IDataPreviewView _view;
		private System.Data.DataTable dt;
		private System.Drawing.Size size;

		public DataPreviewController(System.Data.DataTable dt, System.Drawing.Size size)
		{
			// TODO: Complete member initialization
			this.dt = dt;
			this.size = size;
			Initialize(true);
		}

		public DataPreviewController(System.Data.DataTable dt)
		{
			// TODO: Complete member initialization
			this.dt = dt;
			Initialize(true);
		}

		private void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				_view.SetTableSource(dt);
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
				_view = value as IDataPreviewView;
				if (null != _view)
				{
					Initialize(false);
				}
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