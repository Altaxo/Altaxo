#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

using System;
using Altaxo.Calc;
using Altaxo.Calc.LinearAlgebra;

using NUnit.Framework;

namespace AltaxoTest.Calc.LinearAlgebra
{
  [TestFixture]
  public class TestSingularValueDecomposition
  {
    const double accuracy = 1E-14;

    [Test]
    public void Test01_3x3Matrix()
    {
      double[][] x = new double[][]
        {
          new double[]{1,1,1},
          new double[]{1,2,4},
          new double[]{1,3,9}
        };
     
      double[][] expectedU = new double[][]
       {
         new double[]{0.1323855611751141383983336, -0.8014095371363918468296921, 0.5832810788798007027395599},
         new double[]{0.4263811717635121943505712, -0.4851883520178218054139224, -0.7634077281713910856496549}, 
         new double[]{0.8948034195050466074658404, 0.3497642303796436079288927, 0.2774740052491605347063156}
       };

      double[] expectedSingularValues = new double[]{10.64956309214167630396485, 1.250703401814404251343328, 0.1501564090663296496197768};

      double[][] expectedV = new double[][]
        {
          new double[]{0.1364910597615280451679478, -0.7490454230919167919488232, 0.6483063664273445542029725}, 
          new double[]{0.3445735878052170177361156, -0.5776697728534024470814668, -0.7399774835213155452612752}, 
          new double[]{0.9287837386562145112758128, 0.3243895616023268212207566, 0.1792545093748964368092940}        
        };

      IMatrix X = MatrixMath.ToMatrix(x);

      MatrixMath.SingularValueDecomposition decomp = 
        MatrixMath.GetSingularValueDecomposition(X);


      // Test singular values
      for(int i=0;i<3;i++)
      {
        Assert.AreEqual(expectedSingularValues[i],decomp.Diagonal[i],accuracy,"SingularValue_"+i.ToString());
      }

      
      // Test the U Matrix
      for(int i=0;i<expectedU.Length;i++)
        for(int j=0;j<expectedU[0].Length;j++)
          Assert.AreEqual(expectedU[i][j],decomp.U[i][j],accuracy,string.Format("U[{0}][{1}]",i,j));


      // Test the V Matrix
      for(int i=0;i<expectedV.Length;i++)
        for(int j=0;j<expectedV[0].Length;j++)
          Assert.AreEqual(expectedV[i][j],decomp.V[i][j],accuracy,string.Format("V[{0}][{1}]",i,j));




    }

    [Test]
    public void Test02_4x3Matrix()
    {
      double[][] x = new double[4][]{
                                      new double[]{1,1,1},
                                      new double[]{1,2,4},
                                      new double[]{1,3,9},
                                      new double[]{1,4,16}};
     
      double[][] expectedU = new double[][]
       {
         new double[]{0.06694942772363194830335878, -0.6496907508990609956156238, 0.7234775064393449793067184}, 
         new double[]{0.2274012268163765838895085, -0.6075911990028880215461633, -0.3593349648122762034053743}, 
         new double[]{0.4855667535109152389501883, -0.2675906812177502659847019, -0.4925648741133399561928825}, 
         new double[]{0.8414460078072479134853980, 0.3703108024563522710687606, 0.3237877785361537209441937}       
       };

      double[] expectedSingularValues = new double[]{19.62136402366775701042283, 1.712069874450694805376275, 0.2662528792614848629364699};

      double[][] expectedV = new double[][]
        {
          new double[]{0.08263255367478247093846163, -0.6743660675845837168765756, 0.7337589985572162375998953}, 
          new double[]{0.2723682291746780408033352, -0.6929635293730795997776881, -0.6675455749947378000917574}, 
          new double[]{0.9586383096921561352973747, 0.2550136346341303246410893, 0.1264145456079166187153824}        
        };


      IMatrix X = MatrixMath.ToMatrix(x);

      MatrixMath.SingularValueDecomposition decomp = 
        MatrixMath.GetSingularValueDecomposition(X);


      
      for(int i=0;i<3;i++)
      {
        Assert.AreEqual(expectedSingularValues[i],decomp.Diagonal[i],accuracy,"SingularValue_"+i.ToString());
      }

       
      // Test the U Matrix
      for(int i=0;i<expectedU.Length;i++)
        for(int j=0;j<expectedU[0].Length;j++)
          Assert.AreEqual(Math.Abs(expectedU[i][j]),Math.Abs(decomp.U[i][j]),accuracy,string.Format("U[{0}][{1}]",i,j));


      // Test the V Matrix
      for(int i=0;i<expectedV.Length;i++)
        for(int j=0;j<expectedV[0].Length;j++)
          Assert.AreEqual(Math.Abs(expectedV[i][j]),Math.Abs(decomp.V[i][j]),accuracy,string.Format("V[{0}][{1}]",i,j));


    }

    [Test]
    public void Test03_3x4Matrix()
    {
      double[][] x = new double[3][]{
                                      new double[]{1,1,1,1},
                                      new double[]{1,2,4,8},
                                      new double[]{1,3,9,27}};

      double[][] expectedU = new double[][]
        {
          new double[]{0.04734386493551604995477145, -0.6298036597816627273392776, 0.7753102015184575401987200}, 
          new double[]{0.3019315922550259026382404, -0.7308495124910898427130366, -0.6121244184721608492599749}, 
          new double[]{0.9521532818046222520788651, 0.2630709794279101755469104, 0.1555563811983541387310737} 
        };

      double[] expectedSingularValues = new double[]{30.06856910125475242387046, 2.167597107698223914976808, 0.4274049388650818475368850,0};

      double[][] expectedV = new double[][]
       {
         new double[]{0.04328203096770760439694849, -0.5063589487856289461733263, 0.7457615372696186799476553}, 
         new double[]{0.1166555975127217050621376, -0.6007988024412046764051820, 0.04148409753120615124080518}, 
         new double[]{0.3267348618124714344785811, -0.5469479963247332138954797, -0.6391597680419712306939267}, 
         new double[]{0.9368897840411354775696647, 0.2889451561910784800441657, 0.1832855425226170318735145}     
       };


      IMatrix X = MatrixMath.ToMatrix(x);

      MatrixMath.SingularValueDecomposition decomp = 
        MatrixMath.GetSingularValueDecomposition(X);


      double[] calculatedSingularValues = decomp.Diagonal;
      for(int i=0;i<4;i++)
      {
        Assert.AreEqual(expectedSingularValues[i],calculatedSingularValues[i],accuracy,"SingularValue_"+i.ToString());
      }

      // Test the U Matrix
      for(int i=0;i<expectedU.Length;i++)
        for(int j=0;j<expectedU[0].Length;j++)
          Assert.AreEqual(Math.Abs(expectedU[i][j]),Math.Abs(decomp.U[i][j]),accuracy,string.Format("U[{0}][{1}]",i,j));


      // Test the V Matrix
      for(int i=0;i<expectedV.Length;i++)
        for(int j=0;j<expectedV[0].Length;j++)
          Assert.AreEqual(Math.Abs(expectedV[i][j]),Math.Abs(decomp.V[i][j]),accuracy,string.Format("V[{0}][{1}]",i,j));


    }


  }
}
