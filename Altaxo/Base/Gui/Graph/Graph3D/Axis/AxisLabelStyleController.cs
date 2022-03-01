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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Axis;
using Altaxo.Graph.Graph3D.Background;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing.D3D;
using Altaxo.Gui.Graph.Graph3D.Background;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Graph3D.Axis
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
      yield return new ControllerAndSetNullMethod(_labelFormattingSpecificController, () => LabelFormattingSpecificController = null);
      yield return new ControllerAndSetNullMethod(_labelFontController, () => LabelFontController = null);
      yield return new ControllerAndSetNullMethod(_backgroundController, () => BackgroundController = null);
    }

    #region Bindings

    private FontX3DController _labelFontController;

    public FontX3DController LabelFontController
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

    private IMaterial _labelBrush;

    /// <summary>
    /// Initializes the content of the Color combo box.
    /// </summary>
    public IMaterial LabelBrush
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

    private DimensionfulQuantity _rotationX;

    /// <summary>
    /// Initializes the content of the Rotation edit box.
    /// </summary>
    public DimensionfulQuantity RotationX
    {
      get => _rotationX;
      set
      {
        if (!(_rotationX == value))
        {
          _rotationX = value;
          OnPropertyChanged(nameof(RotationX));
        }
      }
    }

    private DimensionfulQuantity _rotationY;

    public DimensionfulQuantity RotationY
    {
      get => _rotationY;
      set
      {
        if (!(_rotationY == value))
        {
          _rotationY = value;
          OnPropertyChanged(nameof(RotationY));
        }
      }
    }

    private DimensionfulQuantity _rotationZ;

    public DimensionfulQuantity RotationZ
    {
      get => _rotationZ;
      set
      {
        if (!(_rotationZ == value))
        {
          _rotationZ = value;
          OnPropertyChanged(nameof(RotationZ));
        }
      }
    }



    public QuantityWithUnitGuiEnvironment OffsetEnvironment { get; set; } = RelationEnvironment.Instance;

    private DimensionfulQuantity _offsetX;

    /// <summary>
    /// Initializes the content of the XOffset edit box.
    /// </summary>
    public DimensionfulQuantity OffsetX
    {
      get => _offsetX;
      set
      {
        if (!(_offsetX == value))
        {
          _offsetX = value;
          OnPropertyChanged(nameof(OffsetX));
        }
      }
    }

    private DimensionfulQuantity _offsetY;

    /// <summary>
    /// Initializes the content of the YOffset edit box.
    /// </summary>
    public DimensionfulQuantity OffsetY
    {
      get => _offsetY;
      set
      {
        if (!(_offsetY == value))
        {
          _offsetY = value;
          OnPropertyChanged(nameof(OffsetY));
        }
      }
    }

    private DimensionfulQuantity _offsetZ;

    public DimensionfulQuantity OffsetZ
    {
      get => _offsetZ;
      set
      {
        if (!(_offsetZ == value))
        {
          _offsetZ = value;
          OnPropertyChanged(nameof(OffsetZ));
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

    private ItemsController<Alignment> _alignmentX;

    /// <summary>
    /// Initializes the horizontal alignment combo box.
    /// </summary>
    public ItemsController<Alignment> AlignmentX
    {
      get => _alignmentX;
      set
      {
        if (!(_alignmentX == value))
        {
          _alignmentX?.Dispose();
          _alignmentX = value;
          OnPropertyChanged(nameof(AlignmentX));
        }
      }
    }

    private ItemsController<Alignment> _alignmentY;

    public ItemsController<Alignment> AlignmentY
    {
      get => _alignmentY;
      set
      {
        if (!(_alignmentY == value))
        {
          _alignmentY?.Dispose();
          _alignmentY = value;
          OnPropertyChanged(nameof(AlignmentY));
        }
      }
    }

    private ItemsController<Alignment> _alignmentZ;

    public ItemsController<Alignment> AlignmentZ
    {
      get => _alignmentZ;
      set
      {
        if (!(_alignmentZ == value))
        {
          _alignmentZ?.Dispose();
          _alignmentZ = value;
          OnPropertyChanged(nameof(AlignmentZ));
        }
      }
    }


    private ItemsController<Type> _labelStyle;

    public ItemsController<Type> LabelStyle
    {
      get => _labelStyle;
      set
      {
        if (!(_labelStyle == value))
        {
          _labelStyle?.Dispose();
          _labelStyle = value;
          OnPropertyChanged(nameof(LabelStyle));
        }
      }
    }

    public void EhView_LabelStyleChanged(System.Type value)
    {
      if (value is not null && _doc.LabelFormat.GetType() != value)
      {
        _doc.LabelFormat = (Altaxo.Graph.Graph3D.LabelFormatting.ILabelFormatting)Activator.CreateInstance(value);
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
        }
      }
    }


    #endregion


    public override void Dispose(bool isDisposing)
    {
      LabelSides = null;
      AlignmentX = null;
      AlignmentY = null;
      AlignmentZ = null;
      LabelStyle = null;

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
        AlignmentX = new ItemsController<Alignment>(new Collections.SelectableListNodeList(_doc.AlignmentX));
        AlignmentY = new ItemsController<Alignment>(new Collections.SelectableListNodeList(_doc.AlignmentY));
        AlignmentZ = new ItemsController<Alignment>(new Collections.SelectableListNodeList(_doc.AlignmentZ));

        // label formatting type
        var labelTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.Graph3D.LabelFormatting.ILabelFormatting));
        var labelStyles = new Collections.SelectableListNodeList();
        for (int i = 0; i < labelTypes.Length; ++i)
        {
          labelStyles.Add(new Collections.SelectableListNode(labelTypes[i].Name, labelTypes[i], labelTypes[i] == _doc.LabelFormat.GetType()));
        }
        LabelStyle = new ItemsController<Type>(labelStyles, EhView_LabelStyleChanged);

        _labelFormattingSpecificController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.LabelFormat }, typeof(IMVCANController), UseDocument.Directly);

        _labelFontController = new FontX3DController();
        _labelFontController.InitializeDocument(_doc.Font);

        _backgroundController = new BackgroundStyleController(_doc.BackgroundStyle);

        LabelBrush = _doc.Brush;
        AutomaticAlignment = _doc.AutomaticAlignment;
        RotationX = new DimensionfulQuantity(_doc.RotationX, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        RotationY = new DimensionfulQuantity(_doc.RotationY, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        RotationZ = new DimensionfulQuantity(_doc.RotationZ, Altaxo.Units.Angle.Degree.Instance).AsQuantityIn(RotationEnvironment.DefaultUnit);
        OffsetX = new DimensionfulQuantity(_doc.OffsetX, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEnvironment.DefaultUnit);
        OffsetY = new DimensionfulQuantity(_doc.OffsetY, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEnvironment.DefaultUnit);
        OffsetZ = new DimensionfulQuantity(_doc.OffsetZ, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OffsetEnvironment.DefaultUnit);
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

      _doc.Font = (FontX3D)_labelFontController.ModelObject;
      _doc.Brush = LabelBrush;

      _doc.AlignmentX = AlignmentX.SelectedValue;
      _doc.AlignmentY = AlignmentY.SelectedValue;
      _doc.AlignmentZ = AlignmentZ.SelectedValue;
      _doc.AutomaticAlignment = AutomaticAlignment;
      _doc.RotationX = RotationX.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
      _doc.RotationY = RotationY.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
      _doc.RotationZ = RotationZ.AsValueIn(Altaxo.Units.Angle.Degree.Instance);
      _doc.OffsetX = OffsetX.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
      _doc.OffsetY = OffsetY.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
      _doc.OffsetZ = OffsetZ.AsValueIn(Altaxo.Units.Dimensionless.Unity.Instance);
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
