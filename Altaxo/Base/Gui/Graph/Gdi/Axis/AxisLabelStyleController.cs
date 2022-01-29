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
using System;
using System.Collections.Generic;
using System.Drawing;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Graph.Gdi.Background;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  public interface IAxisLabelStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description.
  /// </summary>
  [UserControllerForObject(typeof(AxisLabelStyle))]
  [ExpectedTypeOfView(typeof(IAxisLabelStyleView))]
  public class AxisLabelStyleController : MVCANControllerEditOriginalDocBase<AxisLabelStyle, IAxisLabelStyleView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_labelFormattingSpecificController, () => _labelFormattingSpecificController = null);
      yield return new ControllerAndSetNullMethod(_labelFontController, () => _labelFontController = null);
      yield return new ControllerAndSetNullMethod(_backgroundController, () => _backgroundController = null);
    }

    #region Bindings

    private FontXController _labelFontController;

    public FontXController LabelFontController
    {
      get => _labelFontController;
      set
      {
        if (!(_labelFontController == value))
        {
          _labelFontController?.Dispose();
          _labelFontController = value;
          OnPropertyChanged(nameof(LabelFontController));
        }
      }
    }

    private BackgroundStyleController _backgroundController;

    public BackgroundStyleController BackgroundController
    {
      get => _backgroundController;
      set
      {
        if (!(_backgroundController == value))
        {
          _backgroundController?.Dispose();
          _backgroundController = value;
          OnPropertyChanged(nameof(BackgroundController));
        }
      }
    }




    private BrushX _labelBrush;

    /// <summary>
    /// Initializes the content of the Color combo box.
    /// </summary>
    public BrushX LabelBrush
    {
      get => _labelBrush;
      set
      {
        if (!(_labelBrush == value))
        {
          _labelBrush = value;
          OnPropertyChanged(nameof(LabelBrush));
        }
      }
    }

    private IBackgroundStyle _background;

    /// <summary>
    /// Initializes the background.
    /// </summary>
    public IBackgroundStyle Background
    {
      get => _background;
      set
      {
        if (!(_background == value))
        {
          _background = value;
          OnPropertyChanged(nameof(Background));
        }
      }
    }

    private DimensionfulQuantity _fontSize;

    /// <summary>
    /// Value of the font size in points (1/72 inch).
    /// </summary>
    public DimensionfulQuantity FontSize
    {
      get => _fontSize;
      set
      {
        if (!(_fontSize == value))
        {
          _fontSize = value;
          OnPropertyChanged(nameof(FontSize));
        }
      }
    }

    private bool _automaticAlignment;

    /// <summary>
    /// Sets the automatic alignment check box.
    /// </summary>
    public bool AutomaticAlignment
    {
      get => _automaticAlignment;
      set
      {
        if (!(_automaticAlignment == value))
        {
          _automaticAlignment = value;
          OnPropertyChanged(nameof(AutomaticAlignment));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment RotationEnvironment { get; set; } = AngleEnvironment.Instance;

    private DimensionfulQuantity _rotation;

    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    public DimensionfulQuantity Rotation
    {
      get => _rotation;
      set
      {
        if (!(_rotation == value))
        {
          _rotation = value;
          OnPropertyChanged(nameof(Rotation));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment OffsetEnvironment { get; set; } = RelationEnvironment.Instance;

    private DimensionfulQuantity _xOffset;

    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    public DimensionfulQuantity XOffset
    {
      get => _xOffset;
      set
      {
        if (!(_xOffset == value))
        {
          _xOffset = value;
          OnPropertyChanged(nameof(XOffset));
        }
      }
    }

    private DimensionfulQuantity _yOffset;

    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
    public DimensionfulQuantity YOffset
    {
      get => _yOffset;
      set
      {
        if (!(_yOffset == value))
        {
          _yOffset = value;
          OnPropertyChanged(nameof(YOffset));
        }
      }
    }

    private string _prefixText;

    /// <summary>Gets or sets the prefix text that appears before the label.</summary>
    /// <value>The prefix text.</value>

    public string PrefixText
    {
      get => _prefixText;
      set
      {
        if (!(_prefixText == value))
        {
          _prefixText = value;
          OnPropertyChanged(nameof(PrefixText));
        }
      }
    }
    private string _postfixText;


    /// <summary>Gets or sets the postfix text that appears after the label.</summary>
    /// <value>The postfix text.</value>
    public string PostfixText
    {
      get => _postfixText;
      set
      {
        if (!(_postfixText == value))
        {
          _postfixText = value;
          OnPropertyChanged(nameof(PostfixText));
        }
      }
    }

    private string _suppressedLabelsByNumber;

    public string SuppressedLabelsByNumber
    {
      get => _suppressedLabelsByNumber;
      set
      {
        if (!(_suppressedLabelsByNumber == value))
        {
          _suppressedLabelsByNumber = value;
          OnPropertyChanged(nameof(SuppressedLabelsByNumber));
        }
      }
    }
    private string _suppressedLabelsByValue;

    public string SuppressedLabelsByValue
    {
      get => _suppressedLabelsByValue;
      set
      {
        if (!(_suppressedLabelsByValue == value))
        {
          _suppressedLabelsByValue = value;
          OnPropertyChanged(nameof(SuppressedLabelsByValue));
        }
      }
    }

    private SelectableListNodeList _horizontalAlignmentChoices;

    /// <summary>
    /// Initializes the horizontal aligment combo box.
    /// </summary>
    public SelectableListNodeList HorizontalAlignment
    {
      get => _horizontalAlignmentChoices;
      set
      {
        if (!(_horizontalAlignmentChoices == value))
        {
          _horizontalAlignmentChoices = value;
          OnPropertyChanged(nameof(HorizontalAlignment));
        }
      }
    }

    private StringAlignment _selectedHorizontalAlignment;

    public StringAlignment SelectedHorizontalAlignment
    {
      get => _selectedHorizontalAlignment;
      set
      {
        if (!(_selectedHorizontalAlignment == value))
        {
          _selectedHorizontalAlignment = value;
          OnPropertyChanged(nameof(SelectedHorizontalAlignment));
        }
      }
    }


    private SelectableListNodeList _verticalAlignmentChoices;

    /// <summary>
    /// Initializes the vertical alignement combo box.
    /// </summary>
    public SelectableListNodeList VerticalAlignment
    {
      get => _verticalAlignmentChoices;
      set
      {
        if (!(_verticalAlignmentChoices == value))
        {
          _verticalAlignmentChoices = value;
          OnPropertyChanged(nameof(VerticalAlignment));
        }
      }
    }

    private StringAlignment _selectedVerticalAlignment;

    public StringAlignment SelectedVerticalAlignment
    {
      get => _selectedVerticalAlignment;
      set
      {
        if (!(_selectedVerticalAlignment == value))
        {
          _selectedVerticalAlignment = value;
          OnPropertyChanged(nameof(SelectedVerticalAlignment));
        }
      }
    }


    private SelectableListNodeList _labelStyles;

    /// <summary>
    /// Initializes the label style combo box.
    /// </summary>
    public SelectableListNodeList LabelStyles
    {
      get => _labelStyles;
      set
      {
        if (!(_labelStyles == value))
        {
          _labelStyles = value;
          OnPropertyChanged(nameof(LabelStyles));
        }
      }
    }

    private Type _selectedLabelStyle;

    public Type SelectedLabelStyle
    {
      get => _selectedLabelStyle;
      set
      {
        if (!(_selectedLabelStyle == value))
        {
          _selectedLabelStyle = value;
          OnPropertyChanged(nameof(SelectedLabelStyle));
          if (value is not null)
          {
            EhView_LabelStyleChanged(value);
          }
        }
      }
    }

    void EhView_LabelStyleChanged(System.Type value)
    {
      if (_doc.LabelFormat.GetType() != value)
      {
        _doc.LabelFormat = (Altaxo.Graph.Gdi.LabelFormatting.ILabelFormatting)Activator.CreateInstance(value);
        LabelFormattingSpecificController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.LabelFormat }, typeof(IMVCANController), UseDocument.Directly);
      }
    }


    private SelectableListNodeList _labelSides;

    /// <summary>Sets the possible choices for the label side.</summary>
    public SelectableListNodeList LabelSides
    {
      get => _labelSides;
      set
      {
        if (!(_labelSides == value))
        {
          _labelSides = value;
          OnPropertyChanged(nameof(LabelSides));
        }
      }
    }

    private IMVCANController _labelFormattingSpecificController;

    public IMVCANController LabelFormattingSpecificController
    {
      get => _labelFormattingSpecificController;
      set
      {
        if (!(_labelFormattingSpecificController == value))
        {
          _labelFormattingSpecificController?.Dispose();
          _labelFormattingSpecificController = value;
          OnPropertyChanged(nameof(LabelFormattingSpecificController));
          OnPropertyChanged(nameof(LabelFormattingSpecificGuiControl));
        }
      }
    }

    /// <summary>Sets the label formatting specific GUI control. If no specific options are available, this property is set to <c>null</c>.</summary>
    /// <value>The label formatting specific GUI control.</value>
    public object LabelFormattingSpecificGuiControl => _labelFormattingSpecificController?.ViewObject;

    #endregion

    public override void Dispose(bool isDisposing)
    {
      _labelSides = null;
      _horizontalAlignmentChoices = null;
      _verticalAlignmentChoices = null;
      _labelStyles = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // Label sides
        _labelSides = new Collections.SelectableListNodeList
        {
          new Collections.SelectableListNode("Automatic", null, _doc.LabelSide is null)
        };
        var list = new List<Collections.SelectableListNode>();
        if (_doc.CachedAxisInformation is not null)
        {
          list.Add(new Collections.SelectableListNode(_doc.CachedAxisInformation.NameOfFirstDownSide, CSAxisSide.FirstDown, _doc.LabelSide == CSAxisSide.FirstDown));
          list.Add(new Collections.SelectableListNode(_doc.CachedAxisInformation.NameOfFirstUpSide, CSAxisSide.FirstUp, _doc.LabelSide == CSAxisSide.FirstUp));
        }
        list.Sort((x, y) => string.Compare(x.Text, y.Text));
        _labelSides.AddRange(list);

        // horizontal and vertical alignment
        _horizontalAlignmentChoices = new Collections.SelectableListNodeList(_doc.HorizontalAlignment);
        _verticalAlignmentChoices = new Collections.SelectableListNodeList(_doc.VerticalAlignment);

        // label formatting type
        var labelTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.Gdi.LabelFormatting.ILabelFormatting));
        _labelStyles = new Collections.SelectableListNodeList();
        for (int i = 0; i < labelTypes.Length; ++i)
        {
          _labelStyles.Add(new Collections.SelectableListNode(labelTypes[i].Name, labelTypes[i], labelTypes[i] == _doc.LabelFormat.GetType()));
        }
        SelectedLabelStyle = _doc.LabelFormat.GetType();

        _labelFormattingSpecificController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.LabelFormat }, typeof(IMVCANController), UseDocument.Directly);

        _labelFontController = new FontXController();
        _labelFontController.InitializeDocument(_doc.Font);

        _backgroundController = new BackgroundStyleController(_doc.BackgroundStyle); // no view, because controls locally here

        LabelBrush = _doc.Brush;
        AutomaticAlignment = _doc.AutomaticAlignment;
        Rotation = new DimensionfulQuantity(_doc.Rotation, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        XOffset = new DimensionfulQuantity(_doc.XOffset, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEnvironment.DefaultUnit);
        YOffset = new DimensionfulQuantity(_doc.YOffset, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEnvironment.DefaultUnit);
        Background = _doc.BackgroundStyle;
        SuppressedLabelsByValue = GUIConversion.ToString(_doc.SuppressedLabels.ByValues);
        SuppressedLabelsByNumber = GUIConversion.ToString(_doc.SuppressedLabels.ByNumbers);
        PrefixText = _doc.PrefixText;
        PostfixText = _doc.SuffixText;
        LabelSides = _labelSides;
      }

    }

    public override bool Apply(bool disposeController)
    {
      if (!_labelFontController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      if (!_backgroundController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.Font = (FontX)_labelFontController.ModelObject;
      _doc.Brush = LabelBrush;
      _doc.HorizontalAlignment = SelectedHorizontalAlignment;
      _doc.VerticalAlignment = SelectedVerticalAlignment;
      _doc.AutomaticAlignment = AutomaticAlignment;
      _doc.Rotation = Rotation.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
      _doc.XOffset = XOffset.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
      _doc.YOffset = YOffset.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
      _doc.BackgroundStyle = (IBackgroundStyle?)_backgroundController.ModelObject;

      if (GUIConversion.TryParseMultipleAltaxoVariant(SuppressedLabelsByValue, out var varVals))
      {
        _doc.SuppressedLabels.ByValues.Clear();
        foreach (AltaxoVariant v in varVals)
          _doc.SuppressedLabels.ByValues.Add(v);
      }
      else
        return false;

      if (GUIConversion.TryParseMultipleInt32(SuppressedLabelsByNumber, out var intVals))
      {
        _doc.SuppressedLabels.ByNumbers.Clear();
        foreach (int v in intVals)
          _doc.SuppressedLabels.ByNumbers.Add(v);
      }
      else
        return false;

      _doc.PrefixText = PrefixText;
      _doc.SuffixText = PostfixText;

      var labelSideNode = _labelSides.FirstSelectedNode;
      if (labelSideNode is not null)
        _doc.LabelSide = (CSAxisSide?)labelSideNode.Tag;

      if (_labelFormattingSpecificController is not null && !_labelFormattingSpecificController.Apply(disposeController))
        return false;

      return ApplyEnd(true, disposeController);
    }


  }
}
