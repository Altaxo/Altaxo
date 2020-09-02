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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D.LabelFormatting;

namespace Altaxo.Gui.Graph.Graph3D.LabelFormatting
{
  [ExpectedTypeOfView(typeof(Graph.Gdi.LabelFormatting.IDateTimeLabelFormattingView))]
  [UserControllerForObject(typeof(DateTimeLabelFormatting), 110)]
  public class DateTimeLabelFormattingController : MVCANControllerEditOriginalDocBase<DateTimeLabelFormatting, Graph.Gdi.LabelFormatting.IDateTimeLabelFormattingView>
  {
    private SelectableListNodeList _timeConversionChoices;
    private MultiLineLabelFormattingBaseController _baseController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _timeConversionChoices = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _baseController = new MultiLineLabelFormattingBaseController() { UseDocumentCopy = UseDocument.Directly };
        _baseController.InitializeDocument(_doc);
        _timeConversionChoices = new SelectableListNodeList(_doc.LabelTimeConversion);
      }

      if (_view is not null)
      {
        _baseController.ViewObject = _view.MultiLineLabelFormattingBaseView;
        _view.InitializeTimeConversion(_timeConversionChoices);
        _view.FormattingString = _doc.FormattingString;
        _view.ShowAlternateFormattingOnMidnight = _doc.ShowAlternateFormattingAtMidnight;
        _view.ShowAlternateFormattingOnNoon = _doc.ShowAlternateFormattingAtNoon;
        _view.FormattingStringAlternate = _doc.FormattingStringAlternate;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc.LabelTimeConversion = (DateTimeLabelFormatting.TimeConversion)_timeConversionChoices.FirstSelectedNode.Tag;

      _doc.FormattingString = _view.FormattingString;
      _doc.ShowAlternateFormattingAtMidnight = _view.ShowAlternateFormattingOnMidnight;
      _doc.ShowAlternateFormattingAtNoon = _view.ShowAlternateFormattingOnNoon;
      _doc.FormattingStringAlternate = _view.FormattingStringAlternate;

      return ApplyEnd(true, disposeController);
    }
  }
}
