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
  /// Summary description for Altaxo.Data.DateTimeColumn.
  /// </summary>
  [SerializationSurrogate(0,typeof(Altaxo.Data.DateTimeColumn.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable]
  public class DateTimeColumn 
    :
    Altaxo.Data.DataColumn, 
    System.Runtime.Serialization.ISerializable,
    System.Runtime.Serialization.IDeserializationCallback, 
    INumericColumn
  {
    private DateTime[] m_Array;
    private int        m_Capacity; // shortcut to m_Array.Length;
    private int        m_Count;
    public static readonly DateTime NullValue = DateTime.MinValue;
  
    public DateTimeColumn()
    {
    }

  
    public DateTimeColumn(int initialcapacity)
    {
      m_Count = 0;
      m_Array = new DateTime[initialcapacity];
      m_Capacity = initialcapacity;
    }
        
    public DateTimeColumn(DateTimeColumn from)
    {
      this.m_Count    = from.m_Count;
      this.m_Capacity = from.m_Capacity;
      this.m_Array    = null==from.m_Array ? null : (DateTime[])from.m_Array.Clone();
    }

    public override object Clone()
    {
      return new DateTimeColumn(this);
    }


    #region "Serialization"
    public new class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        Altaxo.Data.DateTimeColumn s = (Altaxo.Data.DateTimeColumn)obj;
        System.Runtime.Serialization.ISurrogateSelector ss = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(typeof(Altaxo.Data.DataColumn),context, out ss);
  
          surr.GetObjectData(obj,info,context); // stream the data of the base object
        }
      {
        ((DataColumn)s).GetObjectData(info,context);
      }

        if(s.m_Count!=s.m_Capacity)
        {
          // instead of the data array itself, stream only the first m_Count
          // array elements, since only they contain data
          DateTime[] streamarray = new DateTime[s.m_Count];
          System.Array.Copy(s.m_Array,streamarray,s.m_Count);
          info.AddValue("Data",streamarray);
        }
        else // if the array is fully filled, we don't need to save a shrinked copy
        {
          info.AddValue("Data",s.m_Array);
        }
      }
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        Altaxo.Data.DateTimeColumn s = (Altaxo.Data.DateTimeColumn)obj;
        System.Runtime.Serialization.ISurrogateSelector ss  = AltaxoStreamingContext.GetSurrogateSelector(context);
        if(null!=ss)
        {
          System.Runtime.Serialization.ISerializationSurrogate surr =
            ss.GetSurrogate(typeof(Altaxo.Data.DataColumn),context, out ss);
          surr.SetObjectData(obj,info,context,selector);
        }
        else
        {
          ((DataColumn)s).SetObjectData(obj,info,context,selector);
        }

        s.m_Array = (DateTime[])(info.GetValue("Data",typeof(DateTime[])));
        s.m_Capacity = null==s.m_Array ? 0 : s.m_Array.Length;
        s.m_Count = s.m_Capacity;
        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.DateTimeColumn),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Altaxo.Data.DateTimeColumn s = (Altaxo.Data.DateTimeColumn)obj;
        // serialize the base class
        info.AddBaseValueEmbedded(s,typeof(Altaxo.Data.DataColumn));
        
        if(null==info.GetProperty("Altaxo.Data.DataColumn.SaveAsTemplate"))
          info.AddArray("Data",s.m_Array,s.m_Count);
        else
          info.AddArray("Data",s.m_Array,0);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DateTimeColumn s = null!=o ? (Altaxo.Data.DateTimeColumn)o : new Altaxo.Data.DateTimeColumn();
        
        

        // deserialize the base class
        info.GetBaseValueEmbedded(s,typeof(Altaxo.Data.DataColumn),parent);

        int count = info.GetInt32Attribute("Count");
        s.m_Array = new DateTime[count];
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

    protected DateTimeColumn(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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


    public DateTime[] Array
    {
      get 
      {
        int len = this.Count;
        DateTime[] arr = new DateTime[len];
        System.Array.Copy(m_Array,0,arr,0,len);
        return arr;
      }

      set
      {
        m_Array = (DateTime[])value.Clone();
        this.m_Count = m_Array.Length;
        this.m_Capacity = m_Array.Length;
        this.NotifyDataChanged(0,m_Count,true);
      }
    }

    protected internal DateTime GetValueDirect(int idx)
    {
      return m_Array[idx];
    }

    public override System.Type GetColumnStyleType()
    {
      return typeof(Altaxo.Worksheet.DateTimeColumnStyle);
    }

      
    public override void CopyDataFrom(Altaxo.Data.DataColumn v)
    {
      if(v.GetType()!=typeof(Altaxo.Data.DateTimeColumn))
      {
        throw new ArgumentException("Try to copy " + v.GetType() + " to " + this.GetType(),"v"); // throw exception
      }
      Altaxo.Data.DateTimeColumn vd = (Altaxo.Data.DateTimeColumn)v;

      // suggestion, but __not__ implemented:
      // if v is a standalone column, then simply take the dataarray
      // otherwise: copy the data by value  
      int oldCount = this.m_Count;
      if(null==vd.m_Array || vd.m_Count==0)
      {
        m_Array=null;
        m_Capacity=0;
        m_Count=0;
      }
      else
      {
        m_Array = (DateTime[])vd.m_Array.Clone();
        m_Capacity = m_Array.Length;
        m_Count = ((Altaxo.Data.DateTimeColumn)v).m_Count;
      }
      if(oldCount>0 || m_Count>0) // message only if really was a change
        NotifyDataChanged(0, oldCount>m_Count? (oldCount):(m_Count),m_Count<oldCount);
    }       

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(m_Capacity*increaseFactor+addSpace);
      int newcapacity2 = i+addSpace+1;
      int newcapacity = newcapacity1>newcapacity2 ? newcapacity1:newcapacity2;
        
      DateTime[] newarray = new DateTime[newcapacity];
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
      if(val.IsTypeOrNull(AltaxoVariant.Content.VDateTime))
        this[i] = (DateTime)val.m_Object;
      else
        throw new ApplicationException("Error: Try to set " + this.TypeAndName + "[" + i + "] with " + val.ToString());
    }

    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant(this[i]);
    }

   
   
    double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
    {
      get
      {
        return i<m_Count ? this[i].Ticks/1E7 : Double.NaN;
      }
    }
    
    double Altaxo.Data.INumericColumn.this[int i]
    {
      get
      {
        return i < m_Count ? this[i].Ticks / 1E7 : Double.NaN;
      }
    }


    public override bool IsElementEmpty(int i)
    {
      return i<m_Count ? (DateTime.MinValue==m_Array[i]) : true;
    }
    public override void SetElementEmpty(int i)
    {
      if (i < m_Count)
        this[i] = NullValue;
    }

    public new DateTime this[int i]
    {
      get
      {
        if(i>=0 && i<m_Count)
          return m_Array[i];
        return DateTime.MinValue; 
      }
      set
      {
        bool bCountDecreased=false;


        if(value==DateTime.MinValue)
        {
          if(i>=0 && i<m_Count-1) // i is inside the used range
          {
            m_Array[i]=value;
          }
          else if(i==(m_Count-1)) // m_Count is then decreasing
          {
            for(m_Count=i; m_Count>0 && (DateTime.MinValue==m_Array[m_Count-1]); --m_Count);
            bCountDecreased=true;;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is a valid value
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
              m_Array[k]=DateTime.MinValue; // fill range between used range and new element with voids
          
            m_Array[i]=value;
            m_Count=i+1;
          }
          else if(i>=0) // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for(int k=m_Count;k<i;k++)
              m_Array[k]=DateTime.MinValue; // fill range between used range and new element with voids
          
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
    public static Altaxo.Data.DateTimeColumn operator +(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.Count<c2.Count ? c1.Count : c2.Count;
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i].AddSeconds(c2.GetValueDirect(i));
      }
      
      
      c3.m_Count=len;
      
      return c3;  
    }

    public static Altaxo.Data.DateTimeColumn operator +(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      int len = c1.m_Count;
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i].AddSeconds(c2);
      }

      
      
      c3.m_Count=len;

      return c3;  
    }


    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DateTimeColumn c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1,c2);
    }


    public static Altaxo.Data.DateTimeColumn operator -(Altaxo.Data.DateTimeColumn c1, Altaxo.Data.DoubleColumn c2)
    {
      int len = c1.Count<c2.Count ? c1.Count : c2.Count;
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(len);
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i].AddSeconds(-c2.GetValueDirect(i));
      }
      
      
      c3.m_Count=len;
      
      return c3;  
    }

  

    public static Altaxo.Data.DateTimeColumn operator -(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(c1.m_Count);
      int len = c1.m_Count;
      for(int i=0;i<len;i++)
      {
        c3.m_Array[i] = c1.m_Array[i].AddSeconds(-c2);
      }

      
      
      c3.m_Count=len;

      return c3;  
    }

    public static Altaxo.Data.DoubleColumn operator -(Altaxo.Data.DateTimeColumn c1, DateTime c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1, c2);
    }

    public static Altaxo.Data.DoubleColumn operator -(DateTime c1, Altaxo.Data.DateTimeColumn c2)
    {
      return Altaxo.Data.DoubleColumn.Subtraction(c1, c2);
    }



    public override bool vop_Subtraction(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DateTimeColumn)
      {
        c3 = this - (Altaxo.Data.DateTimeColumn)c2;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(DataColumn c2, out DataColumn c3)
    {
      if (c2 is Altaxo.Data.DateTimeColumn)
      {
        c3 = (Altaxo.Data.DateTimeColumn)c2 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction(AltaxoVariant c2, out DataColumn c3)
    {
      if (((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDateTime))
      {
        DateTime c22 = (DateTime)c2;
        c3 = this - c22;
        return true;
      }
      c3 = null;
      return false;
    }

    public override bool vop_Subtraction_Rev(AltaxoVariant c2, out DataColumn c3)
    {
      if (((AltaxoVariant)c2).IsType(AltaxoVariant.Content.VDateTime))
      {
        DateTime c22 = (DateTime)c2;
        c3 = c22 - this;
        return true;
      }
      c3 = null;
      return false;
    }

    
    #endregion

  } // end Altaxo.Data.DateTimeColumn
}
