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
  public class TDTransformationClass : TDataClass
  {
    private WITecTreeNode _tdTransformation;

    public string StandardUnit { get; }

    public TDInterpretationClass? Interpretation { get; }

    public bool IsCalibrated { get; }

    public int UnitKind { get; }


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
