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

using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui;
using Altaxo.Serialization;
using System;
using System.Drawing;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Graph.Scales;

	#region Interfaces

	public interface IDensityImagePlotStyleView
	{
		IDensityScaleView DensityScaleView { get; }

		IColorProviderView ColorProviderView { get; }

		/// <summary>
		/// Initializes the content of the ClipToLayer checkbox
		/// </summary>
		bool ClipToLayer { get; set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Controller for the density image plot style
	/// </summary>
	[UserControllerForObject(typeof(DensityImagePlotStyle))]
	[ExpectedTypeOfView(typeof(IDensityImagePlotStyleView))]
	public class DensityImagePlotStyleController : IMVCANController
	{
		private IDensityImagePlotStyleView _view;
		private DensityImagePlotStyle _doc;
		private UseDocument _useDocumentCopy;

		private IMVCANController _scaleController;
		private IMVCANController _colorProviderController;

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_scaleController = new DensityScaleController();
				_scaleController.InitializeDocument(_doc.Scale);

				_colorProviderController = new ColorProviderController();
				_colorProviderController.InitializeDocument(_doc.ColorProvider);
			}

			if (_view != null)
			{
				_scaleController.ViewObject = _view.DensityScaleView;
				_colorProviderController.ViewObject = _view.ColorProviderView;
				_view.ClipToLayer = _doc.ClipToLayer;
			}
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is DensityImagePlotStyle))
				return false;
			_doc = (DensityImagePlotStyle)args[0];
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

		#endregion IMVCANController Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IDensityImagePlotStyleView;

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

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply()
		{
			if (!_scaleController.Apply())
				return false;

			if (!_colorProviderController.Apply())
				return false;

			_doc.ClipToLayer = _view.ClipToLayer;
			_doc.Scale = (NumericalScale)_scaleController.ModelObject;
			_doc.ColorProvider = (IColorProvider)_colorProviderController.ModelObject;

			return true;
		}

		#endregion IApplyController Members
	}
}