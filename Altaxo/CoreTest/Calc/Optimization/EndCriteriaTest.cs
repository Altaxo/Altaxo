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
using Altaxo.Calc.Optimization;
using Xunit;

namespace AltaxoTest.Calc.Optimization
{
  
  public class EndCriteriaTest
  {
    //Test Method Iteration Counter and associated Functionality
    [Fact]
    public void TestIterationCounter()
    {
      int maxiter = 2;
      var ec = new EndCriteria(maxiter, 1e-8, 100, 100);
      Assert.Equal(ec.maxIteration, maxiter);

      ec.iterationCounter++;

      Assert.True(!ec.CheckIterations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      ec.iterationCounter++;

      Assert.True(ec.CheckIterations());
      Assert.Equal(EndCriteria.CriteriaType.MaximumIteration, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.iterationCounter);
      Assert.True(!ec.CheckIterations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Function Evaluation Counter and associated Functionality
    [Fact]
    public void TestFunctionEvaluationCounter()
    {
      int maxeval = 2;
      var ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.Equal(ec.maxFunctionEvaluation, maxeval);

      ec.functionEvaluationCounter++;

      Assert.True(!ec.CheckFunctionEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      ec.functionEvaluationCounter++;

      Assert.True(ec.CheckFunctionEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.MaximumFunctionEvaluation, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.functionEvaluationCounter);
      Assert.True(!ec.CheckFunctionEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Gradient Evaluation Counter and associated Functionality
    [Fact]
    public void TestGradientEvaluationCounter()
    {
      int maxeval = 2;
      var ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.Equal(ec.maxGradientEvaluation, maxeval);

      ec.gradientEvaluationCounter++;

      Assert.True(!ec.CheckGradientEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      ec.gradientEvaluationCounter++;

      Assert.True(ec.CheckGradientEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.MaximumGradientEvaluation, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.gradientEvaluationCounter);
      Assert.True(!ec.CheckGradientEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Hessian Evaluation Counter and associated Functionality
    [Fact]
    public void TestHessianEvaluationCounter()
    {
      int maxeval = 2;
      var ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.Equal(ec.maxHessianEvaluation, maxeval);

      ec.hessianEvaluationCounter++;

      Assert.True(!ec.CheckHessianEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      ec.hessianEvaluationCounter++;

      Assert.True(ec.CheckHessianEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.MaximumHessianEvaluation, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.hessianEvaluationCounter);
      Assert.True(!ec.CheckHessianEvaluations());
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Stationary Point Counter and associated Functionality
    [Fact]
    public void TestStationaryPointCounter()
    {
      int maxstationarypoint = 1;
      var ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.Equal(ec.maxStationaryPointIterations, maxstationarypoint);

      Assert.True(!ec.CheckStationaryPoint(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckStationaryPoint(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.StationaryPoint, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.stationaryPointIterationsCounter);
      Assert.True(!ec.CheckStationaryPoint(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Stationary Gradient Counter and associated Functionality
    [Fact]
    public void TestStationaryGradientCounter()
    {
      int maxstationarypoint = 1;
      var ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.Equal(ec.maxStationaryGradientIterations, maxstationarypoint);

      Assert.True(!ec.CheckStationaryGradient(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckStationaryGradient(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.StationaryGradient, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.stationaryGradientIterationsCounter);
      Assert.True(!ec.CheckStationaryGradient(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Stationary Hessian Counter and associated Functionality
    [Fact]
    public void TestStationaryHessianCounter()
    {
      int maxstationarypoint = 1;
      var ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.Equal(ec.maxStationaryHessianIterations, maxstationarypoint);

      Assert.True(!ec.CheckStationaryHessian(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckStationaryHessian(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.StationaryHessian, ec.Criteria);

      ec.Reset();
      Assert.Equal(0, ec.stationaryHessianIterationsCounter);
      Assert.True(!ec.CheckStationaryHessian(1.0, 1.0));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Function Epsilon and associated Functionality
    [Fact]
    public void TestFunctionEpsilon()
    {
      double functionepsilon = 1e-8;
      var ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.Equal(ec.minFunctionEpsilon, functionepsilon);

      Assert.True(!ec.CheckFunctionEpsilon(functionepsilon * 2));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckFunctionEpsilon(functionepsilon / 2));
      Assert.Equal(EndCriteria.CriteriaType.FunctionEpsilon, ec.Criteria);

      ec.Reset();
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Gradient Epsilon and associated Functionality
    [Fact]
    public void TestGradientEpsilon()
    {
      double functionepsilon = 1e-8;
      var ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.Equal(ec.minGradientEpsilon, functionepsilon);

      Assert.True(!ec.CheckGradientEpsilon(functionepsilon * 2));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckGradientEpsilon(functionepsilon / 2));
      Assert.Equal(EndCriteria.CriteriaType.GradientEpsilon, ec.Criteria);

      ec.Reset();
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }

    //Test Hessian Epsilon and associated Functionality
    [Fact]
    public void TestHessianEpsilon()
    {
      double functionepsilon = 1e-8;
      var ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.Equal(ec.minHessianEpsilon, functionepsilon);

      Assert.True(!ec.CheckHessianEpsilon(functionepsilon * 2));
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);

      Assert.True(ec.CheckHessianEpsilon(functionepsilon / 2));
      Assert.Equal(EndCriteria.CriteriaType.HessianEpsilon, ec.Criteria);

      ec.Reset();
      Assert.Equal(EndCriteria.CriteriaType.None, ec.Criteria);
    }
  }
}
