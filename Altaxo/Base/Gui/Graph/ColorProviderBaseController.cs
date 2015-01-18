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
	using Altaxo.Graph;

	#region Interfaces

	/// <summary>
	/// Interface for Gui elements that show the properties of the <see cref="ColorProviderBase"/> class.
	/// </summary>
	public interface IColorProviderBaseView
	{
		/// <summary>
		/// Get/set the content of the ColorBelow combo box.
		/// </summary>
		NamedColor ColorBelow { get; set; }

		/// <summary>
		/// IGet/set the content of the ColorAbove combo box.
		/// </summary>
		NamedColor ColorAbove { get; set; }

		/// <summary>
		/// Get/set the content of the ColorInvalid combo box.
		/// </summary>
		NamedColor ColorInvalid { get; set; }

		/// <summary>
		/// Get/set the transparency value (0 .. 1).
		/// </summary>
		double Transparency { get; set; }

		/// <summary>
		/// Get/set the ColorSteps value (0..).
		/// </summary>
		int ColorSteps { get; set; }

		/// <summary>Is called when any of the user choices of this control changed. Intended for updating the preview when something changed.</summary>
		event Action ChoiceChanged;
	}

	#endregion Interfaces

	[ExpectedTypeOfView(typeof(IColorProviderBaseView))]
	[UserControllerForObject(typeof(ColorProviderBase))]
	public class ColorProviderBaseController : MVCANDControllerBase<ColorProviderBase, IColorProviderBaseView>
	{
		protected override void Initialize(bool initData)
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

		protected override void AttachView()
		{
			base.AttachView();
			_view.ChoiceChanged += OnMadeDirty;
		}

		protected override void DetachView()
		{
			_view.ChoiceChanged -= OnMadeDirty;
			base.DetachView();
		}

		public override bool Apply(bool disposeController)
		{
			_originalDoc.ColorBelow = _view.ColorBelow;
			_originalDoc.ColorAbove = _view.ColorAbove;
			_originalDoc.ColorInvalid = _view.ColorInvalid;
			_originalDoc.Transparency = _view.Transparency;
			_originalDoc.ColorSteps = _view.ColorSteps;

			return true;
		}
	}
}