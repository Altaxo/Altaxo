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

	#endregion Interfaces

	/// <summary>
	/// Controls the option tab page in the <see cref="DensityImagePlotItem"/> dialog. This tab page allows only
	/// to save the image to clipboard or disc, thus the document is not really controlled.
	/// </summary>
	[ExpectedTypeOfView(typeof(IDensityImagePlotItemOptionView))]
	public class DensityImagePlotItemOptionController : IMVCANController
	{
		private IDensityImagePlotItemOptionView _view;
		private DensityImagePlotItem _doc;

		private void Initialize(bool initData)
		{
		}

		private void EhCopyImageToClipboard()
		{
			var bitmap = _doc.GetPixelwiseImage();
			bitmap.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
			var dao = Current.Gui.GetNewClipboardDataObject();
			dao.SetData(bitmap.GetType(), bitmap);
			Current.Gui.SetClipboardDataObject(dao);
		}

		private void EhSaveImageToDisc()
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

		public void Dispose()
		{
		}

		#endregion IMVCController Members

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

		#endregion IMVCANController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}