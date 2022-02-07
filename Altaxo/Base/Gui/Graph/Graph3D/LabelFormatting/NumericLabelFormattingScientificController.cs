#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Graph.Graph3D.LabelFormatting;

namespace Altaxo.Gui.Graph.Graph3D.LabelFormatting
{
  [UserControllerForObject(typeof(NumericLabelFormattingScientific))]
  [ExpectedTypeOfView(typeof(Gdi.LabelFormatting.INumericLabelFormattingScientificView))]
  public class NumericLabelFormattingScientificController : MVCANControllerEditOriginalDocBase<NumericLabelFormattingScientific, Graph.Gdi.LabelFormatting.INumericLabelFormattingScientificView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _showExponentAlways;

    public bool ShowExponentAlways
    {
      get => _showExponentAlways;
      set
      {
        if (!(_showExponentAlways == value))
        {
          _showExponentAlways = value;
          OnPropertyChanged(nameof(ShowExponentAlways));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ShowExponentAlways = _doc.ShowExponentAlways;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.ShowExponentAlways = ShowExponentAlways;

      return ApplyEnd(true, disposeController);
    }
  }
}
