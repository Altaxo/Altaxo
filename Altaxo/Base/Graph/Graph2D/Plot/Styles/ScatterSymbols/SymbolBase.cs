#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
	public abstract class SymbolBase
	{
		/// <summary>
		/// Used to create clipper polynoms for the outer symbol shape, frame and inset. The outer symbol shape
		/// must fit in a circle of radius 1 (diameter: 2). Translated to clipper values this means that the outer symbol shape
		/// must fit in a circle of this <see cref="ClipperScalingDouble"/>.</summary>
		public const double ClipperScalingDouble = 1073741824.49;

		/// <summary>
		/// Used to create clipper polynoms for the outer symbol shape, frame and inset. The outer symbol shape
		/// must fit in a circle of radius 1 (diameter: 2). Translated to clipper values this means that the outer symbol shape
		/// must fit in a circle of this <see cref="ClipperScalingInt"/>.</summary>
		public const int ClipperScalingInt = 1073741824;

		/// <summary>By multiplying the clipper polynom points with this factor, you will get a symbol size of 1.</summary>
		public const double InverseClipperScalingToSymbolSize1 = 0.5 / 1073741824.0;

		#region Serialization

		protected static void SerializeSetV0(IScatterSymbol obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			var parent = ScatterSymbolListManager.Instance.GetParentList(obj);
			if (null != parent)
			{
				if (null == info.GetProperty(ScatterSymbolList.GetSerializationRegistrationKey(parent)))
					info.AddValue("Set", parent);
				else
					info.AddValue("SetName", parent.Name);
			}
		}

		protected static TItem DeserializeSetV0<TItem>(TItem instanceTemplate, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent) where TItem : IScatterSymbol
		{
			if (info.CurrentElementName == "Set")
			{
				var originalSet = (ScatterSymbolList)info.GetValue("Set", parent);
				ScatterSymbolList registeredSet;
				ScatterSymbolListManager.Instance.TryRegisterList(info, originalSet, Main.ItemDefinitionLevel.Project, out registeredSet);
				return (TItem)ScatterSymbolListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, originalSet.Name); // Note: here we use the name of the original set, not of the registered set. Because the original name is translated during registering into the registered name
			}
			else if (info.CurrentElementName == "SetName")
			{
				string setName = info.GetString("SetName");
				return (TItem)ScatterSymbolListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, setName);
			}
			else // nothing of both, thus symbol belongs to nothing
			{
				return instanceTemplate;
			}
		}

		#endregion Serialization
	}
}