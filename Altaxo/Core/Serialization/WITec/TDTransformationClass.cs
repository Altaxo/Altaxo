#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Represents a transformation definition (TDTransformation) read from a WITec project node.
  /// Provides access to the standard unit, optional interpretation, calibration state and unit kind.
  /// </summary>
  public class TDTransformationClass : TDataClass
  {
    /// <summary>
    /// Backing node for the "TDTransformation" child node.
    /// </summary>
    private WITecTreeNode _tdTransformation;

    /// <summary>
    /// Gets the standard unit string as defined in the transformation node.
    /// </summary>
    public string StandardUnit { get; }

    /// <summary>
    /// Gets the optional interpretation associated with this transformation, or <c>null</c> if none is defined.
    /// </summary>
    public TDInterpretationClass? Interpretation { get; }

    /// <summary>
    /// Gets a value indicating whether the transformation is calibrated.
    /// </summary>
    public bool IsCalibrated { get; }

    /// <summary>
    /// Gets the integer code describing the kind of units used by the transformation.
    /// </summary>
    public int UnitKind { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="TDTransformationClass"/> class.
    /// The constructor reads transformation metadata from the provided node and resolves any referenced interpretation using the reader.
    /// </summary>
    /// <param name="node">The node representing the transformation in the WITec tree.</param>
    /// <param name="reader">The reader used to resolve referenced nodes by identifier.</param>
    public TDTransformationClass(WITecTreeNode node, WITecReader reader)
      : base(node)
    {
      _tdTransformation = node.GetChild("TDTransformation");
      StandardUnit = _tdTransformation.GetData<string>("StandardUnit");
      var interpretationID = _tdTransformation.GetData<int>("InterpretationID");
      if (interpretationID != 0)
      {
        Interpretation = reader.BuildNodeWithID<TDInterpretationClass>(interpretationID);
      }
      IsCalibrated = _tdTransformation.GetData<bool>("IsCalibrated");
      UnitKind = _tdTransformation.GetData<int>("UnitKind");
    }
  }
}
