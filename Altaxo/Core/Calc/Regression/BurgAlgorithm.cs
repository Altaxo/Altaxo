using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Regression
{
  public class BurgAlgorithm
  {
    double[] _f;
    double[] _b;
    double[] _Ak;
  

    public static void Execution(IROVector x, IVector coefficients)
    {
      new BurgAlgorithm().Execute(x, coefficients);
    }

    public static void Execution(IROVector x, IVector coefficients, ref object tempStorage)
    {
      var burg = tempStorage as BurgAlgorithm;
      if (null == burg)
        tempStorage = burg = new BurgAlgorithm();
      burg.Execute(x, coefficients);
    }

    public void Execute(IROVector x, IVector coefficients)
    {
      Execute(x, coefficients, null);
    }

    public void Execute(IROVector x, IVector coefficients, IVector errors)
    {
      int N = x.Length - 1;
      int m = coefficients.Length;

      if (null == _Ak || _Ak.Length < (m + 1))
        _Ak = new double[m + 1];

      if (null==_f || _f.Length < (N+1))
      {
        _b = new double[N+1];
        _f = new double[N+1];
      }

      var Ak = _Ak;
      var f = _f;
      var b = _b;

      Ak[0] = 1;

      // Initialize forward and backward prediction errors with x
      for (int i = 0; i < f.Length; i++)
        f[i] = b[i] = x[i];

      double Dk = 0;

      for (int i = 0; i < f.Length; i++)
        Dk += 2 * f[i] * f[i];

      Dk -= f[0] * f[0] + b[N] * b[N];

      // Burg recursion

      for (int k = 0; k < m; k++)
      {
        // Compute mu
        double mu = 0;
        for (int n = 0; n < N - k; n++)
          mu += f[n + k + 1] * b[n];

        mu *= -2 / Dk;

        // Update Ak
        for (int n = 0; n <= (k + 1) / 2; n++)
        {
          double t1 = Ak[n] + mu * Ak[k + 1 - n];
          double t2 = Ak[k + 1 - n] + mu * Ak[n];
          Ak[n] = t1;
          Ak[k + 1 - n] = t2;
        }

        // Update f and b
        if (errors != null)
        {
          // update forward and backward predition error with simultaneous total error calculation
          double sumE = 0;
          for (int n = 0; n < N - k; n++)
          {
            double t1 = f[n + k + 1] + mu * b[n];
            double t2 = b[n] + mu * f[n + k + 1];
            f[n + k + 1] = t1;
            b[n] = t2;
            sumE += t1 * t1 + t2 * t2;
          }
          errors[k] = sumE / (2 * (N - k));
        }
        else 
        {
          // update forward and backward predition error without total error calculation
          for (int n = 0; n < N - k; n++)
          {
            double t1 = f[n + k + 1] + mu * b[n];
            double t2 = b[n] + mu * f[n + k + 1];
            f[n + k + 1] = t1;
            b[n] = t2;
          }
        }
        // Update Dk
        Dk = (1 - mu * mu) * Dk - f[k + 1] * f[k + 1] - b[N - k - 1] * b[N - k - 1];
      }

      // Assign coefficients
      for (int i = 0; i < m; i++)
        coefficients[i] = Ak[i+1];


    }

   
    

  }
}
