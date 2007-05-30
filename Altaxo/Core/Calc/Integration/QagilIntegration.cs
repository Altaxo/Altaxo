using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  public class QagilIntegration : IntegrationBase
  {
          #region offical C# interface
    gsl_integration_workspace _workSpace;
    gsl_integration_rule _integrationRule;


    public QagilIntegration()
    {
      _integrationRule = new QK15().Integrate;
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
       double b,
       double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return gsl_integration_qagil(f, b, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule);
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
          double b,
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f, b, epsabs, epsrel, limit, _integrationRule, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double b,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule q,
          out double result, out double abserr,
          ref object tempStorage)
    {
      QagilIntegration algo = tempStorage as QagilIntegration;
      if (null == algo)
        tempStorage = algo = new QagilIntegration();
      return algo.Integrate(f, b, epsabs, epsrel, limit, q, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double b,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      return Integration(f, b, epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }

    public static GSL_ERROR
   Integration(ScalarFunctionDD f,
      double b,
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule q,
     out double result, out double abserr
     )
    {
      object tempStorage = null;
      return Integration(f,  b, epsabs, epsrel, limit, q, out result, out abserr, ref tempStorage);
    }


    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
      double b,
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object tempStorage = null;
      return Integration(f, b, epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }


    #endregion

    /* QAGIL: Evaluate an integral over an infinite range using the
      transformation,
   
      integrate(f(x),-Inf,b) = integrate(f(b-(1-t)/t)/t^2,0,1)

      */

    struct il_params { public double b; public ScalarFunctionDD f; } ;

    //static double il_transform (double t, void *params);

    static GSL_ERROR
    gsl_integration_qagil(ScalarFunctionDD f,
                           double b,
                           double epsabs, double epsrel, int limit,
                           gsl_integration_workspace workspace,
                           out double result, out double abserr, gsl_integration_rule q)
    {
      //int status;

      // gsl_function f_transform;
      il_params transform_params = new il_params();
      transform_params.b = b;
      transform_params.f = f;

      //  f_transform.function = &il_transform;
      //  f_transform.params = &transform_params;

      ScalarFunctionDD f_transform = delegate(double t) { return il_transform(t, transform_params); };

      GSL_ERROR status = qags(f_transform, 0.0, 1.0,
                     epsabs, epsrel, limit,
                     workspace,
                     out result, out abserr,
                     q);

      return status;
    }

    static double
    il_transform(double t, il_params p)
    {
      //struct il_params *p = (struct il_params *) params;
      double b = p.b;
      ScalarFunctionDD f = p.f;
      double x = b - (1 - t) / t;
      double y = f(x);
      return (y / t) / t;
    }
  }
}
