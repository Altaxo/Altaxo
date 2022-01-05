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
using System.Windows;
using Altaxo.Units;

namespace Altaxo.Gui.Common.Drawing
{
  using Altaxo.Units;
  using AUL = Altaxo.Units.Length;

  public class LengthImageComboBox : DimensionfulQuantityImageComboBox
  {
    static LengthImageComboBox()
    {
      UnitEnvironmentProperty.OverrideMetadata(typeof(LengthImageComboBox), new FrameworkPropertyMetadata(SizeEnvironment.Instance));
      SelectedQuantityProperty.OverrideMetadata(typeof(LengthImageComboBox), new FrameworkPropertyMetadata(new DimensionfulQuantity(0, AUL.Point.Instance)));
    }

    /*
        public double SelectedQuantityAsValueInPoints
        {
            get { return SelectedQuantity.AsValueIn(AUL.Point.Instance); }
            set
            {
                var quant = new DimensionfulQuantity(value, AUL.Point.Instance);
                if (null != UnitEnvironment)
                    quant = quant.AsQuantityIn(UnitEnvironment.DefaultUnit);
                SelectedQuantity = quant;
            }
        }
        */

    #region Dependency property

    /// <summary>
    /// Gets/sets the quantity. The quantity consist of a numeric value together with a unit.
    /// </summary>
    public double SelectedQuantityAsValueInPoints
    {
      get
      {
        return SelectedQuantity.AsValueIn(AUL.Point.Instance);
      }
      set
      {
        SetValue(SelectedQuantityAsValueInPointsProperty, value);
      }
    }

    public static readonly DependencyProperty SelectedQuantityAsValueInPointsProperty =
        DependencyProperty.Register(nameof(SelectedQuantityAsValueInPoints),
          typeof(double), typeof(LengthImageComboBox),
        new FrameworkPropertyMetadata(EhSelectedQuantityAsValueInPointsChanged));

    private static void EhSelectedQuantityAsValueInPointsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((LengthImageComboBox)obj).OnSelectedQuantityAsValueInPointsChanged(obj, args);
    }

    protected override void OnSelectedQuantityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      base.OnSelectedQuantityChanged(obj, args);

      SelectedQuantityAsValueInPoints = SelectedQuantity.AsValueIn(AUL.Point.Instance);
    }

    protected virtual void OnSelectedQuantityAsValueInPointsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      var value = (double)args.NewValue;

      var quant = new DimensionfulQuantity(value, AUL.Point.Instance);
      if (UnitEnvironment is not null)
        quant = quant.AsQuantityIn(UnitEnvironment.DefaultUnit);
      SelectedQuantity = quant;
    }

    #endregion Dependency property
  }
}
