#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public interface IClipboardRenderingOptionsView
	{
		object EmbeddedRenderingOptionsView { set; }

		bool RenderDropfile { get; set; }

		void SetDropFileImageFormat(SelectableListNodeList list);

		void SetDropFilePixelFormat(SelectableListNodeList list);

		bool RenderEmbeddedObject { get; set; }

		bool RenderLinkedObject { get; set; }
	}

	[ExpectedTypeOfView(typeof(IClipboardRenderingOptionsView))]
	[UserControllerForObject(typeof(ClipboardRenderingOptions), 101)]
	public class ClipboardRenderingOptionsController : MVCANControllerEditOriginalDocBase<ClipboardRenderingOptions, IClipboardRenderingOptionsView>
	{
		private EmbeddedObjectRenderingOptionsController _embeddedController;
		private SelectableListNodeList _imageFormat;
		private SelectableListNodeList _pixelFormat;

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

		public override void Dispose(bool isDisposing)
		{
			_imageFormat = null;
			_pixelFormat = null;
			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_embeddedController = new EmbeddedObjectRenderingOptionsController() { UseDocumentCopy = UseDocument.Directly };
				_embeddedController.InitializeDocument(_doc);
				Current.Gui.FindAndAttachControlTo(_embeddedController);

				_imageFormat = new SelectableListNodeList();
				foreach (ImageFormat item in ImageFormats)
					_imageFormat.Add(new SelectableListNode(item.ToString(), item, _doc.DropFileImageFormat == item));

				_pixelFormat = new SelectableListNodeList();
				var hasMatched = false; // special prog to account for doubling of items in PixelFormats
				foreach (PixelFormat item in PixelFormats)
				{
					var select = _doc.DropFileBitmapPixelFormat == item;
					_pixelFormat.Add(new SelectableListNode(item.ToString(), item, !hasMatched && select));
					hasMatched |= select;
				}
			}

			if (null != _view)
			{
				_view.EmbeddedRenderingOptionsView = _embeddedController.ViewObject;

				_view.RenderDropfile = _doc.RenderDropFile;
				_view.RenderEmbeddedObject = _doc.RenderEmbeddedObject;
				_view.RenderLinkedObject = _doc.RenderLinkedObject;

				_view.SetDropFileImageFormat(_imageFormat);
				_view.SetDropFilePixelFormat(_pixelFormat);
			}
		}

		#region IApplyController Members

		public override bool Apply(bool disposeController)
		{
			if (!_embeddedController.Apply(disposeController))
				return false;

			_doc.RenderDropFile = _view.RenderDropfile;
			var imgfmt = (ImageFormat)_imageFormat.FirstSelectedNode.Tag;
			var pixfmt = (PixelFormat)_pixelFormat.FirstSelectedNode.Tag;

			if (!_doc.TrySetImageAndPixelFormat(imgfmt, pixfmt))
			{
				Current.Gui.ErrorMessageBox("This combination of image and pixel format is not working!");
				return false;
			}
			_doc.RenderEmbeddedObject = _view.RenderEmbeddedObject;
			_doc.RenderLinkedObject = _view.RenderLinkedObject;

			return ApplyEnd(true, disposeController);
		}

		#endregion IApplyController Members
	}
}