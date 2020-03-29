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

using System.Drawing;
using Altaxo.Drawing;

namespace Altaxo.Graph
{
  public interface IHatchBrushTexture : ISyntheticRepeatableTexture
  {
    /// <summary>
    /// Gets an image of the texture. The image dimensions (pixels in x and y direction) are calculated using the provided <paramref name=" maxEffectiveResolutionDpi">maximum effective resolution.</paramref>.
    /// </summary>
    /// <param name="maxEffectiveResolutionDpi">Effective resolution used for later drawing of this image. The higher the resolution, the more pixels are allocated for the bitmap.</param>
    /// <param name="foreColor">Foreground color of the hatch brush.</param>
    /// <param name="backColor">Background color of the hatch brush.</param>
    /// <returns>The image of the texture.</returns>
    System.IO.Stream GetContentStream(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor);
  }
}
