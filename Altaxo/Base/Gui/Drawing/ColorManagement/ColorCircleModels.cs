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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// Supports a Gui component for selecting hue values from a circle of colors.
  /// </summary>
  public interface IColorCircleModel
  {
    /// <summary>
    /// Gets the number of hue values that this circle model uses (the number of points that the user can directly or indirectly move).
    /// </summary>
    /// <value>
    /// The number of hue values.
    /// </value>
    int NumberOfHueValues { get; }

    /// <summary>
    /// Sets the initial hue values of this circle model.
    /// </summary>
    /// <param name="hueOfButtons">The array of hue values. On return, this array must be filled with the initial hue values.</param>
    void SetInitialHueValues(double[] hueOfButtons);

    /// <summary>
    /// Try to set the hue of a button.
    /// </summary>
    /// <param name="indexOfMovedButton">The index of the button that should be moved.</param>
    /// <param name="hue">The hue value that the mouse indicates.</param>
    /// <param name="hueOfButtons">The array of hue values. On return, this array should be updated to reflect the state of the moved button, and maybe, also other button that are
    /// influenced by the hue value.</param>
    void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons);
  }

  public class ColorCircleModelHelperBase
  {
    protected static double BringInbetween0To1(double x)
    {
      if (x < 0)
      {
        while (x < 0)
          x += 1;
      }
      else if (x > 1)
      {
        while (x > 1)
          x -= 1;
      }
      return x;
    }

    protected static double BringInbetweenOrToMinimumDistanceFrom(double x, double min, double max)
    {
      if (!(min <= max))
        throw new ArgumentOutOfRangeException(nameof(max), "max must be >= min");

      if (x < min)
      {
        while (x < min)
          x += 1;
      }
      else if (x > max)
      {
        while (x > max)
          x -= 1;
      }

      if (x >= min && x <= max)
      {
        return x; // condition "inbetween" fullfilled
      }
      else
      {
        // if we can not fullfil the condition, bring x either to min or to max
        var distToMin = Math.Min(Math.Abs(x - min), Math.Min(Math.Abs(x - 1 - min), Math.Abs(x + 1 - min)));
        var distToMax = Math.Min(Math.Abs(x - max), Math.Min(Math.Abs(x - 1 - max), Math.Abs(x + 1 - max)));

        return distToMin < distToMax ? min : max;
      }
    }

    protected static double BringToGreaterThanOrEqualTo(double x, double y)
    {
      while (x < y)
        x += 1;

      return x;
    }
  }

  public class ColorCircleModelPrimary : ColorCircleModelHelperBase, IColorCircleModel
  {
    public int NumberOfHueValues { get { return 1; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double phi, double[] hueOfButtons)
    {
      hueOfButtons[0] = BringInbetween0To1(phi);
    }
  }

  public class ColorCircleModelComplementary : ColorCircleModelHelperBase, IColorCircleModel
  {
    public int NumberOfHueValues { get { return 2; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
      hueOfButtons[1] = 0.5;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton)
      {
        hueOfButtons[0] = BringInbetween0To1(hue);
        hueOfButtons[1] = BringInbetween0To1(hue + 0.5);
      }
      else
      {
        hueOfButtons[0] = BringInbetween0To1(hue + 0.5);
        hueOfButtons[1] = BringInbetween0To1(hue);
      }
    }
  }

  public class ColorCircleModelTriangleComplementary : ColorCircleModelHelperBase, IColorCircleModel
  {
    public int NumberOfHueValues { get { return 3; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
      hueOfButtons[1] = 0.5;
      hueOfButtons[2] = 0.75;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton || 1 == indexOfMovedButton)
      {
        var delta_phi = hue - hueOfButtons[indexOfMovedButton];

        for (int i = 0; i < NumberOfHueValues; ++i)
        {
          hueOfButtons[i] = BringInbetween0To1(hueOfButtons[i] + delta_phi);
        }
      }
      else
      {
        // i == 2 is freely moveable
        hueOfButtons[indexOfMovedButton] = BringInbetween0To1(hue);
      }
    }
  }

  public class ColorCircleModelTriangleSymmetrically : ColorCircleModelHelperBase, IColorCircleModel
  {
    public int NumberOfHueValues { get { return 3; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
      hueOfButtons[1] = 1 / 3.0;
      hueOfButtons[2] = 2 / 3.0;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton)
      {
        var delta_phi = hue - hueOfButtons[indexOfMovedButton];

        for (int i = 0; i < NumberOfHueValues; ++i)
        {
          hueOfButtons[i] = BringInbetween0To1(hueOfButtons[i] + delta_phi);
        }
      }
      else
      {
        var hueMiddle = hueOfButtons[0] + 0.5;

        double hue_prev, hue_next;

        if (1 == indexOfMovedButton)
        {
          hue_prev = hueOfButtons[0];
          hue_next = hueMiddle;
        }
        else
        {
          hue_prev = hueMiddle;
          hue_next = hueOfButtons[0] + 1;
        }

        // readjust hue_next and hue_prev;
        hue_next -= 1.0 / 128;
        hue_prev += 1.0 / 128;

        double phi_curr = BringInbetweenOrToMinimumDistanceFrom(hue, hue_prev, hue_next);

        // calculate delta to hueMiddle
        var delta = hueMiddle - phi_curr;

        hueOfButtons[indexOfMovedButton] = BringInbetween0To1(hueMiddle - delta);
        hueOfButtons[1 == indexOfMovedButton ? 2 : 1] = BringInbetween0To1(hueMiddle + delta);
      }
    }
  }

  public class ColorCircleModelRectangle : ColorCircleModelHelperBase, IColorCircleModel
  {
    private double[] _hueTransformed = new double[4];

    public int NumberOfHueValues { get { return 4; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
      hueOfButtons[1] = 1 / 4.0;
      hueOfButtons[2] = 2 / 4.0;
      hueOfButtons[3] = 3 / 4.0;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton || 2 == indexOfMovedButton)
      {
        var delta_phi = hue - hueOfButtons[indexOfMovedButton];

        for (int i = 0; i < NumberOfHueValues; ++i)
        {
          hueOfButtons[i] = BringInbetween0To1(hueOfButtons[i] + delta_phi);
        }
      }
      else // either button 1 or 3
      {
        // Transform it so that button 0 -> hue = 0 and button 2 -> hue = 0.5

        for (int i = 0; i < NumberOfHueValues; ++i)
          _hueTransformed[i] = BringInbetween0To1(hueOfButtons[i] - hueOfButtons[0]);
        double phi_Transformed = BringInbetween0To1(hue - hueOfButtons[0]);

        // from now on we calculate using the transformed values
        // we also transform the moved button, so that when we really move button 4, we virtually move button 1
        // and when we really move button 3, we virtually move button 2
        if (indexOfMovedButton == 3)
        {
          phi_Transformed = BringInbetween0To1(phi_Transformed - 0.5); // if it is button 3, transform it to the hue value of button 1
          indexOfMovedButton = 1;
        }

        // from now on we need only to deal with moving button 1, and button 1 can only be moved between 0 and 0.5

        var hue_prev = 0.0;
        var hue_next = 0.5;

        // readjust hue_next and hue_prev;
        var diff = hue_next - hue_prev;
        hue_next -= diff / 128;
        hue_prev += diff / 128;

        if (!(hue_next > hue_prev))
          throw new InvalidProgramException();

        double phi_curr = BringInbetweenOrToMinimumDistanceFrom(phi_Transformed, hue_prev, hue_next);

        _hueTransformed[1] = BringInbetween0To1(phi_curr);
        _hueTransformed[2] = 0.5; // correct small rounding errors
        _hueTransformed[3] = BringInbetween0To1(0.5 + phi_curr);

        // back-transform hue values
        for (int i = 1; i < NumberOfHueValues; ++i)
          hueOfButtons[i] = BringInbetween0To1(_hueTransformed[i] + hueOfButtons[0]);
      }
    }
  }

  public class ColorCircleModelPentagonSymmetrically : ColorCircleModelHelperBase, IColorCircleModel
  {
    private double[] _hueTransformed = new double[5];

    public int NumberOfHueValues { get { return 5; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      hueOfButtons[0] = 0;
      hueOfButtons[1] = 1 / 5.0;
      hueOfButtons[2] = 2 / 5.0;
      hueOfButtons[3] = 3 / 5.0;
      hueOfButtons[4] = 4 / 5.0;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton)
      {
        var delta_phi = hue - hueOfButtons[indexOfMovedButton];

        for (int i = 0; i < NumberOfHueValues; ++i)
        {
          hueOfButtons[i] = BringInbetween0To1(hueOfButtons[i] + delta_phi);
        }
      }
      else
      {
        for (int i = 0; i < NumberOfHueValues; ++i)
          _hueTransformed[i] = BringInbetween0To1(hueOfButtons[i] - hueOfButtons[0]);
        double phi_Transformed = BringInbetween0To1(hue - hueOfButtons[0]);

        // from now on we calculate using the transformed values
        // we also transform the moved button, so that when we really move button 4, we virtually move button 1
        // and when we really move button 3, we virtually move button 2
        if (indexOfMovedButton == 3 || indexOfMovedButton == 4)
        {
          phi_Transformed = BringInbetween0To1(-phi_Transformed); // mirror on x-axis
          indexOfMovedButton = indexOfMovedButton == 3 ? 2 : 1;
        }

        // from now on we need only to deal with moving button 1 or 2
        var hueMiddle = 0.5;

        double hue_prev, hue_next;

        switch (indexOfMovedButton)
        {
          case 1:
            hue_prev = 0;
            hue_next = _hueTransformed[2];
            break;

          case 2:
            hue_prev = _hueTransformed[1];
            hue_next = hueMiddle;
            break;

          default:
            throw new InvalidProgramException();
        }

        // readjust hue_next and hue_prev;
        var diff = hue_next - hue_prev;
        hue_next -= diff / 128;
        hue_prev += diff / 128;

        if (!(hue_next > hue_prev))
          throw new InvalidProgramException();

        double phi_curr = BringInbetweenOrToMinimumDistanceFrom(phi_Transformed, hue_prev, hue_next);

        // calculate delta to hueMiddle
        var delta = hueMiddle - phi_curr;

        if (1 == indexOfMovedButton)
        {
          _hueTransformed[1] = BringInbetween0To1(hueMiddle - delta);
          _hueTransformed[4] = BringInbetween0To1(hueMiddle + delta);
        }
        else if (2 == indexOfMovedButton)
        {
          var hueTransformed2 = _hueTransformed[2];
          _hueTransformed[2] = BringInbetween0To1(hueMiddle - delta);
          _hueTransformed[3] = BringInbetween0To1(hueMiddle + delta);

          // here move buttons 1 and 4 relativ to button 0
          var delta1 = (_hueTransformed[2] - hueTransformed2) * _hueTransformed[1] / hueTransformed2;

          _hueTransformed[1] += delta1;
          _hueTransformed[4] -= delta1;
        }

        // back-transform hue values
        for (int i = 1; i < NumberOfHueValues; ++i)
          hueOfButtons[i] = BringInbetween0To1(_hueTransformed[i] + hueOfButtons[0]);
      }
    }
  }

  internal class ColorCircleModelFreeColors : ColorCircleModelHelperBase // , IColorCircleModel
  {
    private int _numberOfHueValues = 2;

    public int NumberOfHueValues { get { return _numberOfHueValues; } }

    public void SetInitialHueValues(double[] hueOfButtons)
    {
      for (int i = 0; i < _numberOfHueValues; ++i)
        hueOfButtons[i] = i / (double)_numberOfHueValues;
    }

    public void TrySetHueOfButton(int indexOfMovedButton, double hue, double[] hueOfButtons)
    {
      if (0 == indexOfMovedButton)
      {
        var delta_phi = hue - hueOfButtons[0];

        for (int i = 0; i < _numberOfHueValues; ++i)
        {
          hueOfButtons[i] = BringInbetween0To1(hueOfButtons[i] + delta_phi);
        }
      }
      else
      {
        int i_prev = indexOfMovedButton - 1;
        int i_next = indexOfMovedButton == (_numberOfHueValues - 1) ? 0 : indexOfMovedButton + 1;

        var phi_prev = BringToGreaterThanOrEqualTo(hueOfButtons[i_prev], hueOfButtons[0]);
        var phi_curr = BringToGreaterThanOrEqualTo(hue, hueOfButtons[0]);
        var phi_next = BringToGreaterThanOrEqualTo(hueOfButtons[i_next], phi_prev);

        if (phi_curr < phi_prev)
          phi_curr = phi_prev;
        else if (phi_curr > phi_next)
          phi_curr = phi_next;

        hueOfButtons[indexOfMovedButton] = BringInbetween0To1(phi_curr);
      }
    }
  }
}
