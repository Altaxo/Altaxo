using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  public class QagiuIntegration : IntegrationBase
  {
          #region offical C# interface
    gsl_integration_workspace _workSpace;
    gsl_integration_rule _integrationRule;


    public QagiuIntegration()
    {
      _integrationRule = DefaultIntegrationRule;
    }

    public static gsl_integration_rule DefaultIntegrationRule
    {
      get
      {
        return new QK15().Integrate;
      }
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
       double a, 
       double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return gsl_integration_qagiu(f, a, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule);
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
          double a, 
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f, a, epsabs, epsrel, limit, _integrationRule, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double a, 
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule q,
          out double result, out double abserr,
          ref object tempStorage)
    {
      QagiuIntegration algo = tempStorage as QagiuIntegration;
      if (null == algo)
        tempStorage = algo = new QagiuIntegration();
      return algo.Integrate(f, a, epsabs, epsrel, limit, q, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double a, 
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      return Integration(f, a,  epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }

    public static GSL_ERROR
   Integration(ScalarFunctionDD f,
     double a, 
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule q,
     out double result, out double abserr
     )
    {
      object tempStorage = null;
      return Integration(f, a, epsabs, epsrel, limit, q, out result, out abserr, ref tempStorage);
    }


    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
      double a, 
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object tempStorage = null;
      return Integration(f, a,  epsabs, epsrel, limit, new QK15().Integrate, out result, out abserr, ref tempStorage);
    }


    #endregion

    /* QAGIU: Evaluate an integral over an infinite range using the
      transformation

      integrate(f(x),a,Inf) = integrate(f(a+(1-t)/t)/t^2,0,1)

      */

    struct iu_params { public double a; public ScalarFunctionDD f; } ;

    //static double iu_transform (double t, void *params);

    internal static GSL_ERROR
    gsl_integration_qagiu(ScalarFunctionDD f,
                           double a,
                           double epsabs, double epsrel, int limit,
                           gsl_integration_workspace workspace,
                           out double result, out double abserr,
                            gsl_integration_rule q
      )
    {
      //int status;

      //  gsl_function f_transform;
      iu_params transform_params = new iu_params();
      transform_params.a = a;
      transform_params.f = f;

      //f_transform.function = &iu_transform;
      //f_transform.params = &transform_params;

      ScalarFunctionDD f_transform = delegate(double t) { return iu_transform(t, transform_params); };

      GSL_ERROR status = qags(f_transform, 0.0, 1.0,
                     epsabs, epsrel, limit,
                     workspace,
                     out result, out abserr,
                     q);

      return status;
    }

    static double
    iu_transform(double t, iu_params p)
    {
      double a = p.a;
      ScalarFunctionDD f = p.f;
      double x = a + (1 - t) / t;
      double y = f(x);
      return (y / t) / t;
    }
  }
}
