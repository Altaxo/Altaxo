#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Units
{
  /// <summary>
  /// Service that manages a single instance of <see cref="UserDefinedUnitEnvironments"/>. During creation, it reads the user defined unit environments from
  /// the property service. During disposal, it writes the environments back to the property service.
  /// </summary>
  /// <seealso cref="System.IDisposable" />
  public class UserDefinedUnitEnvironmentsService : IDisposable
  {
    /// <summary>
    /// Gets the user defined unit environments.
    /// </summary>
    /// <value>
    /// The user defined unit environments.
    /// </value>
    public UserDefinedUnitEnvironments Environments { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDefinedUnitEnvironmentsService"/> class.
    /// Reads the environments from the property service.
    /// </summary>
    public UserDefinedUnitEnvironmentsService()
    {
      Environments = Current.PropertyService.GetValue(UserDefinedUnitEnvironments.PropertyKeyDefaultInstance,
          Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin);
    }

    /// <summary>
    /// A call to Dispose signals that this service is about to shutdown.
    /// Thus we store all unit environments in the property service
    /// </summary>
    public void Dispose()
    {
      Current.PropertyService.SetValue(UserDefinedUnitEnvironments.PropertyKeyDefaultInstance, Environments);
    }
  }
}
