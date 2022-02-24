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
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IEmbeddedObjectRenderingOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IEmbeddedObjectRenderingOptionsView))]
  [UserControllerForObject(typeof(EmbeddedObjectRenderingOptions))]
  public class EmbeddedObjectRenderingOptionsController : MVCANControllerEditOriginalDocBase<EmbeddedObjectRenderingOptions, IEmbeddedObjectRenderingOptionsView>
  {
    private static readonly int[] Resolutions = new int[] { 75, 150, 300, 400, 600, 1000, 1200, 1600, 2000, 2400, 4800 };

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<double> _sourceDpi;

    public ItemsController<double> SourceDpi
    {
      get => _sourceDpi;
      set
      {
        if (!(_sourceDpi == value))
        {
          _sourceDpi = value;
          OnPropertyChanged(nameof(SourceDpi));
        }
      }
    }

    private string _sourceDpiText;

    public string SourceDpiText
    {
      get => _sourceDpiText;
      set
      {
        if (!(_sourceDpiText == value))
        {
          _sourceDpiText = value;
          OnPropertyChanged(nameof(SourceDpiText));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment OutputScalingEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _outputScaling;

    public DimensionfulQuantity OutputScaling
    {
      get => _outputScaling;
      set
      {
        if (!(_outputScaling == value))
        {
          _outputScaling = value;
          OnPropertyChanged(nameof(OutputScaling));
        }
      }
    }

    private NamedColor _backgroundColor;

    public NamedColor BackgroundColor
    {
      get => _backgroundColor;
      set
      {
        if (!(_backgroundColor == value))
        {
          _backgroundColor = value;
          OnPropertyChanged(nameof(BackgroundColor));
        }
      }
    }

    private BrushX _backgroundBrush;

    public BrushX BackgroundBrush
    {
      get => _backgroundBrush;
      set
      {
        if (!(_backgroundBrush == value))
        {
          _backgroundBrush = value;
          OnPropertyChanged(nameof(BackgroundBrush));
        }
      }
    }


    private bool _renderEnhancedMetafile;

    public bool RenderEnhancedMetafile
    {
      get => _renderEnhancedMetafile;
      set
      {
        if (!(_renderEnhancedMetafile == value))
        {
          _renderEnhancedMetafile = value;
          OnPropertyChanged(nameof(RenderEnhancedMetafile));
        }
      }
    }


    private bool _renderEnhancedMetafileAsVectorFormat;

    public bool RenderEnhancedMetafileAsVectorFormat
    {
      get => _renderEnhancedMetafileAsVectorFormat;
      set
      {
        if (!(_renderEnhancedMetafileAsVectorFormat == value))
        {
          _renderEnhancedMetafileAsVectorFormat = value;
          OnPropertyChanged(nameof(RenderEnhancedMetafileAsVectorFormat));
        }
      }
    }


    private bool _renderWindowsMetafile;

    public bool RenderWindowsMetafile
    {
      get => _renderWindowsMetafile;
      set
      {
        if (!(_renderWindowsMetafile == value))
        {
          _renderWindowsMetafile = value;
          OnPropertyChanged(nameof(RenderWindowsMetafile));
        }
      }
    }


    private bool _renderBitmap;

    public bool RenderBitmap
    {
      get => _renderBitmap;
      set
      {
        if (!(_renderBitmap == value))
        {
          _renderBitmap = value;
          OnPropertyChanged(nameof(RenderBitmap));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        SourceDpi = new ItemsController<double>(GetResolutions(_doc.SourceDpiResolution));

        OutputScaling = new DimensionfulQuantity(_doc.OutputScalingFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(OutputScalingEnvironment.DefaultUnit);

        BackgroundColor = _doc.BackgroundColorForFormatsWithoutAlphaChannel;

        BackgroundBrush = _doc.BackgroundBrush ?? new BrushX(NamedColors.Transparent);

        RenderEnhancedMetafile = _doc.RenderEnhancedMetafile;

        RenderEnhancedMetafileAsVectorFormat = _doc.RenderEnhancedMetafileAsVectorFormat;

        RenderWindowsMetafile = _doc.RenderWindowsMetafile;

        RenderBitmap = _doc.RenderBitmap;
      }
    }

    private SelectableListNodeList GetResolutions(double actualResolution)
    {
      var resolutions = new SortedList<double, object>();
      foreach (int res in Resolutions)
        resolutions.Add(res, null);

      if (!resolutions.ContainsKey(actualResolution))
        resolutions.Add(actualResolution, null);

      var result = new SelectableListNodeList();
      foreach (double res in resolutions.Keys)
        result.Add(new SelectableListNode(GUIConversion.ToString(res), res, res == actualResolution));

      return result;
    }

    #region IApplyController Members

    public override bool Apply(bool disposeController)
    {

      if (!GUIConversion.IsDouble(SourceDpiText, out var sr))
        return false;

      if (!(sr > 0))
        return false;

      var backcolor = BackgroundColor;
      if (backcolor.Color.A != 255)
      {
        Current.Gui.ErrorMessageBox("Background color must be fully opaque. Please select an opaque (non-transparent) color");
        return false;
      }

      _doc.SourceDpiResolution = sr;
      _doc.OutputScalingFactor = OutputScaling.AsValueInSIUnits;
      _doc.BackgroundColorForFormatsWithoutAlphaChannel = backcolor;
      _doc.RenderEnhancedMetafile = RenderEnhancedMetafile;
      _doc.RenderEnhancedMetafileAsVectorFormat = RenderEnhancedMetafileAsVectorFormat;
      _doc.RenderWindowsMetafile = RenderWindowsMetafile;
      _doc.RenderBitmap = RenderBitmap;

      if (BackgroundBrush.IsVisible)
        _doc.BackgroundBrush = BackgroundBrush;
      else
        _doc.BackgroundBrush = null;

      return ApplyEnd(true, disposeController);
    }

    #endregion IApplyController Members
  }
}
