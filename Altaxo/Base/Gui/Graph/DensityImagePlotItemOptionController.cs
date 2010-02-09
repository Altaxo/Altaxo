using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Gdi.Plot;
	#region Interfaces

	public interface IDensityImagePlotItemOptionView
	{
		event Action CopyImageToClipboard;
		event Action SaveImageToDisc;
	}


	#endregion

	/// <summary>
	/// Controls the option tab page in the <see cref="DensityImagePlotItem"/> dialog. This tab page allows only
	/// to save the image to clipboard or disc, thus the document is not really controlled.
	/// </summary>
	[ExpectedTypeOfView(typeof(IDensityImagePlotItemOptionView))]
	public class DensityImagePlotItemOptionController : IMVCANController
	{
		IDensityImagePlotItemOptionView _view;
		DensityImagePlotItem _doc;

		void Initialize(bool initData)
		{
		}

		void EhCopyImageToClipboard()
		{
			var bitmap = _doc.GetPixelwiseImage();
			bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
			var dao = Current.Gui.GetNewClipboardDataObject();
			dao.SetData(bitmap.GetType(), bitmap);
			Current.Gui.SetClipboardDataObject(dao);
		}

		void EhSaveImageToDisc()
		{
			var saveOptions = new Gui.SaveFileOptions() { Title = "Choose a file name to save the image" };
			saveOptions.AddFilter("*.png", "Png files (*.png)");
			saveOptions.AddFilter("*.tif", "Tiff files (*.tif)");
			if (!Current.Gui.ShowSaveFileDialog(saveOptions))
				return;
			
			var bitmap = _doc.GetPixelwiseImage();
			bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);

			bitmap.Save(saveOptions.FileName);
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
				if (null != _view)
				{
					_view.CopyImageToClipboard -= EhCopyImageToClipboard;
					_view.SaveImageToDisc -= EhSaveImageToDisc;
				}
				_view = value as IDensityImagePlotItemOptionView;

				if (_view != null)
				{
					Initialize(false);

					_view.CopyImageToClipboard += EhCopyImageToClipboard;
					_view.SaveImageToDisc += EhSaveImageToDisc;
				}
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		#endregion

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (0 == args.Length || !(args[0] is DensityImagePlotItem))
				return false;

			_doc = (DensityImagePlotItem)args[0];
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			return true;
		}

		#endregion
	}
}
