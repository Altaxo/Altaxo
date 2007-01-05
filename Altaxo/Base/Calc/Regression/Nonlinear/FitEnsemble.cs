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
using System.Collections;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds a collection of <see cref="FitElement" />s and is responsible for parameter
  /// bundling.
  /// </summary>
  public class FitEnsemble : System.Collections.CollectionBase, ICloneable
  {
    /// <summary>
    /// Current parameter names
    /// </summary>
    string[] _parameterNames = new string[0];
 
    /// <summary>
    /// All parameters sorted by name.
    /// </summary>
    System.Collections.SortedList _ParametersSortedByName;
    

    public event EventHandler Changed;


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitEnsemble),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        FitEnsemble s = (FitEnsemble)obj;

        info.CreateArray("FitElements",s.Count);
        for(int i=0;i<s.Count;++i)
          info.AddValue("e",s[i]);
        info.CommitArray();
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        FitEnsemble s = o!=null ? (FitEnsemble)o : new FitEnsemble();

        int arraycount = info.OpenArray();
        for(int i=0;i<arraycount;++i)
          s.Add( (FitElement)info.GetValue(s) );
        info.CloseArray(arraycount);

        s.CollectParameterNames();

        return s;
      }
    }

    #endregion



    public FitElement this[int i]
    {
      get 
      {
        return (FitElement)InnerList[i];
      }
      set
      {
        FitElement oldValue = this[i];
        oldValue.Changed -= new EventHandler(EhChildChanged);

        InnerList[i] = value;
        value.Changed += new EventHandler(EhChildChanged);

        OnChanged();
      }
    }

    public void Add(FitElement e)
    {
      InnerList.Add(e);
      e.Changed += new EventHandler(EhChildChanged);

      OnChanged();
    }


    protected virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, EventArgs.Empty);
    }

    protected virtual void EhChildChanged(object obj, EventArgs e)
    {
      CollectParameterNames();
      OnChanged();
    }
    
    #region Fit parameters

    protected void CollectParameterNames()
    {

      _ParametersSortedByName = new System.Collections.SortedList();

      int nameposition = 0;
      for(int i=0;i<InnerList.Count;i++)
      {
        if(null==this[i].FitFunction)
          continue;
        IFitFunction func = this[i].FitFunction;
        FitElement ele = this[i];

        for(int k=0;k<func.NumberOfParameters;k++)
        {
          if(!(_ParametersSortedByName.ContainsKey(ele.ParameterName(k))))
          {
            _ParametersSortedByName.Add(ele.ParameterName(k),nameposition++);
          }
        }
      }

      // now sort the items in the order of the namepositions
      System.Collections.SortedList sortedbypos = new System.Collections.SortedList();
      foreach(DictionaryEntry en in _ParametersSortedByName)
        sortedbypos.Add(en.Value,en.Key);


      _parameterNames = new string[sortedbypos.Count];
      for(int i=0;i<_parameterNames.Length;i++)
        _parameterNames[i] = (string)sortedbypos[i];

    }


    public string ParameterName(int i)
    {
      return _parameterNames[i];
    }

    public int NumberOfParameters
    {
      get
      {
        return _parameterNames.Length;
      }
    }
   
    #endregion

    #region ICloneable Members

    public object Clone()
    {
      FitEnsemble result = new FitEnsemble();
      foreach (FitElement ele in this)
        result.Add((FitElement)ele.Clone());

      return result;
    }

    #endregion
  }
}
