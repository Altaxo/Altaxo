#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.PeakEnhancement;

namespace Altaxo.Gui.Science.Spectroscopy.PeakEnhancement;
public interface IPeakEnhancementSNIPView : IDataContextAwareView
{
}

[UserControllerForObject(typeof(PeakEnhancementSNIP), 100)]
[ExpectedTypeOfView(typeof(IPeakEnhancementSNIPView))]
public class PeakEnhancementSNIPController : MVCANControllerEditImmutableDocBase<PeakEnhancementSNIP, IPeakEnhancementSNIPView>
{
  public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
  {
    yield break;
  }

  #region Bindings

  private bool _isHalfWidthManual;

  public bool IsHalfWidthManual
  {
    get => _isHalfWidthManual;
    set
    {
      if (!(_isHalfWidthManual == value))
      {
        _isHalfWidthManual = value;
        OnPropertyChanged(nameof(IsHalfWidthManual));
      }
    }
  }


  private double _halfWidth;

  public double HalfWidth
  {
    get => _halfWidth;
    set
    {
      if (!(_halfWidth == value))
      {
        _halfWidth = value;
        OnPropertyChanged(nameof(HalfWidth));
      }
    }
  }

  private bool _isHalfWidthInXUnits;

  public bool IsHalfWidthInXUnits
  {
    get => _isHalfWidthInXUnits;
    set
    {
      if (!(_isHalfWidthInXUnits == value))
      {
        _isHalfWidthInXUnits = value;
        OnPropertyChanged(nameof(IsHalfWidthInXUnits));
      }
    }
  }


  private int _numberOfApplications;

  public int NumberOfApplications
  {
    get => _numberOfApplications;
    set
    {
      if (!(_numberOfApplications == value))
      {
        _numberOfApplications = value;
        OnPropertyChanged(nameof(NumberOfApplications));
      }
    }
  }




  #endregion

  protected override void Initialize(bool initData)
  {
    base.Initialize(initData);

    if (initData)
    {
      IsHalfWidthManual = _doc.HalfWidth is not null;
      HalfWidth = _doc.HalfWidth ?? PeakEnhancementSNIP.DefaultHalfWidthInPoints;
      IsHalfWidthInXUnits = _doc.IsHalfWidthInXUnits;
      NumberOfApplications = _doc.NumberOfApplications;
    }
  }

  public override bool Apply(bool disposeController)
  {
    _doc = _doc with
    {
      HalfWidth = IsHalfWidthManual ? HalfWidth : null,
      IsHalfWidthInXUnits = IsHalfWidthInXUnits,
      NumberOfApplications = NumberOfApplications
    };

    return ApplyEnd(true, disposeController);
  }
}

