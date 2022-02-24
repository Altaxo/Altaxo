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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IGraphExportOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IGraphExportOptionsView))]
  [UserControllerForObject(typeof(GraphExportOptions))]
  public class GraphExportOptionsController : MVCANControllerEditOriginalDocBase<GraphExportOptions, IGraphExportOptionsView>
  {
    private static readonly int[] Resolutions = new int[] { 75, 150, 300, 400, 600, 1000, 1200, 1600, 2000, 2400, 4800 };

    private static readonly ImageFormat[] ImageFormats = new ImageFormat[]
    {
      System.Drawing.Imaging.ImageFormat.Bmp,
      System.Drawing.Imaging.ImageFormat.Emf,
      System.Drawing.Imaging.ImageFormat.Exif,
      System.Drawing.Imaging.ImageFormat.Gif,
			//ImageFormat.Icon,
			System.Drawing.Imaging.ImageFormat.Jpeg,
			//ImageFormat.MemoryBmp,
			System.Drawing.Imaging.ImageFormat.Png,
      System.Drawing.Imaging.ImageFormat.Tiff,
      System.Drawing.Imaging.ImageFormat.Wmf
    };

    private static readonly PixelFormat[] PixelFormats = new PixelFormat[]
    {
			// The next three formats are the most used, so we have them on top
			System.Drawing.Imaging.PixelFormat.Format24bppRgb,
      System.Drawing.Imaging.PixelFormat.Format32bppRgb,
      System.Drawing.Imaging.PixelFormat.Format32bppArgb,

      System.Drawing.Imaging.PixelFormat.Format1bppIndexed,
      System.Drawing.Imaging.PixelFormat.Format4bppIndexed,
      System.Drawing.Imaging.PixelFormat.Format8bppIndexed,

      System.Drawing.Imaging.PixelFormat.Format16bppArgb1555,
      System.Drawing.Imaging.PixelFormat.Format16bppGrayScale,
      System.Drawing.Imaging.PixelFormat.Format16bppRgb555,
      System.Drawing.Imaging.PixelFormat.Format16bppRgb565,

      System.Drawing.Imaging.PixelFormat.Format24bppRgb,

      System.Drawing.Imaging.PixelFormat.Format32bppRgb,
      System.Drawing.Imaging.PixelFormat.Format32bppArgb,
      System.Drawing.Imaging.PixelFormat.Format32bppPArgb,

      System.Drawing.Imaging.PixelFormat.Format48bppRgb,

      System.Drawing.Imaging.PixelFormat.Format64bppArgb,
      System.Drawing.Imaging.PixelFormat.Format64bppPArgb,

      System.Drawing.Imaging.PixelFormat.Alpha,
      System.Drawing.Imaging.PixelFormat.PAlpha
    };

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

    private ItemsController<double> _DestinationDpi;

    public ItemsController<double> DestinationDpi
    {
      get => _DestinationDpi;
      set
      {
        if (!(_DestinationDpi == value))
        {
          _DestinationDpi = value;
          OnPropertyChanged(nameof(DestinationDpi));
        }
      }
    }
    private string _DestinationDpiText;

    public string DestinationDpiText
    {
      get => _DestinationDpiText;
      set
      {
        if (!(_DestinationDpiText == value))
        {
          _DestinationDpiText = value;
          OnPropertyChanged(nameof(DestinationDpiText));
        }
      }
    }



    private ItemsController<ImageFormat> _imageFormat;

    public ItemsController<ImageFormat> ImageFormat
    {
      get => _imageFormat;
      set
      {
        if (!(_imageFormat == value))
        {
          _imageFormat?.Dispose();
          _imageFormat = value;
          OnPropertyChanged(nameof(ImageFormat));
        }
      }
    }

    private ItemsController<PixelFormat> _pixelFormat;

    public ItemsController<PixelFormat> PixelFormat
    {
      get => _pixelFormat;
      set
      {
        if (!(_pixelFormat == value))
        {
          _imageFormat?.Dispose();
          _pixelFormat = value;
          OnPropertyChanged(nameof(PixelFormat));
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

    private bool _enableClipboardFormat;

    public bool EnableClipboardFormat
    {
      get => _enableClipboardFormat;
      set
      {
        if (!(_enableClipboardFormat == value))
        {
          _enableClipboardFormat = value;
          OnPropertyChanged(nameof(EnableClipboardFormat));
        }
      }
    }


    private IMVCANController _clipboardFormatController;

    public IMVCANController ClipboardFormatController
    {
      get => _clipboardFormatController;
      set
      {
        if (!(_clipboardFormatController == value))
        {
          _clipboardFormatController = value;
          OnPropertyChanged(nameof(ClipboardFormatController));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        bool hasMatched;
        bool select;

        var imageFormat = new SelectableListNodeList();
        foreach (ImageFormat item in ImageFormats)
          imageFormat.Add(new SelectableListNode(item.ToString(), item, _doc.ImageFormat == item));
        ImageFormat = new ItemsController<ImageFormat>(imageFormat);

        var pixelFormat = new SelectableListNodeList();
        hasMatched = false; // special prog to account for doubling of items in PixelFormats
        foreach (PixelFormat item in PixelFormats)
        {
          select = _doc.PixelFormat == item;
          pixelFormat.Add(new SelectableListNode(item.ToString(), item, !hasMatched && select));
          hasMatched |= select;
        }
        PixelFormat = new ItemsController<PixelFormat>(pixelFormat);

        SourceDpi = new ItemsController<double>(GetResolutions(_doc.SourceDpiResolution));
        DestinationDpi = new ItemsController<double>(GetResolutions(_doc.DestinationDpiResolution));
        BackgroundBrush = _doc.BackgroundBrush ?? new BrushX(NamedColors.Transparent);
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
      if (!GUIConversion.IsDouble(DestinationDpiText, out var dr))
        return false;

      if (!(sr > 0))
        return false;
      if (!(dr > 0))
        return false;

      var imgfmt = _imageFormat.SelectedValue;
      var pixfmt = _pixelFormat.SelectedValue;

      if (!_doc.TrySetImageAndPixelFormat(imgfmt, pixfmt))
      {
        Current.Gui.ErrorMessageBox("This combination of image and pixel format is not working!");
        return false;
      }

      _doc.SourceDpiResolution = sr;
      _doc.DestinationDpiResolution = dr;

      if (BackgroundBrush.IsVisible)
        _doc.BackgroundBrush = BackgroundBrush;
      else
        _doc.BackgroundBrush = null;

      return ApplyEnd(true, disposeController);
    }

    #endregion IApplyController Members
  }
}
