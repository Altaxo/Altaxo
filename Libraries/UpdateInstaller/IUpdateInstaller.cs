#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.AutoUpdates
{
  public interface IUpdateInstaller
  {
    /// <summary>Runs the installer</summary>
    /// <param name="ReportProgress">Used to report the installation progress by the installer routine.
    /// Arguments are: 1st argument is the progress in percent and the 2nd argument is the progress message.
    /// If this function returns true, the installer routine must throw a <see cref="System.Threading.ThreadInterruptedException"/>.</param>
    void Run(Func<double, string, MessageKind, bool> ReportProgress);
  }
}
