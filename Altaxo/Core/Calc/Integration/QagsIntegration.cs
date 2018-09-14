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
using System.Text;

namespace Altaxo.Calc.Integration
{
  /// <summary>
  /// Adaptive integration with singularities.
  /// </summary>
  /// <remarks>
  /// The presence of an integrable singularity in the integration region causes an adaptive routine
  /// to concentrate new subintervals around the singularity. As the subintervals decrease in size
  /// the successive approximations to the integral converge in a limiting fashion. This approach
  /// to the limit can be accelerated using an extrapolation procedure. The QAGS algorithm
  /// combines adaptive bisection with the Wynn epsilon-algorithm to speed up the integration
  /// of many types of integrable singularities.
  /// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
  /// </remarks>
  public class QagsIntegration : IntegrationBase
  {
    #region offical C# interface

    private bool _debug;
    private gsl_integration_workspace _workSpace;
    private gsl_integration_rule _integrationRule;

    /// <summary>
    /// Returns the default integration rule used for this class.
    /// </summary>
    public static gsl_integration_rule DefaultIntegrationRule
    {
      get
      {
        return new QK21().Integrate;
      }
    }

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and default debug flag setting.
    /// </summary>
    public QagsIntegration()
      : this(DefaultIntegrationRule, DefaultDebugFlag)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with a default integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QagsIntegration(bool debug)
      : this(DefaultIntegrationRule, debug)
    {
    }

    /// <summary>
    /// Creates an instance of this integration class with specified integration rule and specified debug flag setting.
    /// </summary>
    /// <param name="integrationRule">Integration rule used for integration.</param>
    /// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    public QagsIntegration(gsl_integration_rule integrationRule, bool debug)
    {
      _integrationRule = integrationRule;
      _debug = debug;
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR Integrate(Func<double, double> f,
       double a, double b,
       double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule, bool debug,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return qags(f, a, b, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule, debug);
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities using the integration rule and debug setting given in the constructor..
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public GSL_ERROR Integrate(Func<double, double> f,
          double a, double b,
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f, a, b, epsabs, epsrel, limit, _integrationRule, _debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <param name="tempStorage">Provides a temporary storage object that you can reuse for repeating function calls.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR
    Integration(Func<double, double> f,
          double a, double b,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule integrationRule, bool debug,
          out double result, out double abserr,
          ref object tempStorage)
    {
      var algo = tempStorage as QagsIntegration;
      if (null == algo)
        tempStorage = algo = new QagsIntegration(integrationRule, debug);
      return algo.Integrate(f, a, b, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <param name="tempStorage">Provides a temporary storage object that you can reuse for repeating function calls.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR
    Integration(Func<double, double> f,
          double a, double b,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      var algo = tempStorage as QagsIntegration;
      if (null == algo)
        tempStorage = algo = new QagsIntegration();
      return algo.Integrate(f, a, b, epsabs, epsrel, limit, out result, out abserr);
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="integrationRule">Integration rule used for integration (only for this function call).</param>
    /// <param name="debug">Setting of the debug flag (only for this function call). If the integration fails or the specified accuracy
    /// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
    /// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR
   Integration(Func<double, double> f,
     double a, double b,
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule integrationRule, bool debug,
     out double result, out double abserr
     )
    {
      object tempStorage = null;
      return Integration(f, a, b, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr, ref tempStorage);
    }

    /// <summary>
    /// Adaptive integration with (unknown) singularities.
    /// </summary>
    /// <param name="f">Function to integrate.</param>
    /// <param name="a">Lower integration limit.</param>
    /// <param name="b">Upper integration limit.</param>
    /// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
    /// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
    /// <param name="limit">Maximum number of subintervals used for integration.</param>
    /// <param name="result">On return, contains the integration result.</param>
    /// <param name="abserr">On return, contains the absolute error of integration.</param>
    /// <returns>Null if successfull, otherwise the appropriate error code.</returns>
    public static GSL_ERROR
    Integration(Func<double, double> f,
      double a, double b,
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object tempStorage = null;
      return Integration(f, a, b, epsabs, epsrel, limit, out result, out abserr, ref tempStorage);
    }

    #endregion offical C# interface
  }
}
