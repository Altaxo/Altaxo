using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public class DataPreviewController : IMVCAController
	{
		private System.Data.DataTable dt;
		private System.Drawing.Size size;

		public DataPreviewController(System.Data.DataTable dt, System.Drawing.Size size)
		{
			// TODO: Complete member initialization
			this.dt = dt;
			this.size = size;
		}

		public DataPreviewController(System.Data.DataTable dt)
		{
			// TODO: Complete member initialization
			this.dt = dt;
		}

		public object ViewObject
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public object ModelObject
		{
			get { throw new NotImplementedException(); }
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool Apply()
		{
			throw new NotImplementedException();
		}
	}
}