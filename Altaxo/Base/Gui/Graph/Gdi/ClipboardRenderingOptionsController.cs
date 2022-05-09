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
using System.Drawing.Imaging;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface IClipboardRenderingOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IClipboardRenderingOptionsView))]
  [UserControllerForObject(typeof(ClipboardRenderingOptions), 101)]
  public class ClipboardRenderingOptionsController : MVCANControllerEditOriginalDocBase<ClipboardRenderingOptions, IClipboardRenderingOptionsView>
  {

    private static readonly ImageFormat[] ImageFormats = new ImageFormat[]
    {
      ImageFormat.Bmp,
      ImageFormat.Emf,
      ImageFormat.Exif,
      ImageFormat.Gif,
			//ImageFormat.Icon,
	  ImageFormat.Jpeg,
			//ImageFormat.MemoryBmp,
			ImageFormat.Png,
      ImageFormat.Tiff,
      ImageFormat.Wmf
    };

    private static readonly PixelFormat[] PixelFormats = new PixelFormat[]
    {
			// The next three formats are the most used, so we have them on top
			PixelFormat.Format24bppRgb,
      PixelFormat.Format32bppRgb,
      PixelFormat.Format32bppArgb,

      PixelFormat.Format1bppIndexed,
      PixelFormat.Format4bppIndexed,
      PixelFormat.Format8bppIndexed,

      PixelFormat.Format16bppArgb1555,
      PixelFormat.Format16bppGrayScale,
      PixelFormat.Format16bppRgb555,
      PixelFormat.Format16bppRgb565,

      PixelFormat.Format24bppRgb,

      PixelFormat.Format32bppRgb,
      PixelFormat.Format32bppArgb,
      PixelFormat.Format32bppPArgb,

      PixelFormat.Format48bppRgb,

      PixelFormat.Format64bppArgb,
      PixelFormat.Format64bppPArgb,

      PixelFormat.Alpha,
      PixelFormat.PAlpha
    };

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_embeddedController, () => _embeddedController = null);
    }

    #region Bindings

    private EmbeddedObjectRenderingOptionsController _embeddedController;

    public EmbeddedObjectRenderingOptionsController EmbeddedController
    {
      get => _embeddedController;
      set
      {
        if (!(_embeddedController == value))
        {
          _embeddedController?.Dispose();
          _embeddedController = value;
          OnPropertyChanged(nameof(EmbeddedController));
        }
      }
    }

    private bool _renderDropFile;

    public bool RenderDropFile
    {
      get => _renderDropFile;
      set
      {
        if (!(_renderDropFile == value))
        {
          _renderDropFile = value;
          OnPropertyChanged(nameof(RenderDropFile));
        }
      }
    }

    private ItemsController<ImageFormat> _dropFileImageFormat;

    public ItemsController<ImageFormat> DropFileImageFormat
    {
      get => _dropFileImageFormat;
      set
      {
        if (!(_dropFileImageFormat == value))
        {
          _dropFileImageFormat?.Dispose();
          _dropFileImageFormat = value;
          OnPropertyChanged(nameof(DropFileImageFormat));
        }
      }
    }

    private ItemsController<PixelFormat> _dropFilePixelFormat;

    public ItemsController<PixelFormat> DropFilePixelFormat
    {
      get => _dropFilePixelFormat;
      set
      {
        if (!(_dropFilePixelFormat == value))
        {
          _dropFilePixelFormat?.Dispose();
          _dropFilePixelFormat = value;
          OnPropertyChanged(nameof(DropFilePixelFormat));
        }
      }
    }




    private bool _renderEmbeddedObject;

    public bool RenderEmbeddedObject
    {
      get => _renderEmbeddedObject;
      set
      {
        if (!(_renderEmbeddedObject == value))
        {
          _renderEmbeddedObject = value;
          OnPropertyChanged(nameof(RenderEmbeddedObject));
        }
      }
    }


    private bool _renderLinkedObject;

    public bool RenderLinkedObject
    {
      get => _renderLinkedObject;
      set
      {
        if (!(_renderLinkedObject == value))
        {
          _renderLinkedObject = value;
          OnPropertyChanged(nameof(RenderLinkedObject));
        }
      }
    }



    #endregion
    public override void Dispose(bool isDisposing)
    {
      DropFileImageFormat = null;
      DropFilePixelFormat = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var embeddedController = new EmbeddedObjectRenderingOptionsController() { UseDocumentCopy = UseDocument.Directly };
        embeddedController.InitializeDocument(_doc);
        Current.Gui.FindAndAttachControlTo(embeddedController);
        EmbeddedController = embeddedController;

        var imageFormat = new SelectableListNodeList();
        foreach (ImageFormat item in ImageFormats)
          imageFormat.Add(new SelectableListNode(item.ToString(), item, _doc.DropFileImageFormat == item));
        DropFileImageFormat = new ItemsController<ImageFormat>(imageFormat);

        var pixelFormat = new SelectableListNodeList();
        var hasMatched = false; // special prog to account for doubling of items in PixelFormats
        foreach (PixelFormat item in PixelFormats)
        {
          var select = _doc.DropFileBitmapPixelFormat == item;
          pixelFormat.Add(new SelectableListNode(item.ToString(), item, !hasMatched && select));
          hasMatched |= select;
        }
        DropFilePixelFormat = new ItemsController<PixelFormat>(pixelFormat);


        RenderDropFile = _doc.RenderDropFile;
        RenderEmbeddedObject = _doc.RenderEmbeddedObject;
        RenderLinkedObject = _doc.RenderLinkedObject;

      }
    }


    public override bool Apply(bool disposeController)
    {
      if (!_embeddedController.Apply(disposeController))
        return false;

      _doc.RenderDropFile = RenderDropFile;
      var imgfmt = DropFileImageFormat.SelectedValue;
      var pixfmt = DropFilePixelFormat.SelectedValue;

      if (!_doc.TrySetImageAndPixelFormat(imgfmt, pixfmt))
      {
        Current.Gui.ErrorMessageBox("This combination of image and pixel format is not working!");
        return false;
      }
      _doc.RenderEmbeddedObject = RenderEmbeddedObject;
      _doc.RenderLinkedObject = RenderLinkedObject;

      return ApplyEnd(true, disposeController);
    }


  }
}
