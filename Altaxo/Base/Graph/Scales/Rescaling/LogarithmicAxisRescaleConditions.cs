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

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Summary description for LogarithmicAxisRescaleConditions.
  /// </summary>
  [Serializable]
  public class LogarithmicAxisRescaleConditions : NumericAxisRescaleConditions
  {

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Axes.Scaling.LogarithmicAxisRescaleConditions", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LogarithmicAxisRescaleConditions),1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LogarithmicAxisRescaleConditions s = (LogarithmicAxisRescaleConditions)obj;

        info.AddBaseValueEmbedded(s,s.GetType().BaseType);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        LogarithmicAxisRescaleConditions s = null!=o ? (LogarithmicAxisRescaleConditions)o : new LogarithmicAxisRescaleConditions();

        info.GetBaseValueEmbedded(s,s.GetType().BaseType,parent);

        return s;
      }
    }
   
    #endregion

    /// <summary>
    /// This will process the temporary values for the axis origin and axis end. Depending on the rescaling conditions,
    /// the values of org and end are changed.
    /// </summary>
    /// <param name="org">The temporary axis origin (usually the lower boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoOrg">On return, this value is true if the org value was not modified.</param>
    /// <param name="end">The temporary axis end (usually the upper boundary of the data set. On return, this value may be modified, depending on the rescale conditions.</param>
    /// <param name="isAutoEnd">On return, this value is true if the end value was not modified.</param>
    public override void Process(ref double org, out bool isAutoOrg, ref double end, out bool isAutoEnd)
    {
      double oorg = org;
      double oend = end;
      isAutoOrg = true;
      isAutoEnd = true;

      if(_spanRescaling!=BoundaryRescaling.Auto)
      {
        switch(_spanRescaling)
        {
          case BoundaryRescaling.Fixed:
            org = Math.Exp((Math.Log(oorg)+Math.Log(oend)-Math.Log(_span))*0.5);
            end = Math.Exp((Math.Log(oorg)+Math.Log(oend)+Math.Log(_span))*0.5);
            isAutoOrg = false;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(Math.Abs(oorg-oend)<_span)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(Math.Abs(oorg-oend)>_span)
              goto case BoundaryRescaling.Fixed;
            break;
        } // switch
      }
      else // spanRescaling is Auto
      {
        switch(_orgRescaling)
        {
          case BoundaryRescaling.Fixed:
            org = _org;
            isAutoOrg = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oorg<_org)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oorg>_org)
              goto case BoundaryRescaling.Fixed;
            break;
        }
        switch(_endRescaling)
        {
          case BoundaryRescaling.Fixed:
            end = _end;
            isAutoEnd = false;
            break;
          case BoundaryRescaling.GreaterOrEqual:
            if(oend<_end)
              goto case BoundaryRescaling.Fixed;
            break;
          case BoundaryRescaling.LessOrEqual:
            if(oend>_end)
              goto case BoundaryRescaling.Fixed;
            break;
        }
      }
    }

    #region ICloneable Members

    public override  object Clone()
    {
      LogarithmicAxisRescaleConditions result = new LogarithmicAxisRescaleConditions();
      result.CopyFrom(this);
      return result;
    }

    #endregion
  }
}
