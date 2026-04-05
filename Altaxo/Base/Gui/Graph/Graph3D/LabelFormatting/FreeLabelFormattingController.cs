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
  /// <summary>
  /// Controls free-form label formatting for 3D graphs.
  /// </summary>
  [ExpectedTypeOfView(typeof(Gdi.LabelFormatting.IFreeLabelFormattingView))]
  [UserControllerForObject(typeof(FreeLabelFormatting), 110)]
  public class FreeLabelFormattingController : MVCANControllerEditOriginalDocBase<FreeLabelFormatting, Gdi.LabelFormatting.IFreeLabelFormattingView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_baseController, () => _baseController = null);
    }

    #region Bindings

    private string _formatString;

    /// <summary>
    /// Gets or sets the format string used for labels.
    /// </summary>
    public string FormatString
    {
      get => _formatString;
      set
      {
        if (!(_formatString == value))
        {
          _formatString = value;
          OnPropertyChanged(nameof(FormatString));
        }
      }
    }

    private MultiLineLabelFormattingBaseController _baseController;

    /// <summary>
    /// Gets or sets the controller for the shared multiline formatting settings.
    /// </summary>
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


    #endregion


    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _baseController = new MultiLineLabelFormattingBaseController() { UseDocumentCopy = UseDocument.Directly };
        _baseController.InitializeDocument(_doc);
        Current.Gui.FindAndAttachControlTo(_baseController);
        FormatString = _doc.FormatString;
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (!_baseController.Apply(disposeController))
        return false;

      _doc.FormatString = FormatString;

      return ApplyEnd(true, disposeController);
    }
  }
}
