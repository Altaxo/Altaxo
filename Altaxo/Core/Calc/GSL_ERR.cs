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

namespace Altaxo.Calc
{
  /// <summary>
  /// Error codes compatible with the GNU Scientific Library (GSL).
  /// </summary>
  public enum GSL_ERR
  {
    /// <summary>Success.</summary>
    GSL_SUCCESS = 0,

    /// <summary>Generic failure.</summary>
    GSL_FAILURE = -1,

    /// <summary>Iteration has not converged.</summary>
    GSL_CONTINUE = -2,  /* iteration has not converged */

    /// <summary>Input domain error (for example, <c>sqrt(-1)</c>).</summary>
    GSL_EDOM = 1,   /* input domain error, e.g sqrt(-1) */

    /// <summary>Output range error (for example, <c>exp(1e100)</c>).</summary>
    GSL_ERANGE = 2,   /* output range error, e.g. exp(1e100) */

    /// <summary>Invalid pointer.</summary>
    GSL_EFAULT = 3,   /* invalid pointer */

    /// <summary>Invalid argument supplied by user.</summary>
    GSL_EINVAL = 4,   /* invalid argument supplied by user */

    /// <summary>Generic failure.</summary>
    GSL_EFAILED = 5,   /* generic failure */

    /// <summary>Factorization failed.</summary>
    GSL_EFACTOR = 6,   /* factorization failed */

    /// <summary>Sanity check failed (should not happen).</summary>
    GSL_ESANITY = 7,   /* sanity check failed - shouldn't happen */

    /// <summary>Memory allocation failed.</summary>
    GSL_ENOMEM = 8,   /* malloc failed */

    /// <summary>Problem with user-supplied function.</summary>
    GSL_EBADFUNC = 9,   /* problem with user-supplied function */

    /// <summary>Iterative process is out of control.</summary>
    GSL_ERUNAWAY = 10,  /* iterative process is out of control */

    /// <summary>Exceeded maximum number of iterations.</summary>
    GSL_EMAXITER = 11,  /* exceeded max number of iterations */

    /// <summary>Attempted to divide by zero.</summary>
    GSL_EZERODIV = 12,  /* tried to divide by zero */

    /// <summary>User specified an invalid tolerance.</summary>
    GSL_EBADTOL = 13,  /* user specified an invalid tolerance */

    /// <summary>Failed to reach the specified tolerance.</summary>
    GSL_ETOL = 14,  /* failed to reach the specified tolerance */

    /// <summary>Underflow.</summary>
    GSL_EUNDRFLW = 15,  /* underflow */

    /// <summary>Overflow.</summary>
    GSL_EOVRFLW = 16,  /* overflow  */

    /// <summary>Loss of accuracy.</summary>
    GSL_ELOSS = 17,  /* loss of accuracy */

    /// <summary>Failed due to roundoff error.</summary>
    GSL_EROUND = 18,  /* failed because of roundoff error */

    /// <summary>Matrix/vector lengths are not conformant.</summary>
    GSL_EBADLEN = 19,  /* matrix, vector lengths are not conformant */

    /// <summary>Matrix is not square.</summary>
    GSL_ENOTSQR = 20,  /* matrix not square */

    /// <summary>Apparent singularity detected.</summary>
    GSL_ESING = 21,  /* apparent singularity detected */

    /// <summary>Integral or series is divergent.</summary>
    GSL_EDIVERGE = 22,  /* integral or series is divergent */

    /// <summary>Requested feature is not supported by the hardware.</summary>
    GSL_EUNSUP = 23,  /* requested feature is not supported by the hardware */

    /// <summary>Requested feature is not (yet) implemented.</summary>
    GSL_EUNIMPL = 24,  /* requested feature not (yet) implemented */

    /// <summary>Cache limit exceeded.</summary>
    GSL_ECACHE = 25,  /* cache limit exceeded */

    /// <summary>Table limit exceeded.</summary>
    GSL_ETABLE = 26,  /* table limit exceeded */

    /// <summary>Iteration is not making progress towards a solution.</summary>
    GSL_ENOPROG = 27,  /* iteration is not making progress towards solution */

    /// <summary>Jacobian evaluations are not improving the solution.</summary>
    GSL_ENOPROGJ = 28,  /* jacobian evaluations are not improving the solution */

    /// <summary>Cannot reach the specified tolerance in <c>F</c>.</summary>
    GSL_ETOLF = 29,  /* cannot reach the specified tolerance in F */

    /// <summary>Cannot reach the specified tolerance in <c>X</c>.</summary>
    GSL_ETOLX = 30,  /* cannot reach the specified tolerance in X */

    /// <summary>Cannot reach the specified tolerance in gradient.</summary>
    GSL_ETOLG = 31,  /* cannot reach the specified tolerance in gradient */

    /// <summary>End of file.</summary>
    GSL_EOF = 32   /* end of file */
  };

  /// <summary>
  /// Represents a GSL-style error (code and message).
  /// </summary>
  public class GSL_ERROR
  {
    /// <summary>
    /// Gets or sets the human-readable error message.
    /// </summary>
    public string Message;

    /// <summary>
    /// Gets or sets the numeric error code.
    /// </summary>
    public GSL_ERR Number;

    #region Fixed static values

    private static readonly GSL_ERROR _sSuccess = new GSL_ERROR("Success", GSL_ERR.GSL_SUCCESS, false);

    /// <summary>
    /// Gets the success result.
    /// </summary>
    public static GSL_ERROR SUCCESS
    {
      get
      {
        return _sSuccess;
      }
    }

    private static readonly GSL_ERROR _sContinue = new GSL_ERROR("Continue", GSL_ERR.GSL_CONTINUE, false);

    /// <summary>
    /// Gets the "continue" result, indicating that an iterative algorithm has not yet converged.
    /// </summary>
    public static GSL_ERROR CONTINUE
    {
      get
      {
        return _sContinue;
      }
    }

    #endregion Fixed static values

    /// <summary>
    /// Initializes a new instance of the <see cref="GSL_ERROR"/> class.
    /// </summary>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="number">The error code.</param>
    /// <param name="bDebug">If set to <c>true</c>, throws an <see cref="ArithmeticException"/>.</param>
    /// <exception cref="ArithmeticException">Thrown when <paramref name="bDebug"/> is <c>true</c>.</exception>
    public GSL_ERROR(string message, GSL_ERR number, bool bDebug)
    {
      Message = message;
      Number = number;

      if (bDebug)
        throw new ArithmeticException(message);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is GSL_ERROR b)
        return Number == b.Number;
      else
        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return Number.GetHashCode();
    }
  }
}
