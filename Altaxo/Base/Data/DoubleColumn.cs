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
using Altaxo.Serialization;
using Altaxo;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for Altaxo.Data.DoubleColumn.
  /// </summary>
  [SerializationSurrogate(0,typeof(Altaxo.Data.DoubleColumn.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable()]
  public class DoubleColumn 
    :
    Altaxo.Data.DataColumn,
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback,
    INumericColumn
  {
    private double[] m_Array;
    private int      m_Capacity; // shortcut to m_Array.Length;
    private int        m_Count;
    public static readonly double NullValue = Double.NaN;
    
    public DoubleColumn()
    {
    }

    
  
    public DoubleColumn(int initialcapacity)
    {
      m_Count = 0;
      m_Array = new double[initialcapacity];
      m_Capacity = initialcapacity;
    }
  
    public DoubleColumn(DoubleColumn from)
    {
      this.m_Count    = from.m_Count; 
      this.m_Capacity = from.m_Capacity;
      this.m_Array    = null==from.m_Array ? null : (double[])from.m_Array.Clone();
    }

    public override object Clone()
    {
      return new DoubleColumn(this);
    }


    #region "Serialization"
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context)
      {
        Altaxo.Data.DoubleColumn s = (Altaxo.Data.DoubleColumn)obj;
        // get the surrogate selector of the base class
        System.Runtime.Serialization.ISurrogateSelector ss  = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
  
          // serialize the base class
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
        else
        {
          ((DataColumn)s).GetObjectData(info,context);
        }

        if(s.m_Count!=s.m_Capacity)
        {
          // instead of the data array itself, stream only the first m_Count
          // array elements, since only they contain data
          double[] streamarray = new Double[s.m_Count];
          System.Array.Copy(s.m_Array,streamarray,s.m_Count);
          info.AddValue("Data",streamarray);
        }
        else // if the array is fully filled, we don't need to save a shrinked copy
        {
          info.AddValue("Data",s.m_Array);
        }
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        Altaxo.Data.DoubleColumn s = (Altaxo.Data.DoubleColumn)obj;
        // get the surrogate selector of the base class

        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(obj.GetType().BaseType,context, out ss);
          // deserialize the base class
          surr.SetObjectData(obj,info,context,selector);
        }
        else
        {
          ((DataColumn)s).SetObjectData(obj,info,context,selector);
        }

        s.m_Array = (double[])(info.GetValue("Data",typeof(double[])));
        s.m_Capacity = null==s.m_Array ? 0 : s.m_Array.Length;
        s.m_Count = s.m_Capacity;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DoubleColumn),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Altaxo.Data.DoubleColumn s = (Altaxo.Data.DoubleColumn)obj;
        // serialize the base class
        info.AddBaseValueEmbedded(s,typeof(Altaxo.Data.DataColumn));
        
        if(null==info.GetProperty("Altaxo.Data.DataColumn.SaveAsTemplate"))
          info.AddArray("Data",s.m_Array,s.m_Count);
        else
          info.AddArray("Data",s.m_Array,0);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DoubleColumn s = null!=o ? (Altaxo.Data.DoubleColumn)o : new Altaxo.Data.DoubleColumn();

        
        // deserialize the base class
        info.GetBaseValueEmbedded(s,typeof(Altaxo.Data.DataColumn),parent);

        int count = info.GetInt32Attribute("Count");
        s.m_Array = new double[count];
        info.GetArray(s.m_Array,count);
        s.m_Capacity = null==s.m_Array ? 0 : s.m_Array.Length;
        s.m_Count = s.m_Capacity;

        return s;
      }
    }

    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
  
    protected DoubleColumn(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      SetObjectData(this,info,context,null);
    }
    public new object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
    {
      return new SerializationSurrogate0().SetObjectData(this,info,context,null);
    }
    public new void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      new SerializationSurrogate0().GetObjectData(this,info,context);
    }

    #endregion

    public override int Count
    {
      get
      {
        return m_Count;
      }
    }

    public double[] Array
    {
      get 
      {
        int len = this.Count;
        double[] arr = new double[len];
        System.Array.Copy(m_Array,0,arr,0,len);
        return arr;
      }

      set
      {
        m_Array = (double[])value.Clone();
        this.m_Count = m_Array.Length;
        this.m_Capacity = m_Array.Length;
        this.NotifyDataChanged(0,m_Count,true);
      }
    }

    protected internal double GetValueDirect(int idx)
    {
      return m_Array[idx];
    }

    public override System.Type GetColumnStyleType()
    {
      return typeof(Altaxo.Worksheet.DoubleColumnStyle);
    }
        
        
    public override void CopyDataFrom(Altaxo.Data.DataColumn v)
    {
      if(v.GetType()!=typeof(Altaxo.Data.DoubleColumn))
      {
        throw new ArgumentException("Try to copy " + v.GetType() + " to " + this.GetType(),"v"); // throw exception
      }

      this.CopyDataFrom(((Altaxo.Data.DoubleColumn)v).m_Array,v.Count);
    }       

    /// <summary>
    /// Returns the used length of the array. This is one plus the highest index of the number different from Double.NaN.
    /// </summary>
    /// <param name="values">The array for which the used length has to be determined.</param>
    /// <param name="currentlength">The current length of the array. Normally values.Length, but you can provide a value less than this.</param>
    /// <returns>The used length, i.e. numbers above the used length until the end of the array are NaNs.</returns>
    static public int GetUsedLength(double[] values, int currentlength)
    {
      for(int i=currentlength-1;i>=0;i--)
      {
        if(!Double.IsNaN(values[i]))
          return i+1;
      }
      return 0;
    }


    /// <summary>
    /// Copies the data from an array into the column. All data in the source array is copied.
    /// </summary>
    /// <param name="srcarray">The source array.</param>
    public void CopyDataFrom(double[] srcarray)
    {
      CopyDataFrom(srcarray,srcarray.Length);
    }

    /// <summary>
    /// Copies the data from an array into the column. The data from index 0 until <c>count-1</c> is copied to the destination.
    /// </summary>
    /// <param name="srcarray">Array containing the source data.</param>
    /// <param name="count">Length of the array (or length of the used range of the array, starting from index 0).</param>
    public void CopyDataFrom(double[] srcarray, int count)
    {
      int oldCount = this.m_Count;
      int srcarraycount=0;

      if(null==srcarray || 0==(srcarraycount=GetUsedLength(srcarray,Math.Min(srcarray.Length,count))))
      {
        m_Array=null;
        m_Capacity=0;
        m_Count=0;
      }
      else
      {
        if(m_Capacity<srcarraycount)
          m_Array = new double[srcarraycount];
        System.Array.Copy(srcarray,m_Array,srcarraycount);
        m_Capacity = m_Array.Length;
        m_Count = srcarraycount;
      }
      if(oldCount>0 || m_Count>0) // message only if really was a change
        NotifyDataChanged(0,oldCount>m_Count? (oldCount):(m_Count),m_Count<oldCount);
    }

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(m_Capacity*increaseFactor+addSpace);
      int newcapacity2 = i+addSpace+1;
      int newcapacity = newcapacity1>newcapacity2 ? newcapacity1:newcapacity2;
        
      double[] newarray = new double[newcapacity];
      if(m_Count>0)
      {
        System.Array.Copy(m_Array,newarray,m_Count);
      }

      m_Array = newarray;
      m_Capacity = m_Array.Length;
    }

    // indexers
    public override void SetValueAt(int i, AltaxoVariant val)
    {
      if(val.IsType(AltaxoVariant.Content.VDouble))
        this[i] = val.m_Double;
      else if(val.CanConvertedToDouble)
        this[i] = val.ToDouble();
      else
        throw new ApplicationException("Error: Try to set " + this.TypeAndName + "[" + i + "] with the string " + val.ToString());
    }

    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant(this[i]);
    }

   

    public override bool IsElementEmpty(int i)
    {
      return i<m_Count ? Double.IsNaN(m_Array[i]) : true;
    }
    public override void SetElementEmpty(int i)
    {
      if (i < m_Count)
        this[i] = NullValue;
    }

    public new double this[int i]
    {
      get
      {
        if(i>=0 && i<m_Count)
          return m_Array[i];
        return double.NaN;  
      }
      set
      {
        bool bCountDecreased=false;


        if(Double.IsNaN(value))
        {
          if(i>=0 && i<m_Count-1) // i is inside the used range
          {
            m_Array[i]=value;
          }
          else if(i==(m_Count-1)) // m_Count is then decreasing
          {
            for(m_Count=i; m_Count>0 && Double.IsNaN(m_Array[m_Count-1]); --m_Count);
            bCountDecreased=true;;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is not NaN
        {
          if(i>=0 && i<m_Count) // i is inside the used range
          {
            m_Array[i]=value;
          }
          else if(i==m_Count && i<m_Capacity) // i is the next value after the used range
          {
            m_Array[i]=value;
            m_Count=i+1;
          }
          else if(i>m_Count && i<m_Capacity) // is is outside used range, but inside capacity of array
          {
            for(int k=m_Count;k<i;k++)
              m_Array[k]=Double.NaN; // fill range between used range and new element with voids
          
            m_Array[i]=value;
            m_Count=i+1;
          }
          else if(i>=0) // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for(int k=m_Count;k<i;k++)
              m_Array[k]=Double.NaN; // fill range between used range and new element with voids
          
            m_Array[i]=value;
            m_Count=i+1;
          }
        }
        NotifyDataChanged(i,i+1,bCountDecreased);
      } // end set  
    } // end indexer


    public override void InsertRows(int nInsBeforeColumn, int nInsCount)
    {
      if(nInsCount<=0 || nInsBeforeColumn>=Count)
        return; // nothing to do

      int newlen = this.m_Count + nInsCount;
      if(newlen>m_Capacity)
        Realloc(newlen);

      // copy values from m_Count downto nBeforeColumn 
      for(int i=m_Count-1, j=newlen-1; i>=nInsBeforeColumn;i--,j--)
        m_Array[j] = m_Array[i];

      for(int i=nInsBeforeColumn+nInsCount-1;i>=nInsBeforeColumn;i--)
        m_Array[i]=NullValue;
    
      this.m_Count=newlen;
      this.NotifyDataChanged(nInsBeforeColumn,m_Count,false);
    }

    public override void RemoveRows(int nDelFirstRow, int nDelCount)
    {
      if(nDelFirstRow<0)
        throw new ArgumentException("Row number must be greater or equal 0, but was " + nDelFirstRow.ToString(), "nDelFirstRow");

      if(nDelCount<=0)
        return; // nothing to do here, but we dont catch it

      // we must be careful, since the range to delete can be
      // above the range this column actually holds, but
      // we must handle this the right way
      int i,j;
      for(i=nDelFirstRow,j=nDelFirstRow+nDelCount;j<m_Count;i++,j++)
        m_Array[i]=m_Array[j];
      
      int prevCount = m_Count;
      m_Count= i<m_Count ? i : m_Count; // m_Count can only decrease

      if(m_Count!=prevCount) // raise a event only if something really changed
        this.NotifyDataChanged(nDelFirstRow,prevCount,true);
    }



    #region "Operators"

    // -----------------------------------------------------------------------------
    //
    //                        Operators
    //
    // -----------------------------------------------------------------------------


    // ----------------------- Addition operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] + c2.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c1.m_Array[i] + c2;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator +(double c2, Altaxo.Data.DoubleColumn c1)
    {
      return c1+c2;
    }


    public override bool vop_Addition(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this + (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }
    public override bool vop_Addition_Rev(DataColumn c2, out DataColumn c3)
    {
      return vop_Addition(c2, out c3);
    }

    public override bool vop_Addition(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this + c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Addition_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      return vop_Addition(c2, out c3);
    }

    // --------------------- Operator Subtract -------------------------------------


    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] - c2.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i]-c2;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator -(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c2 - c1.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }
    

    public override bool vop_Subtraction(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this - (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Subtraction_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 - this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Subtraction(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this - c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Subtraction_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 - this;
        return true;
      }
      c3=null;
      return false;
    }


    
    public static Altaxo.Data.DoubleColumn Subtraction(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DateTimeColumn c2)
    {
      int len = c1.Count<c2.Count ? c1.Count : c2.Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = (c1.GetValueDirect(i)-c2.GetValueDirect(i)).TotalSeconds;
      }
      
      c3.m_Count=len;
      
      return c3;  
    }


    public static Altaxo.Data.DoubleColumn Subtraction(Altaxo.Data.DateTimeColumn c1, DateTime c2)
    {
      int len = c1.Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3.m_Array[i] = (c1.GetValueDirect(i) - c2).TotalSeconds;
      }

      c3.m_Count = len;

      return c3;
    }

    public static Altaxo.Data.DoubleColumn Subtraction(DateTime c1, Altaxo.Data.DateTimeColumn c2)
    {
      int len = c2.Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for (int i = 0; i < len; i++)
      {
        c3.m_Array[i] = (c1 - c2.GetValueDirect(i)).TotalSeconds;
      }

      c3.m_Count = len;

      return c3;
    }

    // ----------------------- Multiplication operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator *(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] * c2.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator *(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c1.m_Array[i] * c2;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator *(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c1.m_Array[i] * c2;
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_Multiplication(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this * (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }
    public override bool vop_Multiplication_Rev(DataColumn c2, out DataColumn c3)
    {
      return vop_Multiplication(c2, out c3);
    }

    public override bool vop_Multiplication(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this * c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Multiplication_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      return vop_Multiplication(c2, out c3);
    }

    // ------------------------ Division operator --------------------------------

    public static Altaxo.Data.DoubleColumn operator /(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] / c2.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator /(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i]/c2;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator /(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c2/c1.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public override bool vop_Division(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this / (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Division_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 / this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Division(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this / c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Division_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 / this;
        return true;
      }
      c3=null;
      return false;
    }


    // -------------------------- operator % ----------------------------------------------
    public static Altaxo.Data.DoubleColumn operator %(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] % c2.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator %(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i] % c2;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator %(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c2%c1.m_Array[i];
      }
      c3.m_Count=len;
      return c3;  
    }

    public override bool vop_Modulo(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this % (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Modulo_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 % this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Modulo(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this % c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Modulo_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 % this;
        return true;
      }
      c3=null;
      return false;
    }


    // ----------------------- AND operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator &(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = ((long)c1.m_Array[i]) & ((long)c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator &(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ((long)c1.m_Array[i]) & c22;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator &(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c22 & ((long)c1.m_Array[i]);
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_And(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this & (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_And_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 & this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_And(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this & c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_And_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 & this;
        return true;
      }
      c3=null;
      return false;
    }

    // ----------------------- OR operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator |(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = ((long)c1.m_Array[i]) | ((long)c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator |(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ((long)c1.m_Array[i]) | c22;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator |(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c22 | ((long)c1.m_Array[i]);
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_Or(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this | (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Or_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 | this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Or(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this | c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Or_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 | this;
        return true;
      }
      c3=null;
      return false;
    }



    // ----------------------- XOR operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator ^(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = ((long)c1.m_Array[i]) ^ ((long)c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator ^(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ((long)c1.m_Array[i]) ^ c22;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator  ^(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      long c22 = (long)c2;
      for(int i=0;i<len;i++)
        c3.m_Array[i] = c22 ^ ((long)c1.m_Array[i]);
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_Xor(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this ^ (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Xor_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 ^ this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Xor(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this ^ c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Xor_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 ^ this;
        return true;
      }
      c3=null;
      return false;
    }

    // ----------------------- ShiftLeft operator -----------------------------------
  
    public static Altaxo.Data.DoubleColumn operator <<(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ((long)c1.m_Array[i]) << c2;
      c3.m_Count = len;
      return c3;
    }



    public override bool vop_ShiftLeft(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1=this;
        Altaxo.Data.DoubleColumn c22 = (DoubleColumn)c2;
        int len = c1.Count<c2.Count ? c1.Count : c2.Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        for(int i=0;i<len;i++)
        {
          c33.m_Array[i] = ((long)c1.m_Array[i]) << ((int)c22.m_Array[i]);
        }
        c33.m_Count=len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftLeft_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1=this;
        Altaxo.Data.DoubleColumn c22= (DoubleColumn)c2;

        int len = c1.Count<c2.Count ? c1.Count : c2.Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        for(int i=0;i<len;i++)
        {
          c33.m_Array[i] = ((long)c22.m_Array[i]) << ((int)c1.m_Array[i]);
        }
        c33.m_Count=len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftLeft(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        int c22 = (int)(double)c2;
        c3 = this << c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftLeft_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1=this;
        int len = c1.m_Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)c2;
        for(int i=0;i<len;i++)
          c33.m_Array[i] = c22 << ((int)c1.m_Array[i]);
        c33.m_Count = len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }


    // ----------------------- ShiftRight operator -----------------------------------

    public static Altaxo.Data.DoubleColumn operator >>(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ((long)c1.m_Array[i]) >> c2;
      c3.m_Count = len;
      return c3;
    }



    public override bool vop_ShiftRight(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        DoubleColumn c1=this;
        DoubleColumn c22 = (DoubleColumn)c2;
        int len = c1.Count<c2.Count ? c1.Count : c2.Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        for(int i=0;i<len;i++)
        {
          c33.m_Array[i] = ((long)c1.m_Array[i]) >> ((int)c22.m_Array[i]);
        }
        c33.m_Count=len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftRight_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        Altaxo.Data.DoubleColumn c1=this;
        DoubleColumn c22 = (DoubleColumn)c2;
        int len = c1.Count<c2.Count ? c1.Count : c2.Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        for(int i=0;i<len;i++)
        {
          c33.m_Array[i] = ((long)c22.m_Array[i]) >> ((int)c1.m_Array[i]);
        }
        c33.m_Count=len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftRight(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1=this;
        int len = c1.m_Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        int c22 = (int)(double)c2;
        for(int i=0;i<len;i++)
          c33.m_Array[i] = ((long)c1.m_Array[i]) >> c22;
        c33.m_Count = len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_ShiftRight_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        DoubleColumn c1=this;
        int len = c1.m_Count;
        Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
        long c22 = (long)(double)c2;
        for(int i=0;i<len;i++)
          c33.m_Array[i] = c22 >> ((int)c1.m_Array[i]);
        c33.m_Count = len;
        c3=c33;
        return true;
      }
      c3=null;
      return false;
    }



    // ----------------------- Lesser operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator <(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = (c1.m_Array[i] < c2.m_Array[i]) ? 1 : 0;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator <(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c1.m_Array[i] < c2 ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator  <(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c2 < c1.m_Array[i] ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_Lesser(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this < (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Lesser_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 < this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Lesser(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this < c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Lesser_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 < this;
        return true;
      }
      c3=null;
      return false;
    }


    // ----------------------- Greater operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator >(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = (c1.m_Array[i] > c2.m_Array[i]) ? 1 : 0;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator >(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c1.m_Array[i] > c2 ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c2 > c1.m_Array[i] ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_Greater(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this > (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Greater_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 > this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Greater(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this > c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_Greater_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 > this;
        return true;
      }
      c3=null;
      return false;
    }




    // ----------------------- LesserOrEqual operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator <=(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = (c1.m_Array[i] <= c2.m_Array[i]) ? 1 : 0;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator <=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c1.m_Array[i] <= c2 ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator <=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c2 <= c1.m_Array[i] ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_LesserOrEqual(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this <= (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_LesserOrEqual_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 <= this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_LesserOrEqual(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this <= c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_LesserOrEqual_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 <= this;
        return true;
      }
      c3=null;
      return false;
    }


    // ----------------------- GreaterOrEqual operator -----------------------------------
    public static Altaxo.Data.DoubleColumn operator >=(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = (c1.m_Array[i] >= c2.m_Array[i]) ? 1 : 0;
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator >=(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c1.m_Array[i] >= c2 ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }

    public static Altaxo.Data.DoubleColumn operator >=(double c2, Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
        c3.m_Array[i] = ( c2 >= c1.m_Array[i] ) ? 1 : 0;
      c3.m_Count = len;
      return c3;
    }


    public override bool vop_GreaterOrEqual(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = this >= (Altaxo.Data.DoubleColumn)c2;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_GreaterOrEqual_Rev(DataColumn c2, out DataColumn c3)
    {
      if(c2 is Altaxo.Data.DoubleColumn)
      {
        c3 = (Altaxo.Data.DoubleColumn)c2 >= this;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_GreaterOrEqual(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = this >= c22;
        return true;
      }
      c3=null;
      return false;
    }

    public override bool vop_GreaterOrEqual_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if(((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDouble))
      {
        double c22 = (double)c2;
        c3 = c22 >= this;
        return true;
      }
      c3=null;
      return false;
    }


    // --------------------------------- Unary Plus ----------------------------
    public static Altaxo.Data.DoubleColumn operator +(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i];
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Plus(out DataColumn c3)
    {
      c3= +this;
      return true;
    }



    // --------------------------------- Unary Minus ----------------------------
    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = -c1.m_Array[i];
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Minus(out DataColumn c3)
    {
      c3= -this;
      return true;
    }

    // --------------------------------- Unary NOT ----------------------------
    public static Altaxo.Data.DoubleColumn operator !(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = 0==c1.m_Array[i] ? 1 : 0;
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Not(out DataColumn c3)
    {
      c3= !this;
      return true;
    }

    // --------------------------------- Unary Complement ----------------------------
    public static Altaxo.Data.DoubleColumn operator ~(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = ~((long)c1.m_Array[i]);
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Complement(out DataColumn c3)
    {
      c3= ~this;
      return true;
    }

    // --------------------------------- Unary Increment ----------------------------
    public static Altaxo.Data.DoubleColumn operator ++(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i]+1;
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Increment(out DataColumn c3)
    {
      int len = this.m_Count;
      Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c33.m_Array[i] = this.m_Array[i]+1;
      }
      c33.m_Count = len;
      c3=c33;
      return true;
    }

    // --------------------------------- Unary Decrement ----------------------------
    public static Altaxo.Data.DoubleColumn operator --(Altaxo.Data.DoubleColumn c1)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i]-1;
      }
      c3.m_Count = len;
      return c3;  
    }


    public override bool vop_Decrement(out DataColumn c3)
    {
      int len = this.m_Count;
      Altaxo.Data.DoubleColumn c33 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c33.m_Array[i] = this.m_Array[i]-1;
      }
      c33.m_Count = len;
      c3=c33;
      return true;
    }


    // -----------------------------------------------------------------------------
    //
    //               arithmetic Functions
    //
    // -----------------------------------------------------------------------------

    public static Altaxo.Data.DoubleColumn Abs(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Abs(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Acos(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Acos(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Asin(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Asin(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 


    public static Altaxo.Data.DoubleColumn Atan(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Atan(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Atan2(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Atan2(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Atan2(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Atan2(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Atan2(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }


    public static Altaxo.Data.DoubleColumn Ceiling(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Ceiling(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Cos(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Cos(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Cosh(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Cosh(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Exp(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Exp(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Floor(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Floor(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.IEEERemainder(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.IEEERemainder(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn IEEERemainder(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.IEEERemainder(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

  
    
    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Log(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Log(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Log(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Log(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Log(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Log(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Max(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Max(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Max(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Max(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Max(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    
    
    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Min(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Min(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Min(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Min(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Min(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    
    
    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Pow(c1.m_Array[i],c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Pow(Altaxo.Data.DoubleColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Pow(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Pow(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Pow(c1,c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    
    
    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Round(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.m_Count<c2.m_Count ? c1.m_Count : c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Round(c1.m_Array[i],(int)c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Round(Altaxo.Data.DoubleColumn c1, int c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Round(c1.m_Array[i],c2);
      }
      c3.m_Count=len;
      return c3;  
    }

    public static Altaxo.Data.DoubleColumn Round(double c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c2.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Round(c1,(int)c2.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;  
    }
    
    public static Altaxo.Data.DoubleColumn Sign(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Sign(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Sin(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Sin(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Sinh(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Sinh(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Sqrt(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Sqrt(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Tan(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Tan(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 

    public static Altaxo.Data.DoubleColumn Tanh(Altaxo.Data.DoubleColumn c1)
    {
      int len=c1.m_Count;
      Altaxo.Data.DoubleColumn c3 = new Altaxo.Data.DoubleColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = System.Math.Tanh(c1.m_Array[i]);
      }
      c3.m_Count=len;
      return c3;
    } 
        
    #endregion

  } // end Altaxo.Data.DoubleColumn
}
