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

#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Graph.Gdi.CS;

namespace Altaxo.Gui.Graph.Gdi.CS
{
  #region Interfaces

  /// <summary>
  /// View contract for the GDI 3D cartesian coordinate system controller.
  /// </summary>
  public interface IG3DCartesicCSView : IDataContextAwareView // view doesn't exist because there is nothing to show
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for <see cref="G3DCartesicCoordinateSystem"/> in the GDI graph layer.
  /// </summary>
  [UserControllerForObject(typeof(G3DCartesicCoordinateSystem), 101)]
  [ExpectedTypeOfView(typeof(IG3DCartesicCSView))]
  public class G3DCartesicCSController : MVCANControllerEditOriginalDocBase<G3DCartesicCoordinateSystem, IG3DCartesicCSView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G3DCartesicCSController"/> class.
    /// </summary>
    public G3DCartesicCSController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G3DCartesicCSController"/> class.
    /// </summary>
    /// <param name="doc">The coordinate system document.</param>
    public G3DCartesicCSController(G2DCartesicCoordinateSystem doc)
    {
      InitializeDocument(doc);
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }
  }
}
