using System;

namespace Altaxo.Calc
{
	/// <summary>
	/// Rounding of numbers
	/// </summary>
	public class Rounding
	{
    /// <summary>
    /// This returns the next number k with k greater or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static int RoundUp(int i , int n)
    {
      n = Math.Abs(n);
      int r = i % n;
      return r==0 ? i : i + n - r;
    }
    /// <summary>
    /// This returns the next number k with k lesser or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static int RoundDown(int i , int n)
    {
      n = Math.Abs(n);
      int r = i % n;
      return r==0 ? i : i - r;
    }
    /// <summary>
    /// This returns the next number k with k greater or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round up.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static long RoundUp(long i , long n)
    {
      n = Math.Abs(n);
      long r = i % n;
      return r==0 ? i : i + n - r;
    }
    /// <summary>
    /// This returns the next number k with k lesser or equal i, and k mod n == 0. 
    /// </summary>
    /// <param name="i">The number to round down.</param>
    /// <param name="n">The rounding step.</param>
    /// <returns></returns>
    public static long RoundDown(long i , long n)
    {
      n = Math.Abs(n);
      long r = i % n;
      return r==0 ? i : i - r;
    }

	}
}
