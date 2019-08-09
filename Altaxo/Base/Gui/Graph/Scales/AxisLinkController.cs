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

using System;
using Altaxo.Graph.Scales;

namespace Altaxo.Gui.Graph.Scales
{
  #region Interfaces

  public interface IAxisLinkView
  {
    /// <summary>
    /// Initializes the type of the link. If <c>true</c>, the linke is initialized as 1:1 link and all other fields are ignored.
    /// </summary>
    bool IsStraightLink { get; set; }

    /// <summary>
    /// Initializes the content of the OrgA edit box.
    /// </summary>
    double OrgA { get; set; }

    /// <summary>
    /// Initializes the content of the OrgB edit box.
    /// </summary>
    double OrgB { get; set; }

    /// <summary>
    /// Initializes the content of the EndA edit box.
    /// </summary>
    double EndA { get; set; }

    /// <summary>
    /// Initializes the content of the EndB edit box.
    /// </summary>
    double EndB { get; set; }
  }

  #endregion Interfaces

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

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (null != _view)
      {
        _view.OrgA = _doc.OrgA;
        _view.OrgB = _doc.OrgB;
        _view.EndA = _doc.EndA;
        _view.EndB = _doc.EndB;
        _view.IsStraightLink = _doc.IsStraightLink;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_view.IsStraightLink)
      {
        _doc.SetToStraightLink();
      }
      else
      {
        _doc.OrgA = _view.OrgA;
        _doc.OrgB = _view.OrgB;
        _doc.EndA = _view.EndA;
        _doc.EndB = _view.EndB;
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
