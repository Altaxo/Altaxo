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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Calc.Regression.Nonlinear
{
	/// <summary>
	/// Holds a collection of <see cref="FitElement" />s and is responsible for parameter bundling.
	/// </summary>
	/// <remarks>The number of parameters in a FitEnsemble is less than or equal to the sum of the number of parameters of all FitElements bundeled in this instance.
	/// (It is less than the sum of parameters if some parameters of different fit elements have equal names).</remarks>
	public class FitEnsemble
		:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IList<FitElement>,
		ICloneable
	{
		/// <summary>
		/// Current parameter names
		/// </summary>
		private string[] _parameterNames = new string[0];

		/// <summary>
		/// All parameters sorted by name.
		/// </summary>
		private SortedList<string, int> _parametersSortedByName = new SortedList<string, int>();

		private List<FitElement> _innerList = new List<FitElement>();

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitEnsemble), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				FitEnsemble s = (FitEnsemble)obj;

				info.CreateArray("FitElements", s._innerList.Count);
				for (int i = 0; i < s._innerList.Count; ++i)
					info.AddValue("e", s[i]);
				info.CommitArray();
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				FitEnsemble s = o != null ? (FitEnsemble)o : new FitEnsemble();

				int arraycount = info.OpenArray();
				for (int i = 0; i < arraycount; ++i)
					s.Add((FitElement)info.GetValue("e", s));
				info.CloseArray(arraycount);

				s.CollectParameterNames();

				return s;
			}
		}

		#endregion Serialization

		#region Fit parameters

		protected void CollectParameterNames()
		{
			_parametersSortedByName.Clear();

			int nameposition = 0;
			for (int i = 0; i < _innerList.Count; i++)
			{
				FitElement ele = this[i];
				IFitFunction func = ele.FitFunction;

				if (null == func)
					continue;

				for (int k = 0; k < func.NumberOfParameters; k++)
				{
					if (!(_parametersSortedByName.ContainsKey(ele.ParameterName(k))))
					{
						_parametersSortedByName.Add(ele.ParameterName(k), nameposition++);
					}
				}
			}

			// now sort the items in the order of the namepositions
			var sortedbypos = new SortedList<int, string>();
			foreach (KeyValuePair<string, int> en in _parametersSortedByName)
				sortedbypos.Add(en.Value, en.Key);

			_parameterNames = new string[sortedbypos.Count];
			for (int i = 0; i < _parameterNames.Length; i++)
				_parameterNames[i] = sortedbypos[i];
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

		#endregion Fit parameters

		#region ICloneable Members

		public object Clone()
		{
			FitEnsemble result = new FitEnsemble();
			foreach (FitElement ele in this)
				result.Add((FitElement)ele.Clone());

			return result;
		}

		#endregion ICloneable Members

		#region IList members

		public FitElement this[int i]
		{
			get
			{
				return _innerList[i];
			}
			set
			{
				FitElement oldValue = this[i];
				if (object.ReferenceEquals(oldValue, value))
					return;

				_innerList[i] = value;
				value.ParentObject = this;

				if (null != oldValue)
					oldValue.Dispose();

				EhSelfChanged(EventArgs.Empty);
			}
		}

		public void Add(FitElement e)
		{
			_innerList.Add(e);
			e.ParentObject = this;

			CollectParameterNames();
			EhSelfChanged(EventArgs.Empty);
		}

		public void Clear()
		{
			_innerList.Clear();
		}

		public bool Contains(FitElement item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(FitElement[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(FitElement item)
		{
			return _innerList.Remove(item);
		}

		public IEnumerator<FitElement> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		public int IndexOf(FitElement item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, FitElement item)
		{
			_innerList.Insert(index, item);
			item.ParentObject = this;
		}

		public void RemoveAt(int index)
		{
			_innerList.RemoveAt(index);
		}

		#endregion IList members

		#region Changed handling

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			CollectParameterNames();

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		#endregion Changed handling

		#region Document node functions

		protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _innerList)
			{
				for (int i = 0; i < _innerList.Count; ++i)
				{
					if (null != _innerList[i])
						yield return new Main.DocumentNodeAndName(_innerList[i], "FitElement" + i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				}
			}
		}

		#endregion Document node functions
	}
}