#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

/*
 * MoreThuenteLineSearch.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Optimization
{
  ///<summary>More-Thuente Line Search Method</summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved. See <a>http://www.dnAnalytics.net</a> for details.</para>
  /// <para>Adopted to Altaxo (c) 2005 Dr. Dirk Lellinger.</para>
  /// </remarks>
  public class MoreThuenteLineSearch : LineSearchMethod 
  {
  
    ///<summary>Constructor for More-Thuente Line Search.</summary>
    public MoreThuenteLineSearch(CostFunction costfunction)
      : this(costfunction, new EndCriteria()) {}
    public MoreThuenteLineSearch(CostFunction costfunction, EndCriteria endcriteria)
    {
      this.costFunction_=costfunction;
      this.endCriteria_=endcriteria;
    }
  
    ///<summary> Method Name </summary>
    public override string MethodName 
    {
      get { return "More-Thuente Line Search Method"; }
    }
    
    public double ftol_=1.0e-4;  //non-negative
    public double gtol_=1.0e-1;  //non-negative
    public double xtol_=1.0e-17; //non-negative
    public double stpmin_= 1.0e-20; //non-negative
    public double stpmax_= 1.0e20; //non-negative
    public int maxfev_ = 40; // positive
    public int maxiter_ = 20; // positive
    
    ///<summary> Minimize the given cost function </summary>
    public override DoubleVector Search(DoubleVector x, DoubleVector s, double stp) 
    {
      
      DoubleVector grad = GradientEvaluation(x);
      double dginit = grad.GetDotProduct(s);
      
      // this is a port of CVSMOD in CG++
      DoubleVector retx = new DoubleVector(x);
      bool brackt;
      bool stage1;
      int nfev;
      int infoc=1;
      double finit;
      double ftest1;
      double dgtest;
      double width;
      double width1;
      double f;
      DoubleVector g;
      double dg;
      double fm;
      double fxm;
      double fym;
      double dxgm;
      double dygm;
      double dgref;
      double dgm;
      double dgxm;
      double dgym;
      
      double stmin; 
      double stmax;
      
      double p5 = 0.5;
      double p66 = 0.66;
      double xtrapf = 4.0;
      double zero = 0.0;
      DoubleVector wa = new DoubleVector(x);
      
      // CHECK THE INPUT PARAMETERS FOR ERRORS.
      if (x.Length<=0)
        throw new OptimizationException("Incorrect Input parameters for Linesearch");
      
      // COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
      // AND CHECK THAT S IS A DESCENT DIRECTION.
      if (dginit>=0)
        throw new OptimizationException("s isn't a decent direction");
        
      // INITIALIZE LOCAL VARIABLES.
      brackt = false;
      stage1 = true;
      nfev = 0;
      finit = FunctionEvaluation(x);
      g = GradientEvaluation(x);

      dgtest = ftol_ * dginit;
      width = stpmax_ - stpmin_;
      width1 = width / 0.5;
      
      // THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
      // FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
      // THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
      // FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
      // THE INTERVAL OF UNCERTAINTY.
      // THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
      // FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
      double stx = 0;
      double fx = finit;
      double dgx = dginit;
      double sty = 0.0;
      double fy = finit;
      double dgy = dginit;
      
      // START OF ITERATION.
      for(int iter=1; iter <= maxiter_; iter++) 
      {
        // SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
        // TO THE PRESENT INTERVAL OF UNCERTAINTY.
        if (brackt) 
        {
          stmin = System.Math.Min(stx,sty);
          stmax = System.Math.Max(stx,sty);
        } 
        else 
        {
          stmin = stx;
          stmax = stp + xtrapf*(stp - stx);
        }
        
        // FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
        stp = System.Math.Max(stp,stpmin_);
        stp = System.Math.Min(stp,stpmax_);
        
        // IF AN UNUSAL TERMINATION IS TO OCCUR THEN LET
        // STP BE THE LOWEST POINT OBTAINED SO FAR
        if ((brackt && (stp <= stmin || stp >=stmax)) || nfev>=(maxfev_-1) || infoc==0 
          || (brackt && (stmax-stmin)<=xtol_*stmax))
          stp = stx;
          
        // EVALUATE THE FUNCTION AND GRADIENT AT STP
        // AND COMPUTE THE DIRECTION DERIVATIVE.
        retx = wa + stp*s;
            
        // Compute function value;
        f = FunctionEvaluation(retx);
        nfev = nfev + 1;
        dg = g.GetDotProduct(s);
        ftest1 = finit + stp*dgtest;
        
        info = 0;
        // TEST FOR CONVERGENCE.
        if ((brackt && (stp <= stmin || stp >= stmax)) || infoc==0)
          info = 6;
        if (stp == stpmax_ && f <= ftest1 && dg<=dgtest)
          info = 5;
        if (stp == stpmin_ && f >= ftest1 || dg >= dgtest)
          info = 4;
        if (nfev >= maxfev_) 
          info = 3;
        if (brackt && (stmax - stmin) <= xtol_*stmax)
          info = 2;
        // More's code has been modified so that at least one new
        // function value is computed during the line search (enforcing
        // at least one interpolation is not easy, since the code may
        // override an interpolation)
        if (f<= ftest1 && System.Math.Abs(dg) <= gtol_*(-dginit) && nfev>1)
          info = 1;
        
        // CHECK FOR TERMINATION.
        if (info!=0) 
        {
          dgref = dg;
          return retx;
        }
        
        // IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
        // FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
        if (stage1 && f<=ftest1 && dg>=System.Math.Min(ftol_,gtol_)*dginit)
          stage1 = false;

        // A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
        // WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
        // FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
        // DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
        // OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
        if (stage1 && f<=fx && f>ftest1) 
        {
        
          // DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
          fm = f - stp * dgtest;
          fxm = fx - stx*dgtest;
          fym = fy - sty*dgtest;
          dgm = dg - dgtest;
          dgxm = dgx - dgtest;
          dgym = dgy - dgtest;
        
          // CALL CSTEPM TO UPDATE THE INTERVAL OF UNCERTAINTY
          // AND TO COMPUTE THE NEW STEP.
          Cstepm(ref stx,ref fxm,ref dgxm,ref sty,ref fym,ref dgym,ref stp,ref fm,ref dgm,ref brackt,
            ref stmin,ref stmax,ref infoc);
          
          // RESET THE FUNCTION AND GRADIENT VALUES FOR F.
          fx = fxm + stx * dgtest;
          fy = fym + sty * dgtest;
          dgx = dgxm + dgtest;
          dgy = dgym + dgtest;
        } 
        else 
        {
          
          // CALL CSTEPM TO UPDATE THE INTERVAL OF UNCERTAINTY
          // AND TO COMPUTE THE NEW STEP.
          Cstepm(ref stx,ref fx,ref dgx,ref sty,ref fy,ref dgy,ref stp,ref f,ref dg,ref brackt,
            ref stmin,ref stmax,ref infoc);
        }
        
        // FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE INTERVAL OF UNCERTAINTY.
        if (brackt) 
        {
          if (System.Math.Abs(sty-stx) > 0.66*width1) 
            stp = stx + 0.5*(sty-stx);
          width1 = width;
          width = System.Math.Abs(sty-stx);
        }
      }
      return retx;
    }
      
    private void Cstepm(ref double stx, ref double fx, ref double dx, ref double sty, ref double fy, ref double dy, 
      ref double stp, ref double fp, ref double dp, ref bool brackt, ref double stpmin, ref double stpmax, ref int info) 
    {
      double gamma,p,q,r,s,sgnd,stpc,stpf,stpq,theta;
      bool bound;
      info = 0;
      
      // CHECK THE INPUT PARAMETERS FOR ERRORS.
      if ((brackt && (stp <=System.Math.Min(stx,sty) || stp>=System.Math.Max(stx,sty))) || 
        dx*(stp-stx)>=0.0 || stpmax<stpmin)
        return;
        
      // DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
      sgnd = dp*(dx/System.Math.Abs(dx));
      
      // FIRST CASE. A HIGHER FUNCTION VALUE.
      // THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
      // TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN
      // ELSE THE AVERAGE OF THE CUBIC AAND QUADRATIC STEPS IS TAKEN.
      if (fp>fx) 
      {
        info = 1;
        bound = true;
        theta = 3*(fx - fp)/(stp-stx) + dx + dp;
        s = System.Math.Max(System.Math.Abs(theta),
          System.Math.Max(System.Math.Abs(dx),System.Math.Abs(dp)));
        gamma = s*System.Math.Sqrt(System.Math.Pow(theta/s,2) - (dx/s)*(dp/s));
        if (stp < stx) 
          gamma = -gamma;
        p = (gamma-dx) + theta;
        q = ((gamma-dx)+gamma) + dp;
        r = p/q;
        stpc = stx + r*(stp-stx);
        stpq = stx + ((dx/((fx-fp)/(stp-stx)+dx))/2)*(stp-stx);
        if (System.Math.Abs(stpc-stx) < System.Math.Abs(stpq-stx))
          stpf = stpc;
        else 
          stpf = stpc + (stpq-stpc)/2;
        brackt = true;
        
        // SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIES OF
        // OPPOSITE SIGN. THE MINIMUM IS BRACKETED.  IF THE CUBIC
        // STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP, 
        // THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
      } 
      else if (sgnd < 0.0) 
      {
        info = 2;
        bound = false;
        theta = 3*(fx-fp)/(stp-stx) + dx + dp;
        s = System.Math.Max(System.Math.Abs(theta),
          System.Math.Max(System.Math.Abs(dx),System.Math.Abs(dp)));
        gamma = s*System.Math.Sqrt(System.Math.Pow(theta/s,2) - (dx/s)*(dp/s));
        if (stp > stx) 
          gamma = -gamma;
        p = (gamma - dp) + theta;
        q = ((gamma - dp) + gamma) + dx;
        r = p/q;
        stpc = stp + r*(stx-stp);
        stpq = stp + (dp/(dp-dx)) * (stx - stp);
        if (System.Math.Abs(stpc-stp) > System.Math.Abs(stpq-stp)) 
          stpf = stpc;
        else
          stpf = stpq;
        brackt = true;
      
        // THIRD CASE.  A LOWER FUNCTION VALUE, DERIVATES OF THE
        // SAME SIGN, AND THE MAGNITUDE OF THE DERVIATIVES DECREASE.
        // THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
        // IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
        // IS BEYOND STP.  OTHERWISE THE CUBIC STEP IS DEFINED TO BE
        // EITHER STPMIN OR STPMAX.  THE QUARDRATIC (SECANT) STEP IS ALSO
        // COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE STEP 
        // CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
      } 
      else if (System.Math.Abs(dp) < System.Math.Abs(dx)) 
      {
        info = 3;
        bound = true;
        theta = 3*(fx-fp)/(stp-stx) + dx + dp;
        s = System.Math.Max(System.Math.Abs(theta),
          System.Math.Max(System.Math.Abs(dx), System.Math.Abs(dp)));
        
        // THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
        // TO INFINITY IN THE DIRECTION OF THE STEP.
        gamma = s*System.Math.Sqrt(System.Math.Max(0.0, System.Math.Pow(theta/s,2) - (dx/s)*(dp/s)));
        if (stp > stx) 
          gamma = -gamma;
        p = (gamma - dp) + theta;
        q = (gamma + (dx-dp)) + gamma;
        r = p/q;
        if (r < 0.0 && gamma != 0.0 )
          stpc = stp + r*(stx-stp);
        else if (stp > stx)
          stpc = stpmax;
        else
          stpc = stpmin;
        stpq = stp + (dp/(dp-dx))*(stx - stp);
        if (brackt)
          if (System.Math.Abs(stp-stpc) < System.Math.Abs(stp-stpq))
            stpf = stpc;
          else
            stpf = stpq;
        else
          if (System.Math.Abs(stp-stpc) > System.Math.Abs(stp-stpq))
          stpf = stpc;
        else
          stpf = stpq;
        
        // FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
        // SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
        // NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
        // IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
      } 
      else 
      {
        info = 4;
        bound = false;
        if (brackt) 
        {
          theta = 3*(fp-fy)/(sty-stp) + dy + dp;
          s = System.Math.Max(System.Math.Abs(theta),
            System.Math.Max(System.Math.Abs(dy), System.Math.Abs(dp)));
          gamma = s*System.Math.Sqrt(System.Math.Pow(theta/s,2) - (dy/s)*(dp/s));
          if (stp > sty)
            gamma = -gamma;
          p = (gamma - dp) + theta;
          q = ((gamma - dp) + gamma) + dy;
          r = p/q;
          stpc = stp = r*(sty - stp);
          stpf = stpc;
        } 
        else if (stp > stx) 
          stpf = stpmax;
        else
          stpf = stpmin;
      }
      
      //  UPDATE THE INTERVAL OF UNCERTAINTY.  THIS UPDATE DOES NOT 
      // DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
      if (fp>fx) 
      {
        sty = stp;
        fy = fp;
        dy = dp;
      } 
      else 
      {
        if (sgnd < 0.0) 
        {
          sty = stx;
          fy = fx;
          dy = dx;
        }
        stx = stp;
        fx = fp;
        dx = dp;
      }
      
      // COMPUTE THE NEW STEP AND SAFEGUARD IT.
      stpf = System.Math.Min(stpmax,stpf);
      stpf = System.Math.Max(stpmin,stpf);
      stp = stpf;
      if (brackt && bound) 
        if (sty > stx) 
          stp = System.Math.Min(stx+0.66*(sty-stx),stp);
        else
          stp = System.Math.Max(stx+0.66*(sty-stx),stp);
      return;
    }
    
    int info;
  }
}
