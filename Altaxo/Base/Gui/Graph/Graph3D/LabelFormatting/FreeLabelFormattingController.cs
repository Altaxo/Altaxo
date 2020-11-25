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
  [ExpectedTypeOfView(typeof(Gdi.LabelFormatting.IFreeLabelFormattingView))]
  [UserControllerForObject(typeof(FreeLabelFormatting), 110)]
  public class FreeLabelFormattingController : MVCANControllerEditOriginalDocBase<FreeLabelFormatting, Gdi.LabelFormatting.IFreeLabelFormattingView>
  {
    private SelectableListNodeList _textBlockAlignmentChoices;

    private MultiLineLabelFormattingBaseController _baseController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _textBlockAlignmentChoices = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _baseController = new MultiLineLabelFormattingBaseController() { UseDocumentCopy = UseDocument.Directly };
        _baseController.InitializeDocument(_doc);
        _textBlockAlignmentChoices = new SelectableListNodeList(_doc.TextBlockAlignment);
      }
      if (_view is not null)
      {
        _baseController.ViewObject = _view.MultiLineLabelFormattingBaseView;
        _view.FormatString = _doc.FormatString;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc.FormatString = _view.FormatString;

      return ApplyEnd(true, disposeController);
    }
  }
}
