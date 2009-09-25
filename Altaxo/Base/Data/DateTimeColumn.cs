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
    private DateTime[] _data;
    private int        _capacity; // shortcut to m_Array.Length;
    private int        _count;
    public static readonly DateTime NullValue = DateTime.MinValue;
  
    public DateTimeColumn()
    {
    }

  
    public DateTimeColumn(int initialcapacity)
    {
      _count = 0;
      _data = new DateTime[initialcapacity];
      _capacity = initialcapacity;
    }
        
    public DateTimeColumn(DateTimeColumn from)
    {
      this._count    = from._count;
      this._capacity = from._capacity;
      this._data    = null==from._data ? null : (DateTime[])from._data.Clone();
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

        if(s._count!=s._capacity)
        {
          // instead of the data array itself, stream only the first m_Count
          // array elements, since only they contain data
          DateTime[] streamarray = new DateTime[s._count];
          System.Array.Copy(s._data,streamarray,s._count);
          info.AddValue("Data",streamarray);
        }
        else // if the array is fully filled, we don't need to save a shrinked copy
        {
          info.AddValue("Data",s._data);
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

        s._data = (DateTime[])(info.GetValue("Data",typeof(DateTime[])));
        s._capacity = null==s._data ? 0 : s._data.Length;
        s._count = s._capacity;
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
          info.AddArray("Data",s._data,s._count);
        else
          info.AddArray("Data",s._data,0);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.DateTimeColumn s = null!=o ? (Altaxo.Data.DateTimeColumn)o : new Altaxo.Data.DateTimeColumn();
        
        

        // deserialize the base class
        info.GetBaseValueEmbedded(s,typeof(Altaxo.Data.DataColumn),parent);

        int count = info.GetInt32Attribute("Count");
        s._data = new DateTime[count];
        info.GetArray(s._data,count);
        s._capacity = null==s._data ? 0 : s._data.Length;
        s._count = s._capacity;
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
        return _count;
      }
    }


    public DateTime[] Array
    {
      get 
      {
        int len = this.Count;
        DateTime[] arr = new DateTime[len];
        System.Array.Copy(_data,0,arr,0,len);
        return arr;
      }

      set
      {
        _data = (DateTime[])value.Clone();
        this._count = _data.Length;
        this._capacity = _data.Length;
        this.NotifyDataChanged(0,_count,true);
      }
    }

    protected internal DateTime GetValueDirect(int idx)
    {
      return _data[idx];
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
      int oldCount = this._count;
      if(null==vd._data || vd._count==0)
      {
        _data=null;
        _capacity=0;
        _count=0;
      }
      else
      {
        _data = (DateTime[])vd._data.Clone();
        _capacity = _data.Length;
        _count = ((Altaxo.Data.DateTimeColumn)v)._count;
      }
      if(oldCount>0 || _count>0) // message only if really was a change
        NotifyDataChanged(0, oldCount>_count? (oldCount):(_count),_count<oldCount);
    }       

    protected void Realloc(int i)
    {
      int newcapacity1 = (int)(_capacity*_increaseFactor+_addSpace);
      int newcapacity2 = i+_addSpace+1;
      int newcapacity = newcapacity1>newcapacity2 ? newcapacity1:newcapacity2;
        
      DateTime[] newarray = new DateTime[newcapacity];
      if(_count>0)
      {
        System.Array.Copy(_data,newarray,_count);
      }

      _data = newarray;
      _capacity = _data.Length;
    }

    // indexers
    public override void SetValueAt(int i, AltaxoVariant val)
    {
      try
      {
        this[i] = val.ToDateTime();
      }
      catch (Exception ex)
      {
        throw new ApplicationException(string.Format("Error: Try to set {0}[{1}] with the string {2}, exception: {3}", this.TypeAndName, i, val.ToString(), ex.Message));
      }
    }

    public override AltaxoVariant GetVariantAt(int i)
    {
      return new AltaxoVariant(this[i]);
    }

   
   
    double Altaxo.Calc.LinearAlgebra.INumericSequence.this[int i]
    {
      get
      {
        return i<_count ? this[i].Ticks/1E7 : Double.NaN;
      }
    }
    
    double Altaxo.Data.INumericColumn.this[int i]
    {
      get
      {
        return i < _count ? this[i].Ticks / 1E7 : Double.NaN;
      }
    }


    public override bool IsElementEmpty(int i)
    {
      return i<_count ? (DateTime.MinValue==_data[i]) : true;
    }
    public override void SetElementEmpty(int i)
    {
      if (i < _count)
        this[i] = NullValue;
    }

    public new DateTime this[int i]
    {
      get
      {
        if(i>=0 && i<_count)
          return _data[i];
        return DateTime.MinValue; 
      }
      set
      {
        bool bCountDecreased=false;


        if(value==DateTime.MinValue)
        {
          if(i>=0 && i<_count-1) // i is inside the used range
          {
            _data[i]=value;
          }
          else if(i==(_count-1)) // m_Count is then decreasing
          {
            for(_count=i; _count>0 && (DateTime.MinValue==_data[_count-1]); --_count);
            bCountDecreased=true;;
          }
          else // i is above the used area
          {
            return; // no need for a change notification here
          }
        }
        else // value is a valid value
        {
          if(i>=0 && i<_count) // i is inside the used range
          {
            _data[i]=value;
          }
          else if(i==_count && i<_capacity) // i is the next value after the used range
          {
            _data[i]=value;
            _count=i+1;
          }
          else if(i>_count && i<_capacity) // is is outside used range, but inside capacity of array
          {
            for(int k=_count;k<i;k++)
              _data[k]=DateTime.MinValue; // fill range between used range and new element with voids
          
            _data[i]=value;
            _count=i+1;
          }
          else if(i>=0) // i is outside of capacity, then realloc the array
          {
            Realloc(i);

            for(int k=_count;k<i;k++)
              _data[k]=DateTime.MinValue; // fill range between used range and new element with voids
          
            _data[i]=value;
            _count=i+1;
          }
        }
        NotifyDataChanged(i,i+1,bCountDecreased);
      } // end set  
    } // end indexer


    public override void InsertRows(int nInsBeforeColumn, int nInsCount)
    {
      if(nInsCount<=0 || nInsBeforeColumn>=Count)
        return; // nothing to do

      int newlen = this._count + nInsCount;
      if(newlen>_capacity)
        Realloc(newlen);

      // copy values from m_Count downto nBeforeColumn 
      for(int i=_count-1, j=newlen-1; i>=nInsBeforeColumn;i--,j--)
        _data[j] = _data[i];

      for(int i=nInsBeforeColumn+nInsCount-1;i>=nInsBeforeColumn;i--)
        _data[i]=NullValue;
    
      this._count=newlen;
      this.NotifyDataChanged(nInsBeforeColumn,_count,false);
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
      for(i=nDelFirstRow,j=nDelFirstRow+nDelCount;j<_count;i++,j++)
        _data[i]=_data[j];
      
      int prevCount = _count;
      _count= i<_count ? i : _count; // m_Count can only decrease

      if(_count!=prevCount) // raise a event only if something really changed
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
        c3._data[i] = c1._data[i].AddSeconds(c2.GetValueDirect(i));
      }
      
      
      c3._count=len;
      
      return c3;  
    }

    public static Altaxo.Data.DateTimeColumn operator +(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      int len = c1._count;
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(len);
      for(int i=0;i<len;i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(c2);
      }

      
      
      c3._count=len;

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
        c3._data[i] = c1._data[i].AddSeconds(-c2.GetValueDirect(i));
      }
      
      
      c3._count=len;
      
      return c3;  
    }

  

    public static Altaxo.Data.DateTimeColumn operator -(Altaxo.Data.DateTimeColumn c1, double c2)
    {
      Altaxo.Data.DateTimeColumn c3 = new Altaxo.Data.DateTimeColumn(c1._count);
      int len = c1._count;
      for(int i=0;i<len;i++)
      {
        c3._data[i] = c1._data[i].AddSeconds(-c2);
      }

      
      
      c3._count=len;

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
