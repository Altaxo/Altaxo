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

#nullable disable
using System;
using Altaxo.Graph.Scales;

namespace Altaxo.Gui.Graph.Scales
{
  public interface IAxisLinkView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for LinkAxisController.
  /// </summary>
  [ExpectedTypeOfView(typeof(IAxisLinkView))]
  [UserControllerForObject(typeof(LinkedScaleParameters))]
  public class AxisLinkController : MVCANControllerEditOriginalDocBase<LinkedScaleParameters, IAxisLinkView>
  {
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }


    #region Binding

    private bool _isStraightLink;
    /// <summary>
    /// Initializes the type of the link. If <c>true</c>, the linke is initialized as 1:1 link and all other fields are ignored.
    /// </summary>
    public bool IsStraightLink
    {
      get => _isStraightLink;
      set
      {
        if (!(_isStraightLink == value))
        {
          _isStraightLink = value;
          OnPropertyChanged(nameof(IsStraightLink));
          OnPropertyChanged(nameof(IsCustomLink));
        }
      }
    }
    public bool IsCustomLink => !IsStraightLink;


    private double _orgA;

    /// <summary>
    /// Initializes the content of the OrgA edit box.
    /// </summary>
    public double OrgA
    {
      get => _orgA;
      set
      {
        if (!(_orgA == value))
        {
          _orgA = value;
          OnPropertyChanged(nameof(OrgA));
        }
      }
    }


    private double _orgB;

    /// <summary>
    /// Initializes the content of the OrgB edit box.
    /// </summary>
    public double OrgB
    {
      get => _orgB;
      set
      {
        if (!(_orgB == value))
        {
          _orgB = value;
          OnPropertyChanged(nameof(OrgB));
        }
      }
    }


    private double _endA;

    /// <summary>
    /// Initializes the content of the EndA edit box.
    /// </summary>
    public double EndA
    {
      get => _endA;
      set
      {
        if (!(_endA == value))
        {
          _endA = value;
          OnPropertyChanged(nameof(EndA));
        }
      }
    }


    private double _endB;

    /// <summary>
    /// Initializes the content of the EndB edit box.
    /// </summary>
    public double EndB
    {
      get => _endB;
      set
      {
        if (!(_endB == value))
        {
          _endB = value;
          OnPropertyChanged(nameof(EndB));
        }
      }
    }


    #endregion
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        OrgA = _doc.OrgA;
        OrgB = _doc.OrgB;
        EndA = _doc.EndA;
        EndB = _doc.EndB;
        IsStraightLink = _doc.IsStraightLink;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (IsStraightLink)
      {
        _doc.SetToStraightLink();
      }
      else
      {
        _doc.OrgA = OrgA;
        _doc.OrgB = OrgB;
        _doc.EndA = EndA;
        _doc.EndB = EndB;
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
