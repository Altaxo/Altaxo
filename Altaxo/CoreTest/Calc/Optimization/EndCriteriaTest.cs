#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

using System;
using System.Collections;
using NUnit.Framework;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;

namespace AltaxoTest.Calc.Optimization 
{

  [TestFixture]
  public class EndCriteriaTest
  {

    //Test Method Iteration Counter and associated Functionality
    [Test]
    public void TestIterationCounter()
    {
      int maxiter = 2;
      EndCriteria ec = new EndCriteria(maxiter, 1e-8, 100,100);
      Assert.AreEqual(ec.maxIteration,maxiter);
      
      ec.iterationCounter++;
      
      Assert.IsTrue(!ec.CheckIterations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      ec.iterationCounter++;
      
      Assert.IsTrue(ec.CheckIterations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.MaximumIteration);
      
      ec.Reset();
      Assert.AreEqual(ec.iterationCounter,0);
      Assert.IsTrue(!ec.CheckIterations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    }   
    
    //Test Function Evaluation Counter and associated Functionality
    [Test]
    public void TestFunctionEvaluationCounter()
    {
      int maxeval = 2;
      EndCriteria ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.AreEqual(ec.maxFunctionEvaluation,maxeval);
      
      ec.functionEvaluationCounter++;
      
      Assert.IsTrue(!ec.CheckFunctionEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      ec.functionEvaluationCounter++;
      
      Assert.IsTrue(ec.CheckFunctionEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.MaximumFunctionEvaluation);
      
      ec.Reset();
      Assert.AreEqual(ec.functionEvaluationCounter,0);
      Assert.IsTrue(!ec.CheckFunctionEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    }   
    
    //Test Gradient Evaluation Counter and associated Functionality
    [Test]
    public void TestGradientEvaluationCounter()
    {
      int maxeval = 2;
      EndCriteria ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.AreEqual(ec.maxGradientEvaluation,maxeval);
      
      ec.gradientEvaluationCounter++;
      
      Assert.IsTrue(!ec.CheckGradientEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      ec.gradientEvaluationCounter++;
      
      Assert.IsTrue(ec.CheckGradientEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.MaximumGradientEvaluation);
      
      ec.Reset();
      Assert.AreEqual(ec.gradientEvaluationCounter,0);
      Assert.IsTrue(!ec.CheckGradientEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    }   
    
    //Test Hessian Evaluation Counter and associated Functionality
    [Test]
    public void TestHessianEvaluationCounter()
    {
      int maxeval = 2;
      EndCriteria ec = new EndCriteria(100, 1e-8, maxeval, 100);
      Assert.AreEqual(ec.maxHessianEvaluation,maxeval);
      
      ec.hessianEvaluationCounter++;
      
      Assert.IsTrue(!ec.CheckHessianEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      ec.hessianEvaluationCounter++;
      
      Assert.IsTrue(ec.CheckHessianEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.MaximumHessianEvaluation);
      
      ec.Reset();
      Assert.AreEqual(ec.hessianEvaluationCounter,0);
      Assert.IsTrue(!ec.CheckHessianEvaluations());
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    } 
    
    //Test Stationary Point Counter and associated Functionality
    [Test]
    public void TestStationaryPointCounter()
    {
      int maxstationarypoint = 1;
      EndCriteria ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.AreEqual(ec.maxStationaryPointIterations,maxstationarypoint);
      
      Assert.IsTrue(!ec.CheckStationaryPoint(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckStationaryPoint(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.StationaryPoint);
      
      ec.Reset();
      Assert.AreEqual(ec.stationaryPointIterationsCounter,0);
      Assert.IsTrue(!ec.CheckStationaryPoint(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    } 
    
    //Test Stationary Gradient Counter and associated Functionality
    [Test]
    public void TestStationaryGradientCounter()
    {
      int maxstationarypoint = 1;
      EndCriteria ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.AreEqual(ec.maxStationaryGradientIterations,maxstationarypoint);
      
      Assert.IsTrue(!ec.CheckStationaryGradient(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckStationaryGradient(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.StationaryGradient);
      
      ec.Reset();
      Assert.AreEqual(ec.stationaryGradientIterationsCounter,0);
      Assert.IsTrue(!ec.CheckStationaryGradient(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    }   
    
    //Test Stationary Hessian Counter and associated Functionality
    [Test]
    public void TestStationaryHessianCounter()
    {
      int maxstationarypoint = 1;
      EndCriteria ec = new EndCriteria(100, 1e-8, 100, maxstationarypoint);
      Assert.AreEqual(ec.maxStationaryHessianIterations,maxstationarypoint);
      
      Assert.IsTrue(!ec.CheckStationaryHessian(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckStationaryHessian(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.StationaryHessian);
      
      ec.Reset();
      Assert.AreEqual(ec.stationaryHessianIterationsCounter,0);
      Assert.IsTrue(!ec.CheckStationaryHessian(1.0,1.0));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    }   
    
    //Test Function Epsilon and associated Functionality
    [Test]
    public void TestFunctionEpsilon()
    {
      double functionepsilon = 1e-8;
      EndCriteria ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.AreEqual(ec.minFunctionEpsilon,functionepsilon);
      
      Assert.IsTrue(!ec.CheckFunctionEpsilon(functionepsilon*2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckFunctionEpsilon(functionepsilon/2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.FunctionEpsilon);
      
      ec.Reset();
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    } 
    
    //Test Gradient Epsilon and associated Functionality
    [Test]
    public void TestGradientEpsilon()
    {
      double functionepsilon = 1e-8;
      EndCriteria ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.AreEqual(ec.minGradientEpsilon,functionepsilon);
      
      Assert.IsTrue(!ec.CheckGradientEpsilon(functionepsilon*2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckGradientEpsilon(functionepsilon/2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.GradientEpsilon);
      
      ec.Reset();
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    } 
    
    //Test Hessian Epsilon and associated Functionality
    [Test]
    public void TestHessianEpsilon()
    {
      double functionepsilon = 1e-8;
      EndCriteria ec = new EndCriteria(100, functionepsilon, 100, 100);
      Assert.AreEqual(ec.minHessianEpsilon,functionepsilon);
      
      Assert.IsTrue(!ec.CheckHessianEpsilon(functionepsilon*2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
      
      Assert.IsTrue(ec.CheckHessianEpsilon(functionepsilon/2));
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.HessianEpsilon);
      
      ec.Reset();
      Assert.AreEqual(ec.Criteria, EndCriteria.CriteriaType.None);
    } 
  }
}
