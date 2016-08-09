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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
	using Drawing.D3D;
	using Graph.Plot.Groups;

	public class LineStyleGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private IDashPattern _value;
		private bool _isStepEnabled = true;
		private IStyleList<IDashPattern> _listOfValues;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineStyleGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LineStyleGroupStyle s = (LineStyleGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);

				if (s._isStepEnabled)
					info.AddValue("ListOfValues", s._listOfValues);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LineStyleGroupStyle s = null != o ? (LineStyleGroupStyle)o : new LineStyleGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");

				if (s._isStepEnabled)
					s._listOfValues = (IStyleList<IDashPattern>)info.GetValue("ListOfValues", s);

				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public LineStyleGroupStyle()
		{
			_value = DashPatternListManager.Instance.BuiltinDefault[0];
		}

		public LineStyleGroupStyle(LineStyleGroupStyle from)
		{
			this._isInitialized = from._isInitialized;
			this._value = from._value;
			this._listOfValues = from._listOfValues;
		}

		#endregion Constructors

		#region ICloneable Members

		public LineStyleGroupStyle Clone()
		{
			return new LineStyleGroupStyle(this);
		}

		object ICloneable.Clone()
		{
			return new LineStyleGroupStyle(this);
		}

		#endregion ICloneable Members

		#region IGroupStyle Members

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			LineStyleGroupStyle from = (LineStyleGroupStyle)fromb;
			this._isInitialized = from._isInitialized;
			this._value = from._value;
			this._listOfValues = from._listOfValues;
		}

		public void BeginPrepare()
		{
			_isInitialized = false;
		}

		public void PrepareStep()
		{
		}

		public void EndPrepare()
		{
		}

		public bool CanCarryOver
		{
			get
			{
				return true;
			}
		}

		public bool CanStep
		{
			get
			{
				return true;
			}
		}

		public int Step(int step)
		{
			if (0 == step)
				return 0; // nothing changed

			if (null == _listOfValues)
				_listOfValues = DashPatternListManager.Instance.BuiltinDefault;

			var list = _listOfValues;
			var listcount = list.Count;

			if (listcount == 0)
			{
				return 0;
			}

			int current = _listOfValues.IndexOf(_value);
			if (!(current >= 0))
				current = 0;

			var valueIndex = Calc.BasicFunctions.PMod(current + step, _listOfValues.Count);
			int wraps = Calc.BasicFunctions.NumberOfWraps(_listOfValues.Count, current, step);
			_value = _listOfValues[valueIndex];
			return wraps;
		}

		/// <summary>
		/// Get/sets whether or not stepping is allowed.
		/// </summary>
		public bool IsStepEnabled
		{
			get
			{
				return _isStepEnabled;
			}
			set
			{
				_isStepEnabled = value;
			}
		}

		/// <summary>
		/// The list of values to switch through
		/// </summary>
		public IStyleList<IDashPattern> ListOfValues
		{
			get
			{
				return _listOfValues;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(_listOfValues, value))
				{
					_listOfValues = value;
					EhSelfChanged();
				}
			}
		}

		#endregion IGroupStyle Members

		#region Other members

		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		public void Initialize(IDashPattern c)
		{
			if (null == c)
				throw new ArgumentNullException(nameof(c));

			_isInitialized = true;
			_value = c;
		}

		public IDashPattern DashStyle
		{
			get
			{
				return _value;
			}
		}

		#endregion Other members

		#region Static helpers

		public static void AddLocalGroupStyle(
	 IPlotGroupStyleCollection externalGroups,
	 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(LineStyleGroupStyle)))
				localGroups.Add(new LineStyleGroupStyle());
		}

		public delegate IDashPattern Getter();

		public static void PrepareStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Getter getter)
		{
			if (!externalGroups.ContainsType(typeof(LineStyleGroupStyle))
				&& null != localGroups
				&& !localGroups.ContainsType(typeof(LineStyleGroupStyle)))
			{
				localGroups.Add(new LineStyleGroupStyle());
			}

			LineStyleGroupStyle grpStyle = null;
			if (externalGroups.ContainsType(typeof(LineStyleGroupStyle)))
				grpStyle = (LineStyleGroupStyle)externalGroups.GetPlotGroupStyle(typeof(LineStyleGroupStyle));
			else if (localGroups != null)
				grpStyle = (LineStyleGroupStyle)localGroups.GetPlotGroupStyle(typeof(LineStyleGroupStyle));

			if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
				grpStyle.Initialize(getter());
		}

		public delegate void Setter(IDashPattern c);

		public static void ApplyStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Setter setter)
		{
			LineStyleGroupStyle grpStyle = null;
			IPlotGroupStyleCollection grpColl = null;
			if (externalGroups.ContainsType(typeof(LineStyleGroupStyle)))
				grpColl = externalGroups;
			else if (localGroups != null && localGroups.ContainsType(typeof(LineStyleGroupStyle)))
				grpColl = localGroups;

			if (null != grpColl)
			{
				grpStyle = (LineStyleGroupStyle)grpColl.GetPlotGroupStyle(typeof(LineStyleGroupStyle));
				grpColl.OnBeforeApplication(typeof(LineStyleGroupStyle));
				setter(grpStyle.DashStyle);
			}
		}

		#endregion Static helpers
	}
}