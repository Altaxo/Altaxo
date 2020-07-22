#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;

namespace Altaxo.Gui
{
  /// <summary>
  /// Provides some basic properties common for all controllers.
  /// </summary>
  public class ControllerBase
  {
    protected InvalidOperationException NoDocumentException =>
      new InvalidOperationException($"Controller {GetType()} was not initialized with a document");


    protected InvalidOperationException NoViewException =>
      new InvalidOperationException($"Controller {GetType()} currently has no view.");

    protected InvalidOperationException NotInitializedException =>
      new InvalidOperationException($"Controller {GetType()} is not property initialized.");


    protected void CheckDocumentInitialized<T>([AllowNull][NotNull] ref T doc)
    {
      if (doc is null)
        throw NoDocumentException;
    }
  }
}
