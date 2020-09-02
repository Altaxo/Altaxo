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
using System.Windows.Controls;
using Altaxo.Calc.Regression.Multivariate;

namespace Altaxo.Gui.Worksheet
{
  /// <summary>
  /// Interaction logic for SpectralPreprocessingControl.xaml
  /// </summary>
  public partial class SpectralPreprocessingControl : UserControl, ISpectralPreprocessingView
  {
    public SpectralPreprocessingControl()
    {
      InitializeComponent();
    }

    private void _rbMethodNone_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (MethodChanged is not null)
        MethodChanged(SpectralPreprocessingMethod.None);
    }

    private void _rbMethodMSC_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (MethodChanged is not null)
        MethodChanged(SpectralPreprocessingMethod.MultiplicativeScatteringCorrection);
    }

    private void _rbMethodSNV_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (MethodChanged is not null)
        MethodChanged(SpectralPreprocessingMethod.StandardNormalVariate);
    }

    private void _rbMethod1stDer_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (MethodChanged is not null)
        MethodChanged(SpectralPreprocessingMethod.FirstDerivative);
    }

    private void _rbMethod2ndDer_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (MethodChanged is not null)
        MethodChanged(SpectralPreprocessingMethod.SecondDerivative);
    }

    private void _rbDetrendingNone_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (DetrendingChanged is not null)
        DetrendingChanged(-1);
    }

    private void _rbDetrendingZero_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (DetrendingChanged is not null)
        DetrendingChanged(0);
    }

    private void _rbDetrending1st_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (DetrendingChanged is not null)
        DetrendingChanged(1);
    }

    private void _rbDetrending2nd_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (DetrendingChanged is not null)
        DetrendingChanged(2);
    }

    private void _chkEnsembleScale_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (EnsembleScaleChanged is not null)
        EnsembleScaleChanged(true == _chkEnsembleScale.IsChecked);
    }

    #region ISpectralPreprocessingView

    public void InitializeMethod(SpectralPreprocessingMethod method)
    {
      switch (method)
      {
        case SpectralPreprocessingMethod.None:
          _rbMethodNone.IsChecked = true;
          break;

        case SpectralPreprocessingMethod.MultiplicativeScatteringCorrection:
          _rbMethodMSC.IsChecked = true;
          break;

        case SpectralPreprocessingMethod.StandardNormalVariate:
          _rbMethodSNV.IsChecked = true;
          break;

        case SpectralPreprocessingMethod.FirstDerivative:
          _rbMethod1stDer.IsChecked = true;
          break;

        case SpectralPreprocessingMethod.SecondDerivative:
          _rbMethod2ndDer.IsChecked = true;
          break;
      }
    }

    public void InitializeDetrending(int detrending)
    {
      switch (detrending)
      {
        case 0:
          _rbDetrendingZero.IsChecked = true;
          break;

        case 1:
          _rbDetrending1st.IsChecked = true;
          break;

        case 2:
          _rbDetrending2nd.IsChecked = true;
          break;

        default:
          _rbDetrendingNone.IsChecked = true;
          break;
      }
    }

    public void InitializeEnsembleScale(bool ensScale)
    {
      _chkEnsembleScale.IsChecked = ensScale;
    }

    public event Action<SpectralPreprocessingMethod> MethodChanged;

    public event Action<int> DetrendingChanged;

    public event Action<bool> EnsembleScaleChanged;

    #endregion ISpectralPreprocessingView
  }
}
