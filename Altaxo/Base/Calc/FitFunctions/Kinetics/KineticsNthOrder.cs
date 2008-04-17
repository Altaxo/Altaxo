using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Calc.FitFunctions.Kinetics
{
  public class KineticsNthOrder
  {
    /// <summary>
    /// This core is related to the solution of a nth order kinetic equation system (see remarks).
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="x0">Starting value at t=0.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics (equal or greater than 0).</param>
    /// <returns>A value, that can be used to calculate the solution of the set of differential equations (see remarks). The return value is 1 at t=0 and 0 at t=Infinity.</returns>
    /// <remarks>
    /// The set of differential equations is:
    /// <para>dy/dt=k*x^order</para>
    /// <para>dy/dt=-dx/dt</para>
    /// with the boundary conditions:
    /// <para>x(t=0)==x0</para> and
    /// <para>y(t=0)==1-x0.</para>
    /// <para>The solution of this system of differential equations is then:</para>
    /// <para><c>y(t)==1-x0*Core(t,x0,k,order)</c>.</para>
    /// <para>and</para>
    /// <para><c>x(t)==x0*Core(t,x0,k,order)</c>.</para>
    /// <para>The Core function starts with a value of 1 at t=0 and ends with a value of 0 at t=Infinity.</para>
    ///</remarks>
    public static double Core(double t, double x0, double k, double order)
    {
      if (order >= 1)
      {
        if (order == 1)
          return Math.Exp(-k * t);
        else
          return Math.Pow(1 + Math.Pow(x0, order - 1) * (order - 1) * k * t, 1/(1 - order));
      }
      else // alpha<1
      {
        if (order == 0)
          return x0 == 0 ? 0 : Math.Max(0, 1 - k * t / x0);
        else if (order > 0)
          return Math.Pow(Math.Max(0, 1 + Math.Pow(x0, order - 1)*(order - 1) * k * t), 1 / (1 - order));
        else
          throw new ArgumentException("Order must be >=0");
      }
    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="p0">Volume fraction of aggregating species at time t=0, which is free (i.e. which is at this time not contained inside an aggragate).</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pInsideAggregate">Aggregates are assumed to contain a constant volume fraction of aggregating species. This parameter represents this constant.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>(pSample-p0)/pInsideAggregate</c>. For t going to infinity,
    /// this value tends to <c>pSample/pInsideAggregate</c>.</returns>
    /// <remarks>
    /// The kinetic equation for this problem (see <see cref="Core"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y. 
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.
    /// </remarks>
    public static double AggregateConcentrationFromP0AndPInsideAggregate(double t, double p0, double k, double order, double pSample, double pInsideAggregate)
    {
      return (pSample - p0 * Core(t, p0, k, order)) / pInsideAggregate;
    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="pA0">Volume fraction of aggregates at time t=0.</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pAInf">Volume fraction of aggregates at time t=Infinity.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>pA0</c>. For t going to infinity,
    /// this value tends to <c>pAInf</c>.</returns>
    /// <remarks>The provided volume fraction of aggregating species <c>pSample</c>is influencing only the rate. It is important only
    /// if you want to compare aggregation processes for sample with different content of aggregating species. If such a comparism is not neccessary,
    /// you can set <c>pSample</c> to 1.
    /// The kinetic equation for this problem (see <see cref="Core"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y. 
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.

    /// </remarks>
    public static double AggregateConcentrationFromPA0AndPAInf(double t, double pA0, double pAInf, double k, double order, double pSample)
    {
      double p0 = pSample * (1 - pA0 / pAInf);
      return (pAInf - (pAInf-pA0) * Core(t, p0, k, order));
    }
  }
}
