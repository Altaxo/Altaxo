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

namespace Altaxo.Gui.Data
{
  using Altaxo.Data;

  /// <summary>
  /// View contract for decomposing data by column content.
  /// </summary>
  public interface IDecomposeByColumnContentView
  {
    /// <summary>
    /// Sets the view for the data configuration.
    /// </summary>
    /// <param name="control">The view object for the data configuration.</param>
    void SetDataControl(object control);

    /// <summary>
    /// Sets the view for the options configuration.
    /// </summary>
    /// <param name="control">The view object for the options configuration.</param>
    void SetOptionsControl(object control);
  }

  /// <summary>
  /// Controller for <see cref="DecomposeByColumnContentDataAndOptions"/>.
  /// </summary>
  [UserControllerForObject(typeof(DecomposeByColumnContentDataAndOptions))]
  [ExpectedTypeOfView(typeof(IDecomposeByColumnContentView))]
  public class DecomposeByColumnContentController : MVCANControllerEditOriginalDocBase<DecomposeByColumnContentDataAndOptions, IDecomposeByColumnContentView>
  {
    private IMVCANController _dataController;
    private IMVCANController _optionsController;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_dataController, () => _dataController = null);
      yield return new ControllerAndSetNullMethod(_optionsController, () => _optionsController = null);
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _dataController = new DecomposeByColumnContentDataController() { UseDocumentCopy = UseDocument.Directly };
        _dataController.InitializeDocument(_doc.Data);

        _optionsController = new DecomposeByColumnContentOptionsController { UseDocumentCopy = UseDocument.Directly };
        _optionsController.InitializeDocument(_doc.Options);
        Current.Gui.FindAndAttachControlTo(_dataController);
        Current.Gui.FindAndAttachControlTo(_optionsController);
      }
      if (_view is not null)
      {
        _view.SetDataControl(_dataController.ViewObject);
        _view.SetOptionsControl(_optionsController.ViewObject);
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (!_dataController.Apply(disposeController))
        return false;
      if (!_optionsController.Apply(disposeController))
        return false;

      return ApplyEnd(true, disposeController);
    }
  }
}
