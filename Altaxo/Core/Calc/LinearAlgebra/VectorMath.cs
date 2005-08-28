#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Calc.LinearAlgebra
{
  /// <summary>
  /// VectorMath provides common static functions concerning vectors.
  /// </summary>
  public class VectorMath
  {
    #region private Helper functions
    static double Square(double x) { return x*x; }
    #endregion
  
    #region Inner types

    /// <summary>
    /// Serves as Wrapper for an double array to plug-in where a IROVector is neccessary.
    /// </summary>
    private class RODoubleArrayWrapper : IROVector
    {
      protected double[] _x;
      int _length;
      
      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      public RODoubleArrayWrapper(double[] x)
      {
        _x = x;
        _length = _x.Length;
      }

      /// <summary>
      /// Constructor, takes a double array for wrapping.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="usedlength">The length used for the vector.</param>
      public RODoubleArrayWrapper(double[] x, int usedlength)
      {
        if(usedlength>x.Length)
          throw new ArgumentException("Length provided in argument usedlength is greater than length of array");

        _x = x;
        _length = usedlength;
      }

      /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
      /// <value>The element at index i.</value>
      public double this[int i] { get { return _x[i]; }}
 
      /// <summary>The smallest valid index of this vector</summary>
      public int LowerBound { get { return _x.GetLowerBound(0); }}
    
      /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
      public int UpperBound { get { return _x.GetUpperBound(0); }}
    
      /// <summary>The number of elements of this vector.</summary>
      public int Length { get { return _length; }}  // change this later to length property
    }

    private class RWDoubleArrayWrapper : RODoubleArrayWrapper, IVector
    {
      public RWDoubleArrayWrapper(double[] x)
        : base(x)
      {
      }

      #region IVector Members

      public new double this[int i]
      {
        get { return _x[i]; }
        set { _x[i] = value; }
      }

      #endregion
    }


    private class ExtensibleVector : IExtensibleVector  
    {
      double[] _arr;
      int      _length;

      public ExtensibleVector(int initiallength)
      {
        _arr = new double[initiallength];
        _length = initiallength;
      }


      #region IVector Members

      public double this[int i]
      {
        get
        {
          
          return _arr[i];
        }
        set
        {
          _arr[i] = value;
        }
      }

      #endregion

      #region IROVector Members

      double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
      {
        get
        {
         
          return _arr[i];
        }
      }

      public int LowerBound
      {
        get
        {
          
          return 0;
        }
      }

      public int UpperBound
      {
        get
        {
          
          return _length-1;
        }
      }

      public int Length
      {
        get
        {
         
          return _length;
        }
      }

      #endregion

      #region IExtensibleVector Members
      public void Append(IROVector a)
      {
        if(_length+a.Length>=_arr.Length)
          Redim((int)(32+1.3*(_length+a.Length)));


        for(int i=0;i<a.Length;i++)
          _arr[i+_length] = a[i+a.LowerBound];
        _length += a.Length;
      }
      #endregion

  
      private void Redim(int newsize)
      {
        if(newsize>_arr.Length)
        {
          double[] oldarr = _arr;
          _arr = new double[newsize];
          Array.Copy(oldarr,0,_arr,0,_length);
        }
      }
    }



    #endregion

    #region Type conversion
    /// <summary>
    /// Wraps a double[] array to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <returns>A wrapper objects with the <see>IROVector</see> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(double[] x)
    {
      return new RODoubleArrayWrapper(x);
    }

    /// <summary>
    /// Wraps a double[] array till a given length to get a IROVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <param name="usedlength">Length of the resulting vector. Can be equal or less the length of the array.</param>
    /// <returns>A wrapper objects with the <see>IROVector</see> interface that wraps the provided array.</returns>
    public static IROVector ToROVector(double[] x, int usedlength)
    {
      return new RODoubleArrayWrapper(x,usedlength);
    }

    /// <summary>
    /// Wraps a double[] array to get a IVector.
    /// </summary>
    /// <param name="x">The array to wrap.</param>
    /// <returns>A wrapper objects with the <see>IVector</see> interface that wraps the provided array.</returns>
    public static IVector ToVector(double[] x)
    {
      return new RWDoubleArrayWrapper(x);
    }

    /// <summary>
    /// Creates a new extensible vector of length <c>length</c>
    /// </summary>
    /// <param name="length">The inital length of the vector.</param>
    /// <returns>An instance of a extensible vector.</returns>
    public static IExtensibleVector CreateExtensibleVector(int length)
    {
      return new ExtensibleVector(length);
    }
    #endregion

    #region Filling

    /// <summary>
    /// Fills a vector with a certain value.
    /// </summary>
    /// <param name="x">The vector to fill.</param>
    /// <param name="val">The value each element is set to.</param>
    public static void Fill(IVector x, double val)
    {
      int end = x.UpperBound;
      for(int i=x.LowerBound;i<=end;i++) x[i]=val;
    }

    #endregion

    #region copy 

    /// <summary>
    /// Copies the source vector to the destination vector. Both vectors must have the same length.
    /// </summary>
    /// <param name="src">The source vector.</param>
    /// <param name="dest">The destination vector.</param>
    public static void Copy(IROVector src, IVector dest)
    {
      if(src.Length!=dest.Length)
        throw new ArgumentException("src and destination vector have unequal length!");

      Copy(src,src.LowerBound,dest,dest.LowerBound,src.Length);
    }

    /// <summary>
    /// Copies elements of a source vector to a destination vector.
    /// </summary>
    /// <param name="src">The source vector.</param>
    /// <param name="srcstart">First element of the source vector to copy.</param>
    /// <param name="dest">The destination vector.</param>
    /// <param name="deststart">First element of the destination vector to copy to.</param>
    /// <param name="count">Number of elements to copy.</param>
    public static void Copy(IROVector src, int srcstart, IVector dest, int deststart, int count)
    {
      for(int i=0;i<count;i++)
        dest[i+deststart] = src[i+srcstart];
    }

    #endregion

    #region Arithmitic

    /// <summary>
    /// Adds (elementwise) two vectors a and b and stores the result in c. All vectors must have the same length.
    /// </summary>
    /// <param name="a">First summand.</param>
    /// <param name="b">Second summand.</param>
    /// <param name="c">The resulting vector.</param>
    public static void Add(IROVector a, IROVector b, IVector c)
    {
      if(a.Length != b.Length)
        throw new ArgumentException("Length of vectors a and b unequal");
      if(c.Length != b.Length)
        throw new ArgumentException("Length of vectors a and c unequal");
      if(a.LowerBound != b.LowerBound || a.LowerBound != c.LowerBound)
        throw new ArgumentException("Vectors a, b, and c have not the same LowerBound property");

      int end = c.UpperBound;
      for(int i=c.LowerBound;i<=end;i++)
        c[i]=a[i]+b[i];
    }

    #endregion

    /// <summary>
    /// Returns the maximum of the elements in xarray.
    /// </summary>
    /// <param name="xarray">The array to search for maximum element.</param>
    /// <returns>Maximum element of xarray. Returns NaN if the array is empty.</returns>
    public static double Max(IROVector xarray)
    {
      double max = xarray.Length==0 ? double.NaN : xarray[xarray.LowerBound];

      int last = xarray.UpperBound;
      for(int i=xarray.LowerBound+1;i<=last;i++)
      {
        max = Math.Max(max,xarray[i]);
      }
      return max;
    }

    /// <summary>
    /// Returns the maximum of the elements in xarray.
    /// </summary>
    /// <param name="xarray">The array to search for maximum element.</param>
    /// <returns>Maximum element of xarray. Returns <see>Double.NaN</see> if the array is empty.</returns>
    public static double Max(double[] xarray)
    {
      double max = xarray.Length==0 ? double.NaN : xarray[xarray.GetLowerBound(0)];

      int last = xarray.GetUpperBound(0);
      for(int i=xarray.GetLowerBound(0)+1;i<=last;i++)
      {
        max = Math.Max(max,xarray[i]);
      }
      return max;
    }

    /// <summary>
    /// Returns the sum of the elements in xarray.
    /// </summary>
    /// <param name="xarray">The array.</param>
    /// <returns>The sum of all elements in xarray.</returns>
    public static double Sum(double[] xarray)
    {
      double sum = 0;
      for(int i=0;i<xarray.Length;i++)
        sum += xarray[i];

      return sum;
    }


    /// <summary>
    /// Returns the sum of squared differences of the elements of xarray and yarray.
    /// </summary>
    /// <param name="xarray">The first array.</param>
    /// <param name="yarray">The other array.</param>
    /// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
    public static double SumOfSquaredDifferences(double[] xarray, double[] yarray)
    {
      if(xarray.Length!=yarray.Length)
        throw new ArgumentException("Length of xarray is unequal length of yarray");

      double sum = 0;
      for(int i=0;i<xarray.Length;i++)
        sum += Square(xarray[i]-yarray[i]);

      return sum;
    }

    /// <summary>
    /// Returns the sum of squared differences of the elements of xarray and yarray.
    /// </summary>
    /// <param name="xarray">The first array.</param>
    /// <param name="yarray">The other array.</param>
    /// <returns>The sum of squared differences all elements of xarray and yarray.</returns>
    public static double SumOfSquaredDifferences(IROVector xarray, IROVector yarray)
    {
      if(xarray.Length!=yarray.Length)
        throw new ArgumentException("Length of xarray is unequal length of yarray");

      double sum = 0;
      for(int i=0;i<xarray.Length;i++)
        sum += Square(xarray[i]-yarray[i]);

      return sum;
    }

  }
}
