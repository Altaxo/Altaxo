using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  public class QagiIntegration : IntegrationBase
  {
          #region offical C# interface
    gsl_integration_workspace _workSpace;
    gsl_integration_rule _integrationRule;


    public QagiIntegration()
    {
      _integrationRule = new QK15().Integrate;
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
        double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return gsl_integration_qagi(f, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule);
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f,  epsabs, epsrel, limit, _integrationRule, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule q,
          out double result, out double abserr,
          ref object tempStorage)
    {
      QagiIntegration algo = tempStorage as QagiIntegration;
      if (null == algo)
        tempStorage = algo = new QagiIntegration();
      return algo.Integrate(f,  epsabs, epsrel, limit, q, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      return Integration(f,  epsabs, epsrel, limit, new QK15().Integrate, out result, out abserr, ref tempStorage);
    }

    public static GSL_ERROR
   Integration(ScalarFunctionDD f,
     
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule q,
     out double result, out double abserr
     )
    {
      object tempStorage = null;
      return Integration(f,  epsabs, epsrel, limit, q, out result, out abserr, ref tempStorage);
    }


    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
      
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object tempStorage = null;
      return Integration(f,  epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }


    #endregion



    /* QAGI: evaluate an integral over an infinite range using the
      transformation

      integrate(f(x),-Inf,Inf) = integrate((f((1-t)/t) + f(-(1-t)/t))/t^2,0,1)

      */

    //static double i_transform (double t, void *params);

    static GSL_ERROR
    gsl_integration_qagi(ScalarFunctionDD f,
                          double epsabs, double epsrel, int limit,
                          gsl_integration_workspace workspace,
                          out double result, out double abserr,
                          gsl_integration_rule q
      )
    {
      //int status;

      //  gsl_function f_transform;
      //  f_transform.function = &i_transform;
      //  f_transform.params = f;

      ScalarFunctionDD f_transform = delegate(double t) { return i_transform(t, f); };

      GSL_ERROR status = qags(f_transform, 0.0, 1.0,
                     epsabs, epsrel, limit,
                     workspace,
                     out result, out abserr,
                     q);

      return status;
    }

    static double i_transform(double t, ScalarFunctionDD func)
    {
      double x = (1 - t) / t;
      double y = func(x) + func(-x);
      return (y / t) / t;
    }

  }
}
