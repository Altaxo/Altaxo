using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.Integration
{
	/// <summary>
	/// Adaptive integration on infinite interval (-Infinity,+Infinity).
	/// </summary>
	/// <remarks>
	/// This class computes the integral of the function f over the infinite interval
	/// (-Infinity,+Infinity). The integral is mapped onto the semi-open interval (0, 1] using the
	/// transformation x = (1 - t)/t.
	/// It is then integrated using the QAGS algorithm. The normal 21-point Gauss-Kronrod
	/// rule of QAGS is replaced by a 15-point rule, because the transformation can generate
	/// an integrable singularity at the origin. In this case a lower-order rule is more efficient.
	/// <para>Ref.: Gnu Scientific library reference manual (<see href="http://www.gnu.org/software/gsl/" />)</para>
	/// </remarks>
	public class QagiIntegration : IntegrationBase
	{
		#region offical C# interface
		bool _debug;
		gsl_integration_workspace _workSpace;
		gsl_integration_rule _integrationRule;

		/// <summary>
		/// Returns the default integration rule used for this class.
		/// </summary>
		public static gsl_integration_rule DefaultIntegrationRule
		{
			get
			{
				return new QK15().Integrate;
			}
		}

		/// <summary>
		/// Creates an instance of this integration class with a default integration rule and default debug flag setting.
		/// </summary>
		public QagiIntegration()
			: this(DefaultIntegrationRule, DefaultDebugFlag)
		{
		}

		/// <summary>
		/// Creates an instance of this integration class with a default integration rule and specified debug flag setting.
		/// </summary>
		/// <param name="debug">Setting of the debug flag for this instance. If the integration fails or the specified accuracy
		/// is not reached, an exception is thrown if the debug flag is set to true. If set to false, the return value of the integration
		/// function will be set to the appropriate error code (an exception will be thrown then only for serious errors).</param>
		public QagiIntegration(bool debug)
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
		public QagiIntegration(gsl_integration_rule integrationRule, bool debug)
		{
			_integrationRule = integrationRule;
			_debug = debug;
		}

		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity).
		/// </summary>
		/// <param name="f">Function to integrate.</param>
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
		public GSL_ERROR Integrate(ScalarFunctionDD f,
				double epsabs, double epsrel, int limit,
			 gsl_integration_rule integrationRule, bool debug,
			 out double result, out double abserr)
		{
			if (null == _workSpace || limit > _workSpace.limit)
				_workSpace = new gsl_integration_workspace(limit);

			return gsl_integration_qagi(f, epsabs, epsrel, limit, _workSpace, out result, out abserr, integrationRule, debug);
		}

		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity) using the integration rule and debug setting given in the constructor.
		/// </summary>
		/// <param name="f">Function to integrate.</param>
		/// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
		/// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
		/// <param name="limit">Maximum number of subintervals used for integration.</param>
		/// <param name="result">On return, contains the integration result.</param>
		/// <param name="abserr">On return, contains the absolute error of integration.</param>
		/// <returns>Null if successfull, otherwise the appropriate error code.</returns>
		public GSL_ERROR Integrate(ScalarFunctionDD f,
					double epsabs, double epsrel, int limit,
					out double result, out double abserr)
		{
			return Integrate(f, epsabs, epsrel, limit, _integrationRule, _debug, out result, out abserr);
		}

		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity).
		/// </summary>
		/// <param name="f">Function to integrate.</param>
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
		public static GSL_ERROR Integration(ScalarFunctionDD f,
					double epsabs, double epsrel,
					int limit,
					gsl_integration_rule integrationRule, bool debug,
					out double result, out double abserr,
					ref object tempStorage)
		{
			QagiIntegration algo = tempStorage as QagiIntegration;
			if (null == algo)
				tempStorage = algo = new QagiIntegration(integrationRule, debug);
			return algo.Integrate(f, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr);
		}

		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity) using default settings for integration rule and debugging.
		/// </summary>
		/// <param name="f">Function to integrate.</param>
		/// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
		/// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
		/// <param name="limit">Maximum number of subintervals used for integration.</param>
		/// <param name="result">On return, contains the integration result.</param>
		/// <param name="abserr">On return, contains the absolute error of integration.</param>
		/// <param name="tempStorage">Provides a temporary storage object that you can reuse for repeating function calls.</param>
		/// <returns>Null if successfull, otherwise the appropriate error code.</returns>
		public static GSL_ERROR Integration(ScalarFunctionDD f,
					double epsabs, double epsrel,
					int limit,
					out double result, out double abserr,
					ref object tempStorage
					)
		{
			QagiIntegration algo = tempStorage as QagiIntegration;
			if (null == algo)
				tempStorage = algo = new QagiIntegration();
			return algo.Integrate(f, epsabs, epsrel, limit, out result, out abserr);
		}

		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity).
		/// </summary>
		/// <param name="f">Function to integrate.</param>
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
		public static GSL_ERROR Integration(ScalarFunctionDD f,
		 double epsabs, double epsrel,
		 int limit,
			gsl_integration_rule integrationRule, bool debug,
		 out double result, out double abserr
		 )
		{
			object tempStorage = null;
			return Integration(f, epsabs, epsrel, limit, integrationRule, debug, out result, out abserr, ref tempStorage);
		}


		/// <summary>
		/// Adaptive integration on infinite interval (-Infinity,+Infinity)  using default settings for integration rule and debugging.
		/// </summary>
		/// <param name="f">Function to integrate.</param>
		/// <param name="epsabs">Specifies the expected absolute error of integration. Should be set to zero (0) if you specify a relative error.</param>
		/// <param name="epsrel">Specifies the expected relative error of integration. Should be set to zero (0) if you specify an absolute error.</param>
		/// <param name="limit">Maximum number of subintervals used for integration.</param>
		/// <param name="result">On return, contains the integration result.</param>
		/// <param name="abserr">On return, contains the absolute error of integration.</param>
		/// <returns>Null if successfull, otherwise the appropriate error code.</returns>
		public static GSL_ERROR Integration(ScalarFunctionDD f,
			double epsabs, double epsrel,
			int limit,
			out double result, out double abserr
			)
		{
			object tempStorage = null;
			return Integration(f, epsabs, epsrel, limit, out result, out abserr, ref tempStorage);
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
													gsl_integration_rule q, bool bDebug
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
										 q, bDebug);

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
