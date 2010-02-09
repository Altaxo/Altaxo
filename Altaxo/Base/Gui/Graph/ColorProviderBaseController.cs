#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Drawing;

using Altaxo.Serialization;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Gui;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	/// <summary>
	/// Interface for Gui elements that show the properties of the <see cref="ColorProviderBase"/> class.
	/// </summary>
	public interface IColorProviderBaseView
	{
		/// <summary>
		/// Get/set the content of the ColorBelow combo box.
		/// </summary>
		Color ColorBelow { get; set; }

		/// <summary>
		/// IGet/set the content of the ColorAbove combo box.
		/// </summary>
		Color ColorAbove { get; set; }

		/// <summary>
		/// Get/set the content of the ColorInvalid combo box.
		/// </summary>
		Color ColorInvalid { get; set; }

		/// <summary>
		/// Get/set the transparency value (0 .. 1).
		/// </summary>
		double Transparency { get; set; }

		/// <summary>
		/// Get/set the ColorSteps value (0..).
		/// </summary>
		int ColorSteps { get; set; }

	}
	#endregion

	[ExpectedTypeOfView(typeof(IColorProviderBaseView))]
	[UserControllerForObject(typeof(ColorProviderBase))]
	public class ColorProviderBaseController : IMVCANController
	{
		ColorProviderBase _originalDoc;
		ColorProviderBase _doc;
		IColorProviderBaseView _view;

		void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.ColorBelow = _doc.ColorBelow;
				_view.ColorAbove = _doc.ColorAbove;
				_view.ColorInvalid = _doc.ColorInvalid;
				_view.Transparency = _doc.Transparency;
				_view.ColorSteps = _doc.ColorSteps;
			}
		}


		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args.Length == 0 || !(args[0] is ColorProviderBase))
				return false;

			_originalDoc = (ColorProviderBase)args[0];
			_doc = (ColorProviderBase)_originalDoc.Clone();
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		#endregion

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
				}

				_view = value as IColorProviderBaseView;

				if (null != _view)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			_originalDoc.ColorBelow = _view.ColorBelow;
			_originalDoc.ColorAbove = _view.ColorAbove;
			_originalDoc.ColorInvalid = _view.ColorInvalid;
			_originalDoc.Transparency = _view.Transparency;
			_originalDoc.ColorSteps = _view.ColorSteps;
			return true;
		}

		#endregion
	}
}
