#region Copyright

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

namespace Altaxo.Graph.Scales
{
  /// <summary>
  /// Represents a collection of <see cref="Scale"/> instances.
  /// </summary>
  [Serializable]
  public class ScaleCollection
  :
  Main.SuspendableDocumentNodeWithSetOfEventArgs,
  Main.ICopyFrom,
  IEnumerable<Scale>
  {
    private Scale[] _scales;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Scales.ScaleCollection", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                ScaleCollection s = (ScaleCollection)obj;

                info.CreateArray("Members", s._scales.Length);
                for (int i = 0; i < s._scales.Length; ++i)
                    info.AddValue("e", s._scales[i]);
                info.CommitArray();
                */
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ScaleCollection?)o ?? new ScaleCollection(info);

        int count = info.OpenArray("Members");
        s._scales = new Scale[count];
        for (int i = 0; i < count; ++i)
        {
          info.OpenElement(); // e
          var scale = (Scale)info.GetValue("Scale", s);
          var tickspacing = (Ticks.TickSpacing)info.GetValue("TickSpacing", s);
          scale.TickSpacing = tickspacing;
          scale.ParentObject = s;
          s._scales[i] = scale;
          info.CloseElement();
        }
        info.CloseArray(count);

        return s;
      }


    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleCollection), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ScaleCollection)obj;

        info.CreateArray("Members", s._scales.Length);
        for (int i = 0; i < s._scales.Length; ++i)
          info.AddValue("e", s._scales[i]);
        info.CommitArray();
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ScaleCollection?)o ?? new ScaleCollection(info);

        int count = info.OpenArray("Members");
        s._scales = new Scale[count];
        for (int i = 0; i < count; ++i)
        {
          s._scales[i] = (Scale)info.GetValue("e", s);
          s._scales[i].ParentObject = s;
        }
        info.CloseArray(count);

        return s;
      }


    }

    #endregion Serialization

    /// <summary>
    /// For deserialization only: Initializes a new instance of the <see cref="ScaleCollection"/> class.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected ScaleCollection(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleCollection"/> class with two scales.
    /// </summary>
    public ScaleCollection()
      : this(2)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleCollection"/> class.
    /// </summary>
    /// <param name="numberOfScales">The number of scales to create.</param>
    public ScaleCollection(int numberOfScales)
    {
      if (numberOfScales <= 0)
        throw new ArgumentOutOfRangeException(nameof(numberOfScales) + " must be >= 1");

      _scales = new Scale[numberOfScales];
      for (int i = 0; i < numberOfScales; ++i)
        this[i] = new LinearScale();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScaleCollection"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public ScaleCollection(ScaleCollection from)
    {
      _scales = new Scale[from._scales.Length];
      CopyFrom(from);
    }

    /// <inheritdoc />
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as ScaleCollection;

      if (from is null)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        int len = Math.Min(_scales.Length, from._scales.Length);
        for (int i = 0; i < len; i++)
        {
          this[i] = (Scale)from._scales[i].Clone();
        }

        suspendToken.Resume();
      }

      return true;
    }

    /// <inheritdoc />
    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_scales is not null)
      {
        for (int i = _scales.Length - 1; i >= 0; --i)
        {
          if (_scales[i] is not null)
            yield return new Main.DocumentNodeAndName(_scales[i], "Scale" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
      }
    }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
      if (isDisposing)
      {
        var scales = _scales;
        _scales = null!;
        if (scales is not null)
        {
          for (int i = 0; i < scales.Length; ++i)
          {
            if (scales[i] is not null)
              scales[i].Dispose();
          }
        }
      }

      base.Dispose(isDisposing);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return new ScaleCollection(this);
    }

    /// <summary>
    /// Creates a strongly typed clone of this collection.
    /// </summary>
    /// <returns>A cloned <see cref="ScaleCollection"/> instance.</returns>
    public ScaleCollection Clone()
    {
      return new ScaleCollection(this);
    }

    /// <summary>
    /// Gets the number of scales in the collection.
    /// </summary>
    public int Count
    {
      get
      {
        return _scales.Length;
      }
    }

    /// <summary>
    /// Gets the conventional name of the scale at the specified index.
    /// </summary>
    /// <param name="idx">The scale index.</param>
    /// <returns>The conventional scale name.</returns>
    public string GetName(int idx)
    {
      switch (idx)
      {
        case 0:
          return "X";

        case 1:
          return "Y";

        case 2:
          return "Z";

        default:
          return "Scale" + idx.ToString();
      }
    }

    /// <summary>
    /// Gets or sets the X scale.
    /// </summary>
    public Scale X
    {
      get
      {
        return this[0];
      }
      set
      {
        this[0] = value;
      }
    }

    /// <summary>
    /// Gets or sets the Y scale.
    /// </summary>
    public Scale Y
    {
      get
      {
        return this[1];
      }
      set
      {
        this[1] = value;
      }
    }

    /// <summary>
    /// Gets or sets the Z scale.
    /// </summary>
    public Scale Z
    {
      get
      {
        return this[2];
      }
      set
      {
        this[2] = value;
      }
    }

    /// <summary>
    /// Gets or sets the scale at the specified index.
    /// </summary>
    /// <param name="i">The scale index.</param>
    /// <returns>The scale at the specified index.</returns>
    public Scale this[int i]
    {
      get
      {
        return _scales[i];
      }
      set
      {
        var oldScale = _scales[i];
        if (ChildSetMember(ref _scales[i], value))
          EhSelfChanged(new Altaxo.Graph.ScaleInstanceChangedEventArgs(oldScale, _scales[i]));
      }
    }

    /// <summary>
    /// Gets the index of the specified scale.
    /// </summary>
    /// <param name="ax">The scale.</param>
    /// <returns>The zero-based index, or <c>-1</c> if the scale was not found.</returns>
    public int IndexOf(Scale ax)
    {
      for (int i = 0; i < _scales.Length; i++)
      {
        if (object.ReferenceEquals(_scales[i], ax))
          return i;
      }

      return -1;
    }

    /// <summary>
    /// Returns an enumerator for the contained scales.
    /// </summary>
    /// <returns>An enumerator for the contained scales.</returns>
    public IEnumerator<Scale> GetEnumerator()
    {
      for (int i = 0; i < _scales.Length; ++i)
        yield return _scales[i];
    }

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < _scales.Length; ++i)
        yield return _scales[i];
    }
  }
}
