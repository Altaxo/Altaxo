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

using System;

namespace Altaxo.Serialization.WITec
{
  public class TDSpectralInterpretationClass : TDInterpretationClass
  {
    protected WITecTreeNode _tdSpectralInterpretation;

    public double ExcitationWaveLength { get; }

    public TDSpectralInterpretationClass(WITecTreeNode node)
      : base(node)
    {
      _tdSpectralInterpretation = node.GetChild("TDSpectralInterpretation");
      ExcitationWaveLength = _tdSpectralInterpretation.GetData<double>("ExcitationWaveLength");
    }


    public override (string Description, string shortCut) GetUnit(int unit)
    {
      return unit switch
      {
        0 => ("Nanometers", "nm"),
        1 => ("Micrometers", "µm"),
        2 => ("Spectroscopic wavenumber", "1/cm"),
        3 => ("Raman Shift", "rel. 1/cm"),
        4 => ("Energy", "eV"),
        5 => ("Energy", "meV"),
        6 => ("Relative Energy", "rel. eV"),
        7 => ("Relative Energy", "rel. meV"),
        > 7 => ("", ""),
        _ => throw new NotImplementedException(),
      };
    }
  }
}

