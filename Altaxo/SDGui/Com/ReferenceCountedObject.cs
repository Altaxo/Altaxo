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

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Com
{
	/// <summary>
	/// Base class for Com objects. Keeps a reference to the ComManager, which keeps a count of all Com objects that are alive.
	/// </summary>
	[ComVisible(false)]
	public class ReferenceCountedObjectBase
	{
		protected ComManager _comManager;

		public ReferenceCountedObjectBase(ComManager comManager)
		{
			_comManager = comManager;
			_comManager.InterlockedIncrementObjectsCount();

			ComDebug.ReportInfo("{0}.Constructor, NumberOfObjectsInUse={1}", this.GetType().Name, _comManager.ObjectsCount);
		}

		~ReferenceCountedObjectBase()
		{
			_comManager.InterlockedDecrementObjectsCount();

			ComDebug.ReportInfo("{0}.Destructor, NumberOfObjectsInUse={1}", this.GetType().Name, _comManager.ObjectsCount);

			_comManager.AttemptToTerminateServer();
		}
	}
}