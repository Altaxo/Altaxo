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
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Scales
{
  [Serializable]
  public class LinkedScaleCollection 
    :
    Main.IChangedEventSource,
    Main.IDocumentNode
  {
    LinkedScale[] _linkedScales = new LinkedScale[2];

    /// <summary>
    /// Fired if one of the scale has changed (or its boundaries).
    /// </summary>
    [field: NonSerialized]
    public event EventHandler ScalesChanged;

    /// <summary>
    /// Fired if something in this class or in its child has changed.
    /// </summary>
    [field: NonSerialized]
    event EventHandler _changed;

    [NonSerialized]
    object _parent;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayerAxisPropertiesCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinkedScaleCollection), 1)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        LinkedScaleCollection s = (LinkedScaleCollection)obj;

        info.CreateArray("Properties", s._linkedScales.Length);
        for (int i = 0; i < s._linkedScales.Length; ++i)
          info.AddValue("e", s._linkedScales[i]);
        info.CommitArray();
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinkedScaleCollection s = SDeserialize(o, info, parent);
        return s;
      }


      protected virtual LinkedScaleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinkedScaleCollection s = null != o ? (LinkedScaleCollection)o : new LinkedScaleCollection();

        int count = info.OpenArray("Properties");
        s._linkedScales = new LinkedScale[count];
        for (int i = 0; i < count; ++i)
          s.SetLinkedScale((LinkedScale)info.GetValue("e", s), i);
        info.CloseArray(count);

        return s;
      }
    }
    #endregion

    public LinkedScaleCollection()
    {
      _linkedScales = new LinkedScale[2];
      this.SetLinkedScale(new LinkedScale(), 0);
      this.SetLinkedScale(new LinkedScale(), 1);
    }

    public LinkedScaleCollection(LinkedScaleCollection from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(LinkedScaleCollection from)
    {
      if (_linkedScales != null)
      {
        for (int i = 0; i < _linkedScales.Length; ++i)
        {
          if (_linkedScales[i] != null)
            _linkedScales[i].LinkPropertiesChanged -= new EventHandler(EhLinkPropertiesChanged);
          _linkedScales[i] = null;
        }
      }

      _linkedScales = new LinkedScale[from._linkedScales.Length];
      for (int i = 0; i < from._linkedScales.Length; i++)
      {
        _linkedScales[i] = from._linkedScales[i].Clone();
        _linkedScales[i].LinkPropertiesChanged += new EventHandler(EhLinkPropertiesChanged);
      }

      OnChanged();
    }

    public LinkedScaleCollection Clone()
    {
      return new LinkedScaleCollection(this);
    }

    public LinkedScale X
    {
      get
      {
        return _linkedScales[0];
      }
    }

    public LinkedScale Y
    {
      get
      {
        return _linkedScales[1];
      }
    }

    public Scale Scale(int i)
    {
      return _linkedScales[i].Scale;
    }
    public void SetScale(int i, Scale ax)
    {
      _linkedScales[i].Scale = ax;
    }
    public int IndexOf(Scale ax)
    {
      for (int i = 0; i < _linkedScales.Length; i++)
      {
        if (_linkedScales[i].Scale == ax)
          return i;
      }

      return -1;
    }

    protected void SetLinkedScale(LinkedScale newvalue, int i)
    {
      LinkedScale oldvalue = _linkedScales[i];
      _linkedScales[i] = newvalue;

      if (!object.ReferenceEquals(oldvalue, newvalue))
      {
        if (null != oldvalue)
          oldvalue.LinkPropertiesChanged -= new EventHandler(EhLinkPropertiesChanged);
        if (null != newvalue)
          newvalue.LinkPropertiesChanged += new EventHandler(EhLinkPropertiesChanged);
      }
    }

    private void EhLinkPropertiesChanged(object sender, EventArgs e)
    {
      if (ScalesChanged != null)
        ScalesChanged(this, EventArgs.Empty);

      OnChanged();
    }


    public event EventHandler Changed
    {
      add { _changed += value; }
      remove { _changed -= value; }
    }


    protected virtual void OnChanged()
    {
      if (_parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

      if (null != _changed)
        _changed(this, EventArgs.Empty);
    }


    #region IDocumentNode Members

    public object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public string Name
    {
      get { return "LinkedScaleCollection"; }
    }

    #endregion

   

  
   
  }





}
