using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.DataConnection
{
	public class QueryPropertiesController : IMVCAController
	{
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

		public Altaxo.DataConnection.QueryBuilder QueryBuilder { get; set; }
	}
}