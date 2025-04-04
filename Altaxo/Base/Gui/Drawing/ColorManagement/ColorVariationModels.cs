﻿#region Copyright

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

#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Interface to a model that designates how to make variations of a base color.
  /// </summary>
  public interface IColorVariationModel
  {
    /// <summary>
    /// Gets the color variations.
    /// </summary>
    /// <param name="baseColor">The base color used to get variations.</param>
    /// <param name="variations">The variations array. On return, this array must be filled with the color variations. The item at index 0 is always the base color.</param>
    void GetColorVariations(AxoColor baseColor, AxoColor[] variations);
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.ColorVariationModelDesaturate}")]
  public class ColorVariationModelDesaturate : IColorVariationModel
  {
    public void GetColorVariations(AxoColor baseColor, AxoColor[] variations)
    {
      var (alpha, hue, saturation, brightness) = baseColor.ToAhsb();

      // desaturate, but still the last item should have a rest of saturation left

      int numberOfItems = variations.Length;

      variations[0] = baseColor;
      for (int i = 1; i < numberOfItems; ++i)
      {
        var newSaturation = saturation * (1 - i / (float)numberOfItems);
        variations[i] = AxoColor.FromAhsb(alpha, hue, newSaturation, brightness);
      }
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.ColorVariationModelDesaturateAndDarker}")]
  public class ColorVariationModelDesaturateAndDarker : IColorVariationModel
  {
    public void GetColorVariations(AxoColor baseColor, AxoColor[] variations)
    {
      var (alpha, hue, saturation, brightness) = baseColor.ToAhsb();

      // desaturate, but still the last item should have a rest of saturation left

      int numberOfItems = variations.Length;

      variations[0] = baseColor;
      for (int i = 1; i < numberOfItems; ++i)
      {
        var newSaturation = saturation * (1 - i / (float)numberOfItems);
        var newBrightness = brightness * (1 - i / (float)numberOfItems);
        variations[i] = AxoColor.FromAhsb(alpha, hue, newSaturation, newBrightness);
      }
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.ColorVariationModelDarker}")]
  public class ColorVariationModelDarker : IColorVariationModel
  {
    public void GetColorVariations(AxoColor baseColor, AxoColor[] variations)
    {
      var (alpha, hue, saturation, brightness) = baseColor.ToAhsb();

      // make darker, but still the last item should have a rest of color left (i.e. it should not be completely black)

      int numberOfItems = variations.Length;

      variations[0] = baseColor;
      for (int i = 1; i < numberOfItems; ++i)
      {
        var newBrightness = brightness * (1 - i / (float)numberOfItems);
        variations[i] = AxoColor.FromAhsb(alpha, hue, saturation, newBrightness);
      }
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.ColorVariationModelWarmer}")]
  public class ColorVariationModelWarmer : IColorVariationModel
  {
    public void GetColorVariations(AxoColor baseColor, AxoColor[] variations)
    {
      var red = baseColor.ScR;
      var blue = baseColor.ScB;

      // make warmer = more red

      int numberOfItems = variations.Length;

      variations[0] = baseColor;
      for (int i = 1; i < numberOfItems; ++i)
      {
        var newRed = (red + (1 - red) * (i / (float)numberOfItems));
        var newBlue = (blue * (1 - i / (float)numberOfItems));
        variations[i] = AxoColor.FromScRgb(baseColor.ScA, newRed, baseColor.ScG, newBlue);
      }
    }
  }

  [DisplayName("${res:ClassNames.Altaxo.Gui.Drawing.ColorManagement.ColorVariationModelColder}")]
  public class ColorVariationModelColder : IColorVariationModel
  {
    public void GetColorVariations(AxoColor baseColor, AxoColor[] variations)
    {
      var blue = baseColor.ScB;
      var red = baseColor.ScR;

      // make warmer = more red

      int numberOfItems = variations.Length;

      variations[0] = baseColor;
      for (int i = 1; i < numberOfItems; ++i)
      {
        var newBlue = (blue + (1 - blue) * (i / (float)numberOfItems));
        var newRed = (red * (1 - i / (float)numberOfItems));
        variations[i] = AxoColor.FromScRgb(baseColor.ScA, newRed, baseColor.ScG, newBlue);
      }
    }
  }
}
