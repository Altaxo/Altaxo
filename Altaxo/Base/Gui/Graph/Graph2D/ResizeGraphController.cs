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
using System.Text;
using Altaxo.Collections;
using Altaxo.Geometry;
using Altaxo.Graph.Graph2D;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph2D
{
  public interface IResizeGraphView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IResizeGraphView))]
  [UserControllerForObject(typeof(ResizeGraphOptions))]
  public class ResizeGraphController : MVCANControllerEditOriginalDocBase<ResizeGraphOptions, IResizeGraphView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private string _reportOfOldValues;

    public string ReportOfOldValues
    {
      get => _reportOfOldValues;
      set
      {
        if (!(_reportOfOldValues == value))
        {
          _reportOfOldValues = value;
          OnPropertyChanged(nameof(ReportOfOldValues));
        }
      }
    }

    private string _reportOfDerivedValues;

    public string ReportOfDerivedValues
    {
      get => _reportOfDerivedValues;
      set
      {
        if (!(_reportOfDerivedValues == value))
        {
          _reportOfDerivedValues = value;
          OnPropertyChanged(nameof(ReportOfDerivedValues));
        }
      }
    }

    private bool _isNewRootLayerSizeChosen;

    public bool IsNewRootLayerSizeChosen
    {
      get => _isNewRootLayerSizeChosen;
      set
      {
        if (!(_isNewRootLayerSizeChosen == value))
        {
          _isNewRootLayerSizeChosen = value;
          OnPropertyChanged(nameof(IsNewRootLayerSizeChosen));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment RootLayerSizeEnvironment => SizeEnvironment.Instance;

    private DimensionfulQuantity _rootLayerSizeX;

    public DimensionfulQuantity RootLayerSizeX
    {
      get => _rootLayerSizeX;
      set
      {
        if (!(_rootLayerSizeX == value))
        {
          _rootLayerSizeX = value;
          OnPropertyChanged(nameof(RootLayerSizeX));
        }
      }
    }
    private DimensionfulQuantity _rootLayerSizeY;

    public DimensionfulQuantity RootLayerSizeY
    {
      get => _rootLayerSizeY;
      set
      {
        if (!(_rootLayerSizeY == value))
        {
          _rootLayerSizeY = value;
          OnPropertyChanged(nameof(RootLayerSizeY));
        }
      }
    }

    private bool _isNewStandardFontFamilyChosen;

    public bool IsNewStandardFontFamilyChosen
    {
      get => _isNewStandardFontFamilyChosen;
      set
      {
        if (!(_isNewStandardFontFamilyChosen == value))
        {
          _isNewStandardFontFamilyChosen = value;
          OnPropertyChanged(nameof(IsNewStandardFontFamilyChosen));
        }
      }
    }

    private string _standardFontFamilyName;

    public string StandardFontFamilyName
    {
      get => _standardFontFamilyName;
      set
      {
        if (!(_standardFontFamilyName == value))
        {
          _standardFontFamilyName = value;
          OnPropertyChanged(nameof(StandardFontFamilyName));
          EhFontChanged();
        }
      }
    }

    private bool _isResetAllFontsToStandardFontFamilyChosen;

    public bool IsResetAllFontsToStandardFontFamilyChosen
    {
      get => _isResetAllFontsToStandardFontFamilyChosen;
      set
      {
        if (!(_isResetAllFontsToStandardFontFamilyChosen == value))
        {
          _isResetAllFontsToStandardFontFamilyChosen = value;
          OnPropertyChanged(nameof(IsResetAllFontsToStandardFontFamilyChosen));
        }
      }
    }

    private bool _isNewStandardFontSizeChosen;

    public bool IsNewStandardFontSizeChosen
    {
      get => _isNewStandardFontSizeChosen;
      set
      {
        if (!(_isNewStandardFontSizeChosen == value))
        {
          _isNewStandardFontSizeChosen = value;
          OnPropertyChanged(nameof(IsNewStandardFontSizeChosen));
        }
      }
    }

    private double _standardFontSize;

    public double StandardFontSize
    {
      get => _standardFontSize;
      set
      {
        if (!(_standardFontSize == value))
        {
          _standardFontSize = value;
          OnPropertyChanged(nameof(StandardFontSize));
        }
      }
    }

    private ItemsController<ResizeGraphOptions.ScalarSizeActions> _actionForFontSize;

    public ItemsController<ResizeGraphOptions.ScalarSizeActions> ActionForFontSize
    {
      get => _actionForFontSize;
      set
      {
        if (!(_actionForFontSize == value))
        {
          _actionForFontSize = value;
          OnPropertyChanged(nameof(ActionForFontSize));
        }
      }
    }

    private ItemsController<ResizeGraphOptions.ScalarSizeActions> _actionForSymbolSize;

    public ItemsController<ResizeGraphOptions.ScalarSizeActions> ActionForSymbolSize
    {
      get => _actionForSymbolSize;
      set
      {
        if (!(_actionForSymbolSize == value))
        {
          _actionForSymbolSize = value;
          OnPropertyChanged(nameof(ActionForSymbolSize));
        }
      }
    }

    private ItemsController<ResizeGraphOptions.ScalarSizeActions> _actionForLineThickness;

    public ItemsController<ResizeGraphOptions.ScalarSizeActions> ActionForLineThickness
    {
      get => _actionForLineThickness;
      set
      {
        if (!(_actionForLineThickness == value))
        {
          _actionForLineThickness = value;
          OnPropertyChanged(nameof(ActionForLineThickness));
        }
      }
    }

    private bool _isUserDefinedLineThicknessChosen;

    public bool IsUserDefinedLineThicknessChosen
    {
      get => _isUserDefinedLineThicknessChosen;
      set
      {
        if (!(_isUserDefinedLineThicknessChosen == value))
        {
          _isUserDefinedLineThicknessChosen = value;
          OnPropertyChanged(nameof(IsUserDefinedLineThicknessChosen));
        }
      }
    }

    private double _userDefinedLineThicknessValue;

    public double UserDefinedLineThicknessValue
    {
      get => _userDefinedLineThicknessValue;
      set
      {
        if (!(_userDefinedLineThicknessValue == value))
        {
          _userDefinedLineThicknessValue = value;
          OnPropertyChanged(nameof(UserDefinedLineThicknessValue));
        }
      }
    }


    private ItemsController<ResizeGraphOptions.ScalarSizeActions> _actionForTickLength;

    public ItemsController<ResizeGraphOptions.ScalarSizeActions> ActionForTickLength
    {
      get => _actionForTickLength;
      set
      {
        if (!(_actionForTickLength == value))
        {
          _actionForTickLength = value;
          OnPropertyChanged(nameof(ActionForTickLength));
        }
      }
    }


    private bool _isUserDefinedMajorTickLengthChosen;

    public bool IsUserDefinedMajorTickLengthChosen
    {
      get => _isUserDefinedMajorTickLengthChosen;
      set
      {
        if (!(_isUserDefinedMajorTickLengthChosen == value))
        {
          _isUserDefinedMajorTickLengthChosen = value;
          OnPropertyChanged(nameof(IsUserDefinedMajorTickLengthChosen));
        }
      }
    }

    private double _userDefinedMajorTickLength;

    public double UserDefinedMajorTickLength
    {
      get => _userDefinedMajorTickLength;
      set
      {
        if (!(_userDefinedMajorTickLength == value))
        {
          _userDefinedMajorTickLength = value;
          OnPropertyChanged(nameof(UserDefinedMajorTickLength));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ActionForFontSize = new ItemsController<ResizeGraphOptions.ScalarSizeActions>(new SelectableListNodeList(_doc.ActionForFontSize));
        ActionForSymbolSize = new ItemsController<ResizeGraphOptions.ScalarSizeActions>(new SelectableListNodeList(_doc.ActionForSymbolSize));
        ActionForLineThickness = new ItemsController<ResizeGraphOptions.ScalarSizeActions>(new SelectableListNodeList(_doc.ActionForLineThickness));
        ActionForTickLength = new ItemsController<ResizeGraphOptions.ScalarSizeActions>(new SelectableListNodeList(_doc.ActionForTickLength));

        var cult = Altaxo.Settings.GuiCulture.Instance;
        var stb = new StringBuilder();
        stb.AppendFormat(cult, "Root layer size: ({0} pt x {1} pt) = ({2} mm x {3} mm)", _doc.OldRootLayerSize.X, _doc.OldRootLayerSize.Y, _doc.OldRootLayerSize.X * 25.4 / 72, _doc.OldRootLayerSize.Y * 25.4 / 72);
        stb.AppendLine();
        stb.AppendFormat(cult, "Standard font family: {0}", string.IsNullOrEmpty(_doc.OldStandardFontFamily) ? "<not set>" : _doc.OldStandardFontFamily);
        stb.AppendLine();
        stb.AppendFormat(cult, "Standard font size: {0}", _doc.OldStandardFontSize.HasValue ? _doc.OldStandardFontSize.Value.ToString(cult) + " pt" : "<not set>");
        stb.AppendLine();
        stb.AppendFormat(cult, "Standard line width: {0}", _doc.OldLineThickness.HasValue ? _doc.OldLineThickness.Value.ToString(cult) + " pt" : "<not set>");
        ReportOfOldValues = stb.ToString();

        RootLayerSizeX = new DimensionfulQuantity(_doc.OldRootLayerSize.X, Altaxo.Units.Length.Point.Instance).AsQuantityIn(RootLayerSizeEnvironment.DefaultUnit);
        RootLayerSizeY = new DimensionfulQuantity(_doc.OldRootLayerSize.Y, Altaxo.Units.Length.Point.Instance).AsQuantityIn(RootLayerSizeEnvironment.DefaultUnit);

        StandardFontFamilyName = _doc.OldStandardFontFamily;
        if (_doc.OldStandardFontSize.HasValue)
          StandardFontSize = _doc.OldStandardFontSize.Value;
        if (_doc.OldMajorTickLength.HasValue)
          UserDefinedMajorTickLength = _doc.OldMajorTickLength.Value;

        if (_doc.OldLineThickness.HasValue)
          UserDefinedLineThicknessValue = _doc.OldLineThickness.Value;
      }
    }

    private void EhFontChanged()
    {
      var fontFamily = IsNewStandardFontFamilyChosen ? StandardFontFamilyName : _doc.OldStandardFontFamily;
      var fontSize = IsNewStandardFontSizeChosen ? StandardFontSize : _doc.OldStandardFontSize ?? 12;
      var font = Altaxo.Graph.Gdi.GdiFontManager.GetFontX(fontFamily, fontSize, Altaxo.Drawing.FontXStyle.Regular);

      var bag = new Altaxo.Main.Properties.PropertyBag();
      bag.SetValue(Altaxo.Graph.Gdi.GraphDocument.PropertyKeyDefaultFont, font);
      var newLineWidth = Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(bag);
      var newMajorTickLength = Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(bag);

      var cult = Altaxo.Settings.GuiCulture.Instance;
      var stb = new StringBuilder();
      stb.AppendFormat(cult, "Standard font family: {0}", fontFamily);
      stb.AppendLine();
      stb.AppendFormat(cult, "Standard font size: {0} pt", fontSize);
      stb.AppendLine();
      stb.AppendFormat(cult, "Derived line width: {0} pt", newLineWidth);
      stb.AppendLine();
      stb.AppendFormat(cult, "Derived major tick length: {0} pt", newMajorTickLength);
      ReportOfDerivedValues = stb.ToString();

      if (!IsUserDefinedLineThicknessChosen)
        UserDefinedLineThicknessValue = newLineWidth;

      if (!IsUserDefinedMajorTickLengthChosen)
        UserDefinedMajorTickLength = newMajorTickLength;
    }

    public override bool Apply(bool disposeController)
    {
      if (IsNewRootLayerSizeChosen)
        _doc.NewRootLayerSize = new PointD2D(RootLayerSizeX.AsValueIn(Altaxo.Units.Length.Point.Instance), RootLayerSizeX.AsValueIn(Altaxo.Units.Length.Point.Instance));
      else
        _doc.NewRootLayerSize = null;

      if (IsNewStandardFontFamilyChosen)
        _doc.NewStandardFontFamily = StandardFontFamilyName;
      else
        _doc.NewStandardFontFamily = null;

      _doc.OptionResetAllFontsToStandardFont = IsResetAllFontsToStandardFontFamilyChosen;

      if (IsNewStandardFontSizeChosen)
        _doc.NewStandardFontSize = StandardFontSize;
      else
        _doc.NewStandardFontSize = null;

      _doc.ActionForFontSize = ActionForFontSize.SelectedValue;

      _doc.ActionForSymbolSize = ActionForSymbolSize.SelectedValue;

      _doc.ActionForLineThickness = ActionForLineThickness.SelectedValue;

      if (IsUserDefinedLineThicknessChosen)
        _doc.UserDefinedLineThickness = UserDefinedLineThicknessValue;
      else
        _doc.UserDefinedLineThickness = null;

      _doc.ActionForTickLength = ActionForTickLength.SelectedValue;

      if (IsUserDefinedMajorTickLengthChosen)
        _doc.UserDefinedMajorTickLength = UserDefinedMajorTickLength;
      else
        _doc.UserDefinedMajorTickLength = null;

      return ApplyEnd(true, disposeController);
    }

    public static ResizeGraphOptions _lastUsedInstance;

    /// <summary>
    /// Shows the resize graph dialog and if Ok, the graph is resized afterwards.
    /// </summary>
    /// <param name="doc">The graph document to resize.</param>
    public static void ShowResizeGraphDialog(Altaxo.Graph.Gdi.GraphDocument doc)
    {
      var resizeOptions = _lastUsedInstance is null ? new ResizeGraphOptions() : (ResizeGraphOptions)_lastUsedInstance.Clone();

      resizeOptions.InitializeOldValues(doc);

      var controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { resizeOptions }, typeof(IMVCANController));

      if (Current.Gui.ShowDialog(controller, "Resize graph"))
      {
        resizeOptions = (ResizeGraphOptions)controller.ModelObject;
        resizeOptions.ResizeGraph(doc);
        _lastUsedInstance = resizeOptions;
      }
    }

    /// <summary>
    /// Shows the resize graph dialog and if Ok, the graph is resized afterwards.
    /// </summary>
    /// <param name="docs">The graph documents to resize.</param>
    /// <returns>True if the graphs were resized; false otherwise.</returns>
    /// <remarks>The old values shown in the dialog are taken from the first graph in the enumeration.</remarks>
    public static bool ShowResizeGraphDialog(IEnumerable<Altaxo.Graph.Gdi.GraphDocument> docs)
    {
      var resizeOptions = _lastUsedInstance is null ? new ResizeGraphOptions() : (ResizeGraphOptions)_lastUsedInstance.Clone();

      var docEnum = docs.GetEnumerator();

      bool result = false;
      try
      {
        if (!docEnum.MoveNext())
          return result; // Enumeration is empty

        resizeOptions.InitializeOldValues(docEnum.Current);

        var controller = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { resizeOptions }, typeof(IMVCANController));

        if (Current.Gui.ShowDialog(controller, "Resize graph"))
        {
          resizeOptions = (ResizeGraphOptions)controller.ModelObject;

          do
          {
            resizeOptions.ResizeGraph(docEnum.Current);
          } while (docEnum.MoveNext());

          _lastUsedInstance = resizeOptions;
          result = true;
        }
      }
      finally
      {
        docEnum.Dispose();
      }
      return result;
    }
  }
}
