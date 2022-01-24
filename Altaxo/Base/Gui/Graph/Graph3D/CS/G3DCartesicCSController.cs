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
using Altaxo.Graph.Graph3D.CS;

namespace Altaxo.Gui.Graph.Graph3D.CS
{
  #region Interfaces

  public interface IG3DCartesicCSView
  {
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(G3DCartesicCoordinateSystem), 101)]
  [ExpectedTypeOfView(typeof(IG3DCartesicCSView))]
  public class G3DCartesicCSController : MVCANControllerEditImmutableDocBase<G3DCartesicCoordinateSystem, IG3DCartesicCSView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public G3DCartesicCSController()
    {
    }

    public G3DCartesicCSController(G3DCartesicCoordinateSystem doc)
    {
      InitializeDocument(doc);
    }

    #region Bindings

    private bool _exchangeXY;

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

    private bool _reverseZ;

    public bool ReverseZ
    {
      get => _reverseZ;
      set
      {
        if (!(_reverseZ == value))
        {
          _reverseZ = value;
          OnPropertyChanged(nameof(ReverseZ));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        ExchangeXY = _doc.IsXYInterchanged;
        ReverseX = _doc.IsXReversed;
        ReverseY = _doc.IsYReversed;
        ReverseZ = _doc.IsZReversed;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc = _doc.WithXYInterchangedAndXYZReversed(
      isXYInterchanged: ExchangeXY,
      isXReversed: ReverseX,
      isYReversed: ReverseY,
      isZReversed: ReverseZ);
      return ApplyEnd(true, disposeController);
    }
  }
}
