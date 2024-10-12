#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Altaxo.Calc;

namespace Altaxo.Serialization.WITec
{
  public class TDSpectralTransformationClass : TDTransformationClass
  {
    protected WITecTreeNode _tdSpectralTransformation;

    public TDSpectralTransformationClass(WITecTreeNode node, WITecReader reader)
      : base(node, reader)
    {
      _tdSpectralTransformation = node.GetChild("TDSpectralTransformation");
    }

    public IEnumerable<double> Transform(IEnumerable<double> values)
    {
      var transformationType = _tdSpectralTransformation.GetData<int>("SpectralTransformationType");

      switch (transformationType)
      {
        case 0: // Polynom (2nd order)
          {
            var coefficients = _tdSpectralTransformation.GetData<double[]>("Polynom");
            foreach (var value in values)
            {
              yield return RMath.EvaluatePolynomOrderAscending(value, coefficients);
            }
          }
          break;
        case 1: // spectrometer grating
          {
            var lambdaC = _tdSpectralTransformation.GetData<double>("LambdaC");
            var m = _tdSpectralTransformation.GetData<double>("m");
            var d = _tdSpectralTransformation.GetData<double>("d");
            var gamma = _tdSpectralTransformation.GetData<double>("Gamma");
            var f = _tdSpectralTransformation.GetData<double>("f");
            var delta = _tdSpectralTransformation.GetData<double>("Delta");
            var x = _tdSpectralTransformation.GetData<double>("x");
            var nC = _tdSpectralTransformation.GetData<double>("nC");

            var alpha = Math.Asin(lambdaC * m / d / 2 / Math.Cos(gamma / 2)) - gamma / 2;
            var lh = f * Math.Cos(delta);
            var hc = f * Math.Sin(delta);
            var betac = gamma + alpha;
            var betah = betac - delta;

            foreach (var value in values)
            {
              var hi = x * (nC - value) - hc;
              var betai = betah - Math.Atan2(hi, lh);
              yield return d / m * (Math.Sin(alpha) + Math.Sin(betai));
            }
          }
          break;
        case 2: // Free polynom
          {
            var order = _tdSpectralTransformation.GetData<Int32>("FreePolynomOrder");
            var coefficients = _tdSpectralTransformation.GetData<double[]>("FreePolynom");
            var startBin = (int)_tdSpectralTransformation.GetData<double>("FreePolynomStartBin");
            var stopBin = (int)_tdSpectralTransformation.GetData<double>("FreePolynomStopBin");

            var result = values.ToArray();
            stopBin = Math.Max(startBin, stopBin);
            startBin = Math.Max(startBin, 0);
            stopBin = Math.Min(stopBin, result.Length - 1);

            for (int i = startBin; i <= stopBin; ++i)
            {
              result[i] = RMath.EvaluatePolynomOrderAscending(result[i], coefficients);
            }
            for (int i = 0; i < startBin; ++i)
            {
              result[i] = result[startBin];
            }
            for (int i = stopBin + 1; i < result.Length; ++i)
            {
              result[i] = result[stopBin];
            }
            foreach (var value in result)
            {
              yield return value;
            }
          }
          break;
        default:
          throw new NotImplementedException($"The transformation type {transformationType} is not yet implemented.");
      }
    }
  }


}
