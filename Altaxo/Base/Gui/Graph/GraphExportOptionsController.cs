using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;

using Altaxo.Collections;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{


	public interface IGraphExportOptionsView
	{
		void SetImageFormat(SelectableListNodeList list);
		void SetPixelFormat(SelectableListNodeList list);
		void SetExportArea(SelectableListNodeList list);
		void SetSourceDpi(SelectableListNodeList list);
		void SetDestinationDpi(SelectableListNodeList list);
    void SetClipboardFormat(SelectableListNodeList list);
    bool EnableClipboardFormat { set; }
		string SourceDpiResolution { get; }
		string DestinationDpiResolution { get; }
    BrushX BackgroundBrush { get; set; }
	}

	[ExpectedTypeOfView(typeof(IGraphExportOptionsView))]
	[UserControllerForObject(typeof(GraphExportOptions))]
	public class GraphExportOptionsController : IMVCANController
	{
		IGraphExportOptionsView _view;
		GraphExportOptions _doc;
		GraphExportOptions _tempDoc;

		SelectableListNodeList _imageFormat;
		SelectableListNodeList _pixelFormat;
		SelectableListNodeList _exportArea;
		SelectableListNodeList _sourceDpi;
		SelectableListNodeList _destinationDpi;
    SelectableListNodeList _clipboardFormat;
    static readonly int[] Resolutions = new int[] { 75, 150, 300, 400, 600, 1000, 1200, 1600, 2000, 2400, 4800 };
		static readonly ImageFormat[] ImageFormats = new ImageFormat[]
		{ ImageFormat.Bmp,
			ImageFormat.Emf,
			ImageFormat.Exif,
			ImageFormat.Gif,
			//ImageFormat.Icon,
			ImageFormat.Jpeg, 
			//ImageFormat.MemoryBmp,
			ImageFormat.Png,
			ImageFormat.Tiff,
			ImageFormat.Wmf};

		static readonly PixelFormat[] PixelFormats = new PixelFormat[]
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



		
	

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length >= 1 && args[0] is GraphExportOptions)
			{
				_doc = (GraphExportOptions)args[0];
				_tempDoc = new GraphExportOptions();
				_tempDoc.CopyFrom(_doc);

				Initialize(true);
				return true;
			}

			return false;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion

		void Initialize(bool initData)
		{
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


				_exportArea = new SelectableListNodeList();
				foreach (GraphExportArea item in Enum.GetValues(typeof(GraphExportArea)))
					_exportArea.Add(new SelectableListNode(item.ToString(), item, _doc.ExportArea == item));

        _clipboardFormat = new SelectableListNodeList();
        foreach (GraphCopyPageClipboardFormat item in Enum.GetValues(typeof(GraphCopyPageClipboardFormat)))
          _clipboardFormat.Add(new SelectableListNode(item.ToString(), item, _doc.ClipboardFormat == item));

				_sourceDpi = GetResolutions(_doc.SourceDpiResolution);
				_destinationDpi = GetResolutions(_doc.DestinationDpiResolution);
			}

			if (null != _view)
			{
        _view.EnableClipboardFormat = _doc.IsIntentedForClipboardOperation;
        _view.SetClipboardFormat(_clipboardFormat);
				_view.SetImageFormat(_imageFormat);
				_view.SetPixelFormat(_pixelFormat);
				_view.SetExportArea(_exportArea);
				_view.SetSourceDpi(_sourceDpi);
				_view.SetDestinationDpi(_destinationDpi);
        _view.BackgroundBrush = null == _doc.BackgroundBrush ? new BrushX(Color.Transparent) : _doc.BackgroundBrush;
			}
		}

		SelectableListNodeList GetResolutions(double actualResolution)
		{
			var resolutions = new SortedList<double,object>();
			foreach (int res in Resolutions)
				resolutions.Add(res, null);

			if (!resolutions.ContainsKey(actualResolution))
				resolutions.Add(actualResolution, null);

			var result = new SelectableListNodeList();
			foreach (double res in resolutions.Keys)
				result.Add(new SelectableListNode(Serialization.GUIConversion.ToString(res), res, res == actualResolution));

			return result;

		}

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IGraphExportOptionsView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			string s;
			double sr, dr;
			
			if (!Serialization.GUIConversion.IsDouble(_view.SourceDpiResolution, out sr))
				return false;
			if (!Serialization.GUIConversion.IsDouble(_view.DestinationDpiResolution, out dr))
				return false;

			if(!(sr>0))
				return false;
			if(!(dr>0))
				return false;

			var imgfmt = (ImageFormat)_imageFormat.FirstSelectedNode.Tag;
			var pixfmt = (PixelFormat)_pixelFormat.FirstSelectedNode.Tag;

			if (!_doc.TrySetImageAndPixelFormat(imgfmt, pixfmt))
			{
				Current.Gui.ErrorMessageBox("This combination of image and pixel format is not working!");
				return false;
			}
			
			_doc.ExportArea = (GraphExportArea)_exportArea.FirstSelectedNode.Tag;
      _doc.ClipboardFormat = (GraphCopyPageClipboardFormat)_clipboardFormat.FirstSelectedNode.Tag;
			_doc.SourceDpiResolution = sr;
			_doc.DestinationDpiResolution = dr;

      if (_view.BackgroundBrush.IsVisible)
        _doc.BackgroundBrush = _view.BackgroundBrush;
      else
        _doc.BackgroundBrush = null;

			return true;
		}

		#endregion
	}
}
