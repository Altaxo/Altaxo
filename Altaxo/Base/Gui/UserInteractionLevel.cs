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

namespace Altaxo.Gui
{
  /// <summary>
  /// Determines the degree of interaction with the user during operations.
  /// </summary>
  public enum UserInteractionLevel
  {
    /// <summary>
    /// No interaction with the user. Warnings will be ignored, and errors usually throw an exception.
    /// </summary>
    None = 0,

    /// <summary>
    /// Interaction with the user only if errors occured. Warnings will be ignored.
    /// </summary>
    InteractOnErrors = 1,

    /// <summary>
    /// Interaction with the user only if errors or warnings occured.
    /// </summary>
    InteractOnWarningsAndErrors = 2,

    /// <summary>
    /// Always interaction with the user. This means that in any case for instance a dialog will be presented to the user.
    /// </summary>
    InteractAlways = 3
  }
}
