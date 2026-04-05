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
using System.Collections.Generic;
using System.Windows.Input;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Plot.Data
{
  #region Interfaces

  /// <summary>
  /// Provides the view contract for <see cref="FunctionPlotDataController"/>.
  /// </summary>
  public interface IFunctionPlotDataView : IDataContextAwareView
  {
  }

  #endregion Interfaces

  /// <summary>
  /// Controller for editing <see cref="XYFunctionPlotData"/>.
  /// </summary>
  [UserControllerForObject(typeof(XYFunctionPlotData))]
  [ExpectedTypeOfView(typeof(IFunctionPlotDataView))]
  public class FunctionPlotDataController : MVCANControllerEditOriginalDocBase<XYFunctionPlotData, IFunctionPlotDataView>
  {
    private IMVCAController _functionController;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_functionController, () => _functionController = null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionPlotDataController"/> class.
    /// </summary>
    public FunctionPlotDataController()
    {
      CmdEdit = new RelayCommand(EhView_EditText);
    }

    #region Bindings

    /// <summary>
    /// Gets the command that opens the function editor.
    /// </summary>
    public ICommand CmdEdit { get; }

    private string _functionText;

    /// <summary>
    /// Gets or sets the function text.
    /// </summary>
    public string FunctionText
    {
      get => _functionText;
      set
      {
        if (!(_functionText == value))
        {
          _functionText = value;
          OnPropertyChanged(nameof(FunctionText));
        }
      }
    }

    private bool _functionTextIsEditable;

    /// <summary>
    /// Gets or sets a value indicating whether the function text can be edited.
    /// </summary>
    public bool FunctionTextIsEditable
    {
      get => _functionTextIsEditable;
      set
      {
        if (!(_functionTextIsEditable == value))
        {
          _functionTextIsEditable = value;
          OnPropertyChanged(nameof(FunctionTextIsEditable));
        }
      }
    }

    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // try to find a controller for the underlying function
        _functionController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.Function }, typeof(IMVCAController), UseDocument.Directly);
      }
      if (_view is not null)
      {
        bool editable = _functionController is not null;
        string text;
        if (_doc.Function is Altaxo.Scripting.IScriptText)
          text = ((Altaxo.Scripting.IScriptText)_doc.Function).ScriptText;
        else
          text = _doc.Function.ToString();

        FunctionText = text;
        FunctionTextIsEditable = editable;
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    private void EhView_EditText()
    {
      if (Current.Gui.ShowDialog(_functionController, "Edit script"))
      {
        _doc.Function = (Altaxo.Calc.IScalarFunctionDD)_functionController.ModelObject;
        Initialize(false);
      }
    }
  }
}
