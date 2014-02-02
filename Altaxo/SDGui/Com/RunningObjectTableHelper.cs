#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	public static class RunningObjectTableHelper
	{
		#region Running Object Table management (ROT)

		public static IRunningObjectTable GetROT()
		{
			IRunningObjectTable rot;
			Int32 hr = Ole32Func.GetRunningObjectTable(0, out rot);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.NOERROR);
			return rot;
		}

		public static void ROTUnregister(ref int cookie)
		{
			// Revoke any existing file moniker. See Brockschmidt, Inside Ole 2nd ed. p988
			IRunningObjectTable rot = GetROT();
			if (0 != cookie)
			{
				rot.Revoke(cookie);
				cookie = 0;
			}
		}

		public static void ROTRegisterAsRunning(IMoniker new_moniker, object o, ref int rot_cookie, Type intf)
		{
			// Revoke any existing file moniker. See Brockschmidt, Inside Ole 2nd ed. p988
			ROTUnregister(ref rot_cookie);

			// Register the moniker in the running object table (ROT).
#if COMLOGGING
			Debug.ReportInfo("Registering {0} in ROT", DataObjectHelper.GetDisplayName(new_moniker));
#endif
			IRunningObjectTable rot = GetROT();

			// This flag solved a terrible problem where Word would stop
			// communicating after its first call to GetObject().
			rot_cookie = rot.Register(1 /*ROTFLAGS_REGISTRATIONKEEPSALIVE*/, o, new_moniker);
		}

		#endregion Running Object Table management (ROT)
	}
}