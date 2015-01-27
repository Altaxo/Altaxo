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

using Altaxo.Collections;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	public interface IGraphExportOptionsView
	{
		void SetImageFormat(SelectableListNodeList list);

		void SetPixelFormat(SelectableListNodeList list);

		void SetSourceDpi(SelectableListNodeList list);

		void SetDestinationDpi(SelectableListNodeList list);

		string SourceDpiResolution { get; }

		string DestinationDpiResolution { get; }

		BrushX BackgroundBrush { get; set; }
	}

	[ExpectedTypeOfView(typeof(IGraphExportOptionsView))]
	[UserControllerForObject(typeof(GraphExportOptions))]
	public class GraphExportOptionsController : MVCANControllerEditOriginalDocBase<GraphExportOptions, IGraphExportOptionsView>
	{
		private SelectableListNodeList _imageFormat;
		private SelectableListNodeList _pixelFormat;
		private SelectableListNodeList _sourceDpi;
		private SelectableListNodeList _destinationDpi;

		private static readonly int[] Resolutions = new int[] { 75, 150, 300, 400, 600, 1000, 1200, 1600, 2000, 2400, 4800 };

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
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				bool hasMatched;
				bool select;

				_imageFormat = new SelectableListNodeList();
				foreach (ImageFormat item in ImageFormats)
					_imageFormat.Add(new SelectableListNode(item.ToString(), item, _doc.ImageFormat == item));

				_pixelFormat = new SelectableListNodeList();
				hasMatched = false; // special prog to account for doubling of items in PixelFormats
				foreach (PixelFormat item in PixelFormats)
				{
					select = _doc.PixelFormat == item;
					_pixelFormat.Add(new SelectableListNode(item.ToString(), item, !hasMatched && select));
					hasMatched |= select;
				}

				_sourceDpi = GetResolutions(_doc.SourceDpiResolution);
				_destinationDpi = GetResolutions(_doc.DestinationDpiResolution);
			}

			if (null != _view)
			{
				_view.SetImageFormat(_imageFormat);
				_view.SetPixelFormat(_pixelFormat);
				_view.SetSourceDpi(_sourceDpi);
				_view.SetDestinationDpi(_destinationDpi);
				_view.BackgroundBrush = null == _doc.BackgroundBrush ? new BrushX(NamedColors.Transparent) : _doc.BackgroundBrush;
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
			double sr, dr;

			if (!GUIConversion.IsDouble(_view.SourceDpiResolution, out sr))
				return false;
			if (!GUIConversion.IsDouble(_view.DestinationDpiResolution, out dr))
				return false;

			if (!(sr > 0))
				return false;
			if (!(dr > 0))
				return false;

			var imgfmt = (ImageFormat)_imageFormat.FirstSelectedNode.Tag;
			var pixfmt = (PixelFormat)_pixelFormat.FirstSelectedNode.Tag;

			if (!_doc.TrySetImageAndPixelFormat(imgfmt, pixfmt))
			{
				Current.Gui.ErrorMessageBox("This combination of image and pixel format is not working!");
				return false;
			}

			_doc.SourceDpiResolution = sr;
			_doc.DestinationDpiResolution = dr;

			if (_view.BackgroundBrush.IsVisible)
				_doc.BackgroundBrush = _view.BackgroundBrush;
			else
				_doc.BackgroundBrush = null;

			return ApplyEnd(true, disposeController);
		}

		#endregion IApplyController Members
	}
}