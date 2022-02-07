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
using Altaxo.Collections;
using Altaxo.Graph.Graph3D.LabelFormatting;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Graph3D.LabelFormatting
{
  [ExpectedTypeOfView(typeof(Graph.Gdi.LabelFormatting.IDateTimeLabelFormattingView))]
  [UserControllerForObject(typeof(DateTimeLabelFormatting), 110)]
  public class DateTimeLabelFormattingController : MVCANControllerEditOriginalDocBase<DateTimeLabelFormatting, Graph.Gdi.LabelFormatting.IDateTimeLabelFormattingView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    #region Bindings

    private bool _showAlternateFormattingOnMidnight;

    public bool ShowAlternateFormattingOnMidnight
    {
      get => _showAlternateFormattingOnMidnight;
      set
      {
        if (!(_showAlternateFormattingOnMidnight == value))
        {
          _showAlternateFormattingOnMidnight = value;
          OnPropertyChanged(nameof(ShowAlternateFormattingOnMidnight));
        }
      }
    }
    private bool _showAlternateFormattingOnNoon;

    public bool ShowAlternateFormattingOnNoon
    {
      get => _showAlternateFormattingOnNoon;
      set
      {
        if (!(_showAlternateFormattingOnNoon == value))
        {
          _showAlternateFormattingOnNoon = value;
          OnPropertyChanged(nameof(ShowAlternateFormattingOnNoon));
        }
      }
    }

    private string _FormattingString;

    public string FormattingString
    {
      get => _FormattingString;
      set
      {
        if (!(_FormattingString == value))
        {
          _FormattingString = value;
          OnPropertyChanged(nameof(FormattingString));
        }
      }
    }
    private string _FormattingStringAlternate;

    public string FormattingStringAlternate
    {
      get => _FormattingStringAlternate;
      set
      {
        if (!(_FormattingStringAlternate == value))
        {
          _FormattingStringAlternate = value;
          OnPropertyChanged(nameof(FormattingStringAlternate));
        }
      }
    }

    private MultiLineLabelFormattingBaseController _baseController;

    public MultiLineLabelFormattingBaseController BaseController
    {
      get => _baseController;
      set
      {
        if (!(_baseController == value))
        {
          _baseController?.Dispose();
          _baseController = value;
          OnPropertyChanged(nameof(BaseController));
        }
      }
    }

    private SingleSelectableListNodeList _timeConversionChoices;

    public SingleSelectableListNodeList TimeConversionChoices
    {
      get => _timeConversionChoices;
      set
      {
        if (!(_timeConversionChoices == value))
        {
          _timeConversionChoices = value;
          OnPropertyChanged(nameof(TimeConversionChoices));
        }
      }
    }

    #endregion


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
        Current.Gui.FindAndAttachControlTo(_baseController);

        TimeConversionChoices = new SingleSelectableListNodeList(_doc.LabelTimeConversion);

        FormattingString = _doc.FormattingString;
        ShowAlternateFormattingOnMidnight = _doc.ShowAlternateFormattingAtMidnight;
        ShowAlternateFormattingOnNoon = _doc.ShowAlternateFormattingAtNoon;
        FormattingStringAlternate = _doc.FormattingStringAlternate;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc.LabelTimeConversion = (DateTimeLabelFormatting.TimeConversion)(_timeConversionChoices.SelectedItem.Tag);
      _doc.FormattingString = FormattingString;
      _doc.ShowAlternateFormattingAtMidnight = ShowAlternateFormattingOnMidnight;
      _doc.ShowAlternateFormattingAtNoon = ShowAlternateFormattingOnNoon;
      _doc.FormattingStringAlternate = FormattingStringAlternate;

      return ApplyEnd(true, disposeController);
    }
  }
}
