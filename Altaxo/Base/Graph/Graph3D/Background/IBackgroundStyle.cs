#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;

namespace Altaxo.Graph.Graph3D.Background
{
  /// <summary>
  /// Background for some items, mainly intended for text.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IDocumentLeafNode" />
  /// <seealso cref="System.ICloneable" />
  public interface IBackgroundStyle : Main.IDocumentLeafNode, ICloneable
  {
    /// <summary>
    /// Measures the background size and position.
    /// </summary>
    /// <param name="itemRectangle">Position and size of the item for which this background is intended. For text, this is the position and size of the text rectangle, already with a margin around.</param>
    /// <returns>The position and size of the rectangle that fully includes the background (but not the item).</returns>
    RectangleD3D Measure(RectangleD3D itemRectangle);

    /// <summary>
    /// Draws the specified background
    /// </summary>
    /// <param name="g">The drawing context.</param>
    /// <param name="itemRectangle">Position and size of the item for which this background is intended. For text, this is the position and size of the text rectangle, already with a margin around.
    /// This parameter should have the same size as was used in the previous call to <see cref="Measure(RectangleD3D)"/></param>
    void Draw(IGraphicsContext3D g, RectangleD3D itemRectangle);

    /// <summary>
    /// Draws the specified background
    /// </summary>
    /// <param name="g">The drawing context.</param>
    /// <param name="itemRectangle">Position and size of the item for which this background is intended. For text, this is the position and size of the text rectangle, already with a margin around.
    /// <param name="overrideMaterial">Draw the background not with its own material, but with the material specified in this parameter.</param>
    /// This parameter should have the same size as was used in the previous call to <see cref="Measure(RectangleD3D)"/></param>
    void Draw(IGraphicsContext3D g, RectangleD3D itemRectangle, IMaterial overrideMaterial);

    /// <summary>
    /// Gets or sets the material used to draw the background.
    /// </summary>
    /// <value>
    /// The material.
    /// </value>
    IMaterial Material { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this background supports user defined material. For some background classes, the
    /// kind of material may be fixed, for instance a black background. In this case the value is false. For other backgrounds, you
    /// are free to chose the material, in this case the value is true.
    /// </summary>
    /// <value>
    /// <c>true</c> if this background allows to chose the material;  otherwise <c>false</c>.
    /// </value>
    bool SupportsUserDefinedMaterial { get; set; }

    /// <summary>
    /// Gets or sets the user defined background thickness. If this value is null, the background class has to find an appropriate thickness in the <see cref="Measure"/> step by itself.
    /// </summary>
    /// <value>
    /// The user defined thickness.
    /// </value>
    double? Thickness { get; set; }

    /// <summary>
    /// Gets or sets the user defined distance. If this value is null, the background class has to find an appropriate distance in the <see cref="Measure"/> step by itself.
    /// </summary>
    /// <value>
    /// The user defined distance.
    /// </value>
    double? Distance { get; set; }
  }
}
