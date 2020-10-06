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
  public enum GSL_ERR
  {
    GSL_SUCCESS = 0,
    GSL_FAILURE = -1,
    GSL_CONTINUE = -2,  /* iteration has not converged */
    GSL_EDOM = 1,   /* input domain error, e.g sqrt(-1) */
    GSL_ERANGE = 2,   /* output range error, e.g. exp(1e100) */
    GSL_EFAULT = 3,   /* invalid pointer */
    GSL_EINVAL = 4,   /* invalid argument supplied by user */
    GSL_EFAILED = 5,   /* generic failure */
    GSL_EFACTOR = 6,   /* factorization failed */
    GSL_ESANITY = 7,   /* sanity check failed - shouldn't happen */
    GSL_ENOMEM = 8,   /* malloc failed */
    GSL_EBADFUNC = 9,   /* problem with user-supplied function */
    GSL_ERUNAWAY = 10,  /* iterative process is out of control */
    GSL_EMAXITER = 11,  /* exceeded max number of iterations */
    GSL_EZERODIV = 12,  /* tried to divide by zero */
    GSL_EBADTOL = 13,  /* user specified an invalid tolerance */
    GSL_ETOL = 14,  /* failed to reach the specified tolerance */
    GSL_EUNDRFLW = 15,  /* underflow */
    GSL_EOVRFLW = 16,  /* overflow  */
    GSL_ELOSS = 17,  /* loss of accuracy */
    GSL_EROUND = 18,  /* failed because of roundoff error */
    GSL_EBADLEN = 19,  /* matrix, vector lengths are not conformant */
    GSL_ENOTSQR = 20,  /* matrix not square */
    GSL_ESING = 21,  /* apparent singularity detected */
    GSL_EDIVERGE = 22,  /* integral or series is divergent */
    GSL_EUNSUP = 23,  /* requested feature is not supported by the hardware */
    GSL_EUNIMPL = 24,  /* requested feature not (yet) implemented */
    GSL_ECACHE = 25,  /* cache limit exceeded */
    GSL_ETABLE = 26,  /* table limit exceeded */
    GSL_ENOPROG = 27,  /* iteration is not making progress towards solution */
    GSL_ENOPROGJ = 28,  /* jacobian evaluations are not improving the solution */
    GSL_ETOLF = 29,  /* cannot reach the specified tolerance in F */
    GSL_ETOLX = 30,  /* cannot reach the specified tolerance in X */
    GSL_ETOLG = 31,  /* cannot reach the specified tolerance in gradient */
    GSL_EOF = 32   /* end of file */
  };

  public class GSL_ERROR
  {
    public string Message;
    public GSL_ERR Number;

    #region Fixed static values

    private static readonly GSL_ERROR _sSuccess = new GSL_ERROR("Success", GSL_ERR.GSL_SUCCESS, false);

    public static GSL_ERROR SUCCESS
    {
      get
      {
        return _sSuccess;
      }
    }

    private static readonly GSL_ERROR _sContinue = new GSL_ERROR("Continue", GSL_ERR.GSL_CONTINUE, false);

    public static GSL_ERROR CONTINUE
    {
      get
      {
        return _sContinue;
      }
    }

    #endregion Fixed static values

    public GSL_ERROR(string message, GSL_ERR number, bool bDebug)
    {
      Message = message;
      Number = number;

      if (bDebug)
        throw new ArithmeticException(message);
    }

    public override bool Equals(object? obj)
    {
      if (obj is GSL_ERROR b)
        return Number == b.Number;
      else
        return false;
    }

    public override int GetHashCode()
    {
      return Number.GetHashCode();
    }
  }
}
