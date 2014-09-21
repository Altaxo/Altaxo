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
	public interface IEmbeddedObjectRenderingOptionsView
	{
		void SetSourceDpi(SelectableListNodeList list);

		string SourceDpiResolution { get; }

		NamedColor BackgroundColor { get; set; }

		BrushX BackgroundBrush { get; set; }

		bool RenderEnhancedMetafile { get; set; }

		bool RenderEnhancedMetafileAsVectorFormat { get; set; }

		bool RenderWindowsMetafile { get; set; }

		bool RenderBitmap { get; set; }
	}

	[ExpectedTypeOfView(typeof(IEmbeddedObjectRenderingOptionsView))]
	[UserControllerForObject(typeof(EmbeddedObjectRenderingOptions))]
	public class EmbeddedObjectRenderingOptionsController : MVCANControllerBase<EmbeddedObjectRenderingOptions, IEmbeddedObjectRenderingOptionsView>
	{
		private static readonly int[] Resolutions = new int[] { 75, 150, 300, 400, 600, 1000, 1200, 1600, 2000, 2400, 4800 };
		private SelectableListNodeList _sourceDpi;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_sourceDpi = GetResolutions(_originalDoc.SourceDpiResolution);
			}

			if (null != _view)
			{
				_view.SetSourceDpi(_sourceDpi);
				_view.BackgroundColor = _doc.BackgroundColorForFormatsWithoutAlphaChannel;
				_view.BackgroundBrush = null == _doc.BackgroundBrush ? new BrushX(NamedColors.Transparent) : _doc.BackgroundBrush;

				_view.RenderEnhancedMetafile = _doc.RenderEnhancedMetafile;
				_view.RenderEnhancedMetafileAsVectorFormat = _doc.RenderEnhancedMetafileAsVectorFormat;
				_view.RenderWindowsMetafile = _doc.RenderWindowsMetafile;
				_view.RenderBitmap = _doc.RenderBitmap;
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

		public override bool Apply()
		{
			double sr;

			if (!GUIConversion.IsDouble(_view.SourceDpiResolution, out sr))
				return false;

			if (!(sr > 0))
				return false;

			var backcolor = _view.BackgroundColor;
			if (backcolor.Color.A != 255)
			{
				Current.Gui.ErrorMessageBox("Background color must be fully opaque. Please select an opaque (non-transparent) color");
				return false;
			}

			_doc.SourceDpiResolution = sr;
			_doc.BackgroundColorForFormatsWithoutAlphaChannel = backcolor;
			_doc.RenderEnhancedMetafile = _view.RenderEnhancedMetafile;
			_doc.RenderEnhancedMetafileAsVectorFormat = _view.RenderEnhancedMetafileAsVectorFormat;
			_doc.RenderWindowsMetafile = _view.RenderWindowsMetafile;
			_doc.RenderBitmap = _view.RenderBitmap;

			if (_view.BackgroundBrush.IsVisible)
				_doc.BackgroundBrush = _view.BackgroundBrush;
			else
				_doc.BackgroundBrush = null;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}

		#endregion IApplyController Members
	}
}