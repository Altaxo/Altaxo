#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	/// <summary>
	/// Implements a model which can split or express a color in components. The components are provided as text.
	/// </summary>
	public interface ITextOnlyColorModel
	{
		/// <summary>
		/// Gets the names of components of this color model.
		/// </summary>
		/// <returns>Names of components of this color model.</returns>
		string[] GetNamesOfComponents();

		/// <summary>
		/// Gets the components (in text form) for the provided color.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="formatProvider">The format provider.</param>
		/// <returns>The components (in text form) for the provided color.</returns>
		string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider);
	}

	/// <summary>
	/// Gets the color model to be used with <see cref="ColorModelController"/>.
	/// </summary>
	public interface IColorModel
	{
		/// <summary>
		/// Gets the color for the 1D color surface from the relative position. The position 0 designates the lower position,
		/// the position 1 the upper position.
		/// </summary>
		/// <param name="relativePosition">The relative position. The value has to be in the range [0, 1].</param>
		/// <returns>The color at the relative position</returns>
		AxoColor GetColorFor1DColorSurfaceFromRelativePosition(double relativePosition);

		/// <summary>
		/// Gets the color for the 2D color surface from the relative position. The meaning is defined so that
		/// the argument (1, 1) returns the <paramref name="baseColor"/>, whereas (0,0) returns black.
		/// </summary>
		/// <param name="relativePosition">The relative position. Both components have to be in the range [0, 1].</param>
		/// <param name="baseColor">Base color, i.e. the color returned from the 1D color surface.</param>
		/// <returns></returns>
		AxoColor GetColorFor2DColorSurfaceFromRelativePosition(PointD2D relativePosition, AxoColor baseColor);

		(double position1D, PointD2D position2D) GetRelativePositionsFor1Dand2DColorSurfaceFromColor(AxoColor color);

		/// <summary>
		/// Gets the names of the color components of this model
		/// </summary>
		/// <returns>Names of the color components of this model.</returns>
		string[] GetNamesOfComponents();

		double[] GetComponentsForColor(AxoColor color);

		AxoColor GetColorFromComponents(double[] components);

		/// <summary>
		/// Gets a value indicating whether this instance is using byte components.
		/// </summary>
		/// <value>
		///  If <c>true</c>, this instance is using byte components (0..255); otherwise, if<c>false</c>, the model is using norm components (0..1).
		/// </value>
		bool IsUsingByteComponents { get; }
	}
}