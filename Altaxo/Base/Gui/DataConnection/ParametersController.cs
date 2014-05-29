using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public class ParametersController : IMVCAController
	{
		private List<System.Data.OleDb.OleDbParameter> parms;

		public ParametersController(List<System.Data.OleDb.OleDbParameter> parms)
		{
			// TODO: Complete member initialization
			this.parms = parms;
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