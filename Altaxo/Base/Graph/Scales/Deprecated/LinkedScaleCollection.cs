﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Scales.Deprecated
{
  [Serializable]
  public class LinkedScaleCollection
    :
    Main.SuspendableDocumentNodeWithSetOfEventArgs
  {
    private LinkedScale[] _linkedScales = new LinkedScale[2];

    /// <summary>
    /// Fired if one of the scale has changed (or its boundaries).
    /// </summary>
    [field: NonSerialized]
    public event EventHandler? ScalesChanged;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisPropertiesCollection", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.LinkedScaleCollection", 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinkedScaleCollection)obj;

        info.CreateArray("Properties", s._linkedScales.Length);
        for (int i = 0; i < s._linkedScales.Length; ++i)
          info.AddValue("e", s._linkedScales[i]);
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinkedScaleCollection?)o ?? new LinkedScaleCollection();

        int count = info.OpenArray("Properties");
        s._linkedScales = new LinkedScale[count];
        for (int i = 0; i < count; ++i)
          s.SetLinkedScale((LinkedScale)info.GetValue("e", s), i);
        info.CloseArray(count);

        return s;
      }


    }

    #endregion Serialization

    public LinkedScaleCollection()
    {
      _linkedScales = new LinkedScale[2];
      SetLinkedScale(new LinkedScale(), 0);
      SetLinkedScale(new LinkedScale(), 1);
    }

    public LinkedScaleCollection(LinkedScaleCollection from)
    {
      CopyFrom(from);
    }

    public void CopyFrom(LinkedScaleCollection from)
    {
      if (ReferenceEquals(this, from))
        return;

      for (int i = 0; i < _linkedScales.Length; ++i)
      {
        if (_linkedScales[i] is { } ls)
          ls.LinkPropertiesChanged -= new EventHandler(EhLinkPropertiesChanged);
      }

      _linkedScales = new LinkedScale[from._linkedScales.Length];
      for (int i = 0; i < from._linkedScales.Length; i++)
      {
        _linkedScales[i] = from._linkedScales[i].Clone();
        _linkedScales[i].LinkPropertiesChanged += new EventHandler(EhLinkPropertiesChanged);
      }

      EhSelfChanged(EventArgs.Empty);
    }

    public LinkedScaleCollection Clone()
    {
      return new LinkedScaleCollection(this);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_linkedScales is not null)
      {
        for (int i = 0; i < _linkedScales.Length; ++i)
        {
          if (_linkedScales[i] is { } ls)
            yield return new Main.DocumentNodeAndName(ls, "LinkedScale" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
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
      var oldvalue = _linkedScales[i];
      _linkedScales[i] = newvalue;

      if (!object.ReferenceEquals(oldvalue, newvalue))
      {
        if (oldvalue is not null)
          oldvalue.LinkPropertiesChanged -= new EventHandler(EhLinkPropertiesChanged);
        if (newvalue is not null)
          newvalue.LinkPropertiesChanged += new EventHandler(EhLinkPropertiesChanged);
      }
    }

    private void EhLinkPropertiesChanged(object? sender, EventArgs e)
    {
      ScalesChanged?.Invoke(this, EventArgs.Empty);

      EhSelfChanged(EventArgs.Empty);
    }
  }
}
