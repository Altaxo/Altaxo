#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
using Altaxo.Science.Spectroscopy.Smoothing;

namespace Altaxo.Gui.Science.Spectroscopy.Smoothing
{
  public interface ISmoothingModifiedSincView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(ISmoothingModifiedSincView))]
  [UserControllerForObject(typeof(SmoothingModifiedSinc))]
  public class SmoothingModifiedSincController : MVCANControllerEditImmutableDocBase<SmoothingModifiedSinc, ISmoothingModifiedSincView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings



    private bool _isMS1Smoothing;

    public bool IsMS1Smoothing
    {
      get => _isMS1Smoothing;
      set
      {
        if (!(_isMS1Smoothing == value))
        {
          _isMS1Smoothing = value;
          OnPropertyChanged(nameof(IsMS1Smoothing));
        }
      }
    }

    private int _Degree;

    public int Degree
    {
      get => _Degree;
      set
      {
        if (!(_Degree == value))
        {
          _Degree = value;
          OnPropertyChanged(nameof(Degree));
        }
      }
    }

    private int _NumberOfPoints;

    public int NumberOfPoints
    {
      get => _NumberOfPoints;
      set
      {
        if (!(_NumberOfPoints == value))
        {
          _NumberOfPoints = value;
          OnPropertyChanged(nameof(NumberOfPoints));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        NumberOfPoints = _doc.NumberOfPoints;
        Degree = _doc.Degree;
        IsMS1Smoothing = _doc.IsMS1Smoothing;
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          NumberOfPoints = NumberOfPoints,
          Degree = Degree,
          IsMS1Smoothing = IsMS1Smoothing
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox($"Error while applying the changes: {ex.Message}", "Input error");
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }

  }
}
