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
  /// <summary>
  /// Controller for editing <see cref="G2DPolarCoordinateSystem"/>.
  /// </summary>
  [UserControllerForObject(typeof(G2DPolarCoordinateSystem), 101)]
  [ExpectedTypeOfView(typeof(IG2DCartesicCSView))]
  public class G2DPolarCSController : MVCANControllerEditOriginalDocBase<G2DPolarCoordinateSystem, IG2DCartesicCSView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G2DPolarCSController"/> class.
    /// </summary>
    public G2DPolarCSController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="G2DPolarCSController"/> class.
    /// </summary>
    /// <param name="doc">The coordinate system document.</param>
    public G2DPolarCSController(G2DPolarCoordinateSystem doc)
    {
      InitializeDocument(doc);
    }

    #region Bindings

    private bool _exchangeXY;

    /// <summary>
    /// Gets or sets a value indicating whether X and Y are exchanged.
    /// </summary>
    public bool ExchangeXY
    {
      get => _exchangeXY;
      set
      {
        if (!(_exchangeXY == value))
        {
          _exchangeXY = value;
          OnPropertyChanged(nameof(ExchangeXY));
        }
      }
    }
    private bool _reverseX;

    /// <summary>
    /// Gets or sets a value indicating whether the X direction is reversed.
    /// </summary>
    public bool ReverseX
    {
      get => _reverseX;
      set
      {
        if (!(_reverseX == value))
        {
          _reverseX = value;
          OnPropertyChanged(nameof(ReverseX));
        }
      }
    }
    private bool _reverseY;

    /// <summary>
    /// Gets or sets a value indicating whether the Y direction is reversed.
    /// </summary>
    public bool ReverseY
    {
      get => _reverseY;
      set
      {
        if (!(_reverseY == value))
        {
          _reverseY = value;
          OnPropertyChanged(nameof(ReverseY));
        }
      }
    }

    #endregion


    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ExchangeXY = _doc.IsXYInterchanged;
        ReverseX = _doc.IsXReverse;
        ReverseY = _doc.IsYReverse;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc.IsXYInterchanged = ExchangeXY;
      _doc.IsXReverse = ReverseX;
      _doc.IsYReverse = ReverseY;
      return ApplyEnd(true, disposeController);
    }
  }
}
