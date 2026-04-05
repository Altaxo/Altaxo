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

#nullable disable
using System.Collections.Generic;
using Altaxo.Data;
using Altaxo.Science.Thermorheology.MasterCurves;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// View interface for <see cref="MasterCurveCreationMainController"/>.
  /// </summary>
  public interface IMasterCurveCreationMainView
  {
    /// <summary>
    /// Initializes the data tab.
    /// </summary>
    void InitializeDataTab(object guiControl);

    /// <summary>
    /// Initializes the edit tab.
    /// </summary>
    void InitializeEditTab(object guiControl);
  }

  /// <summary>
  /// Main controller hosting the data and edit tabs for master curve creation.
  /// </summary>
  [UserControllerForObject(typeof(MasterCurveCreationOptions))]
  [ExpectedTypeOfView(typeof(IMasterCurveCreationMainView))]
  internal class MasterCurveCreationMainController : IMVCANController
  {
    /// <summary>
    /// The edited document.
    /// </summary>
    private MasterCurveCreationOptions _doc;

    /// <summary>
    /// The attached view.
    /// </summary>
    private IMasterCurveCreationMainView _view;

    /// <summary>
    /// The controller for the data tab.
    /// </summary>
    private IMVCANController _dataController;

    private void Initialize(bool initData)
    {
      if (initData)
      {
        _dataController = new MasterCurveCreationDataController();
        var data = new List<List<DoubleColumn>>();
        _dataController.InitializeDocument(data);
        Current.Gui.FindAndAttachControlTo(_dataController);
      }

      if (_view is not null)
      {
        _view.InitializeDataTab(_dataController.ViewObject);
      }
    }

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || 0 == args.Length || !(args[0] is MasterCurveCreationOptions))
        return false;

      _doc = args[0] as MasterCurveCreationOptions;
      Initialize(true);
      return true;
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    /// <inheritdoc/>
    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        _view = value as IMasterCurveCreationMainView;

        if (_view is not null)
        {
          Initialize(false);
        }
      }
    }

    /// <inheritdoc/>
    public object ModelObject
    {
      get { return _doc; }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      return true;
    }

    /// <summary>
    /// Tries to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>true</c> if the revert operation was successful; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }
  }
}
