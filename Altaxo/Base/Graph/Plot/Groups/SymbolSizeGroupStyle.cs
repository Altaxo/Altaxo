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
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// This group style is intended to publish the symbol size to all interested
  /// plot styles.
  /// </summary>
  public class SymbolSizeGroupStyle
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IPlotGroupStyle
  {
    private bool _isInitialized;
    private double _symbolSize;
    private static readonly Type MyType = typeof(SymbolSizeGroupStyle);

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SymbolSizeGroupStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SymbolSizeGroupStyle)obj;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SymbolSizeGroupStyle?)o ?? new SymbolSizeGroupStyle();
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolSizeGroupStyle"/> class.
    /// </summary>
    public SymbolSizeGroupStyle()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolSizeGroupStyle"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public SymbolSizeGroupStyle(SymbolSizeGroupStyle from)
    {
      _isInitialized = from._isInitialized;
      _symbolSize = from._symbolSize;
    }

    #endregion Constructors

    #region ICloneable Members

    /// <summary>
    /// Creates a copy of this style.
    /// </summary>
    /// <returns>A copied style instance.</returns>
    public SymbolSizeGroupStyle Clone()
    {
      return new SymbolSizeGroupStyle(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new SymbolSizeGroupStyle(this);
    }

    #endregion ICloneable Members

    #region IGroupStyle Members

    /// <inheritdoc/>
    public void TransferFrom(IPlotGroupStyle fromb)
    {
      var from = (SymbolSizeGroupStyle)fromb;
      _isInitialized = from._isInitialized;
      _symbolSize = from._symbolSize;
    }

    /// <inheritdoc/>
    public void BeginPrepare()
    {
      _isInitialized = false;
    }

    /// <inheritdoc/>
    public void PrepareStep()
    {
    }

    /// <inheritdoc/>
    public void EndPrepare()
    {
    }

    /// <inheritdoc/>
    public bool CanCarryOver
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public bool CanStep
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public int Step(int step)
    {
      return 0;
    }

    /// <summary>
    /// Get/sets whether or not stepping is allowed.
    /// </summary>
    public bool IsStepEnabled
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    #endregion IGroupStyle Members

    #region Other members

    /// <summary>
    /// Gets a value indicating whether this style was initialized.
    /// </summary>
    public bool IsInitialized
    {
      get
      {
        return _isInitialized;
      }
    }

    /// <summary>
    /// Initializes the symbol size.
    /// </summary>
    /// <param name="symbolSize">The symbol size.</param>
    public void Initialize(double symbolSize)
    {
      _isInitialized = true;
      _symbolSize = symbolSize;
    }

    /// <summary>
    /// Gets the symbol size.
    /// </summary>
    public double SymbolSize
    {
      get
      {
        return _symbolSize;
      }
    }

    #endregion Other members

    #region Static helpers

    /// <summary>
    /// Adds the symbol-size group style to the external collection when required.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
    {
      if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(SymbolSizeGroupStyle)))
      {
        var gstyle = new SymbolSizeGroupStyle
        {
          IsStepEnabled = true
        };
        externalGroups.Add(gstyle);
      }
    }

    /// <summary>
    /// Adds the symbol-size group style to the local collection when required.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    public static void AddLocalGroupStyle(
     IPlotGroupStyleCollection externalGroups,
     IPlotGroupStyleCollection localGroups)
    {
      if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(SymbolSizeGroupStyle)))
        localGroups.Add(new SymbolSizeGroupStyle());
    }

    /// <summary>
    /// Represents a delegate that returns a symbol size.
    /// </summary>
    /// <returns>The symbol size returned by the delegate.</returns>
    public delegate double SymbolSizeGetter();

    /// <summary>
    /// Prepares a symbol-size group style for later application.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="getter">The delegate that supplies the symbol size.</param>
    public static void PrepareStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      SymbolSizeGetter getter)
    {
      if (!externalGroups.ContainsType(typeof(SymbolSizeGroupStyle))
        && localGroups is not null
        && !localGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
      {
        localGroups.Add(new SymbolSizeGroupStyle());
      }

      SymbolSizeGroupStyle? grpStyle = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpStyle = (SymbolSizeGroupStyle)externalGroups.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));
      else if (localGroups is not null)
        grpStyle = (SymbolSizeGroupStyle)localGroups.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));

      if (grpStyle is not null && getter is not null && !grpStyle.IsInitialized)
        grpStyle.Initialize(getter());
    }

    /// <summary>
    /// Represents a delegate that stores a symbol size.
    /// </summary>
    /// <param name="c">The symbol size to store.</param>
    public delegate void SymbolSizeSetter(double c);

    /// <summary>
    /// Tries to apply the symbol-size group style.
    /// </summary>
    /// <param name="externalGroups">The external group-style collection.</param>
    /// <param name="localGroups">The local group-style collection.</param>
    /// <param name="setter">The receiver of the symbol size.</param>
    /// <returns><c>true</c> if successfully applied; otherwise, <c>false</c>.</returns>
    public static bool ApplyStyle(
      IPlotGroupStyleCollection externalGroups,
      IPlotGroupStyleCollection localGroups,
      SymbolSizeSetter setter)
    {
      IPlotGroupStyleCollection? grpColl = null;
      if (externalGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpColl = externalGroups;
      else if (localGroups is not null && localGroups.ContainsType(typeof(SymbolSizeGroupStyle)))
        grpColl = localGroups;

      if (grpColl is not null)
      {
        var grpStyle = (SymbolSizeGroupStyle)grpColl.GetPlotGroupStyle(typeof(SymbolSizeGroupStyle));
        grpColl.OnBeforeApplication(typeof(SymbolSizeGroupStyle));
        setter(grpStyle.SymbolSize);
        return true;
      }
      else
      {
        return false;
      }
    }

    #endregion Static helpers
  }
}
