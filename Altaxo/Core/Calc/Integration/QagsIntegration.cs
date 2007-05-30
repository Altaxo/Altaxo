using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
  public class QagsIntegration : IntegrationBase
  {
      #region offical C# interface
    gsl_integration_workspace _workSpace;
    gsl_integration_rule _integrationRule;


    public QagsIntegration()
    {
      _integrationRule = new QK21().Integrate;
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
       double a, double b,
       double epsabs, double epsrel, int limit,
       gsl_integration_rule integrationRule,
       out double result, out double abserr)
    {
      if (null == _workSpace || limit > _workSpace.limit)
        _workSpace = new gsl_integration_workspace(limit);

      return qags(f, a, b, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule);
    }

    public GSL_ERROR Integrate(ScalarFunctionDD f,
          double a, double b,
          double epsabs, double epsrel, int limit,
          out double result, out double abserr)
    {
      return Integrate(f, a, b, epsabs, epsrel, limit, _integrationRule, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double a, double b,
          double epsabs, double epsrel,
          int limit,
          gsl_integration_rule q,
          out double result, out double abserr,
          ref object tempStorage)
    {
      QagsIntegration algo = tempStorage as QagsIntegration;
      if (null == algo)
        tempStorage = algo = new QagsIntegration();
      return algo.Integrate(f, a, b, epsabs, epsrel, limit, q, out result, out abserr);
    }

    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
          double a, double b,
          double epsabs, double epsrel,
          int limit,
          out double result, out double abserr,
          ref object tempStorage
          )
    {
      return Integration(f, a, b, epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }

    public static GSL_ERROR
   Integration(ScalarFunctionDD f,
     double a, double b,
     double epsabs, double epsrel,
     int limit,
      gsl_integration_rule q,
     out double result, out double abserr
     )
    {
      object tempStorage = null;
      return Integration(f, a, b, epsabs, epsrel, limit, q, out result, out abserr, ref tempStorage);
    }


    public static GSL_ERROR
    Integration(ScalarFunctionDD f,
      double a, double b,
      double epsabs, double epsrel,
      int limit,
      out double result, out double abserr
      )
    {
      object tempStorage = null;
      return Integration(f, a, b, epsabs, epsrel, limit, new QK21().Integrate, out result, out abserr, ref tempStorage);
    }


    #endregion

  }
}
