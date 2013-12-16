using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
	public static class InterfaceGuid
	{
		public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

		public static readonly Guid IID_IDispatch = new Guid("{00020400-0000-0000-C000-000000000046}");

		public static readonly Guid IID_IOleObject = new Guid("00000112-0000-0000-C000-000000000046");
	}
}