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

using Altaxo.Drawing.DashPatternManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.DashPatterns
{
	public abstract class DashPatternBase : IDashPattern
	{
		#region Serialization

		protected static void SerializeV0(IDashPattern obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			var parent = DashPatternListManager.Instance.GetParentList(obj);
			if (null != parent)
			{
				if (null == info.GetProperty(DashPatternList.GetSerializationRegistrationKey(parent)))
					info.AddValue("Set", parent);
				else
					info.AddValue("SetName", parent.Name);
			}
		}

		protected static TItem DeserializeV0<TItem>(TItem instanceTemplate, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent) where TItem : IDashPattern
		{
			if (info.CurrentElementName == "Set")
			{
				var originalSet = (DashPatternList)info.GetValue("Set", parent);
				DashPatternList registeredSet;
				DashPatternListManager.Instance.TryRegisterList(info, originalSet, Main.ItemDefinitionLevel.Project, out registeredSet);
				return (TItem)DashPatternListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, originalSet.Name); // Note: here we use the name of the original set, not of the registered set. Because the original name is translated during registering into the registered name
			}
			else if (info.CurrentElementName == "SetName")
			{
				string setName = info.GetString("SetName");
				return (TItem)DashPatternListManager.Instance.GetDeserializedInstanceFromInstanceAndSetName(info, instanceTemplate, setName);
			}
			else // nothing of both, thus symbol belongs to nothing
			{
				return instanceTemplate;
			}
		}

		#endregion Serialization

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.GetType() == obj?.GetType();
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public abstract double this[int index] { get; set; }

		public abstract int Count { get; }

		public virtual double DashOffset { get { return 0; } }

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public void Add(double item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(double item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(double[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<double> GetEnumerator()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public int IndexOf(double item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, double item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(double item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}
	}
}