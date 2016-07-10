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
	using Graph.Plot.Groups;
	using Styles;

	public class ScatterSymbolGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private IScatterSymbol _shapeAndStyle;
		private bool _isStepEnabled = true;

		/// <summary>
		/// The list of symbols to switch through
		/// </summary>
		private ScatterSymbolList _symbolList;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScatterSymbolGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ScatterSymbolGroupStyle s = (ScatterSymbolGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);

				if (s._isStepEnabled)
					info.AddValue("SymbolList", s._symbolList);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ScatterSymbolGroupStyle s = null != o ? (ScatterSymbolGroupStyle)o : new ScatterSymbolGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");

				if (s._isStepEnabled)
					s._symbolList = (ScatterSymbolList)info.GetValue("SymbolList", s);
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		static ScatterSymbolGroupStyle()
		{
		}

		public ScatterSymbolGroupStyle()
		{
			_shapeAndStyle = new Styles.ScatterSymbols.Cube();
		}

		public ScatterSymbolGroupStyle(ScatterSymbolGroupStyle from)
		{
			this._isInitialized = from._isInitialized;
			this._shapeAndStyle = from._shapeAndStyle;
			this._symbolList = from._symbolList;
		}

		#endregion Constructors

		#region ICloneable Members

		public ScatterSymbolGroupStyle Clone()
		{
			return new ScatterSymbolGroupStyle(this);
		}

		object ICloneable.Clone()
		{
			return new ScatterSymbolGroupStyle(this);
		}

		#endregion ICloneable Members

		#region IGroupStyle Members

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			ScatterSymbolGroupStyle from = (ScatterSymbolGroupStyle)fromb;
			this._isInitialized = from._isInitialized;
			this._shapeAndStyle = from._shapeAndStyle;
			this._symbolList = from._symbolList;
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

			if (null == _symbolList)
				_symbolList = ScatterSymbolList.BuiltinDefault;

			var list = _symbolList;
			var listcount = list.Count;

			if (listcount == 0)
			{
				return 0;
			}

			var idx = list.IndexOf(_shapeAndStyle);
			if (idx < 0)
				idx = 0;

			var destIdx = idx + step;
			int wraps = 0;
			if (destIdx >= 0)
			{
				while (destIdx >= listcount)
				{
					++wraps;
					destIdx -= listcount;
				}
			}
			else
			{
				while (destIdx < 0)
				{
					++wraps;
					destIdx += listcount;
				}
			}

			_shapeAndStyle = list[destIdx];
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
		/// The list of symbols to switch through
		/// </summary>
		public ScatterSymbolList ScatterSymbolList
		{
			get
			{
				return _symbolList;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (!object.ReferenceEquals(_symbolList, value))
				{
					_symbolList = value;
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

		public void Initialize(IScatterSymbol s)
		{
			if (null == s)
				throw new ArgumentNullException(nameof(s));

			_isInitialized = true;
			_shapeAndStyle = s;
		}

		public IScatterSymbol ShapeAndStyle
		{
			get
			{
				return _shapeAndStyle;
			}
		}

		#endregion Other members

		#region Static helpers

		public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
		{
			if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(ScatterSymbolGroupStyle)))
			{
				ScatterSymbolGroupStyle gstyle = new ScatterSymbolGroupStyle();
				gstyle.IsStepEnabled = true;
				externalGroups.Add(gstyle);
			}
		}

		public static void AddLocalGroupStyle(
		 IPlotGroupStyleCollection externalGroups,
		 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ScatterSymbolGroupStyle)))
				localGroups.Add(new ScatterSymbolGroupStyle());
		}

		public delegate IScatterSymbol Getter();

		public static void PrepareStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Getter getter)
		{
			if (!externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle))
				&& null != localGroups
				&& !localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
			{
				localGroups.Add(new ScatterSymbolGroupStyle());
			}

			ScatterSymbolGroupStyle grpStyle = null;
			if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
				grpStyle = (ScatterSymbolGroupStyle)externalGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
			else if (localGroups != null)
				grpStyle = (ScatterSymbolGroupStyle)localGroups.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));

			if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
				grpStyle.Initialize(getter());
		}

		public delegate void Setter(IScatterSymbol val);

		public static void ApplyStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Setter setter)
		{
			ScatterSymbolGroupStyle grpStyle = null;
			IPlotGroupStyleCollection grpColl = null;
			if (externalGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
				grpColl = externalGroups;
			else if (localGroups != null && localGroups.ContainsType(typeof(ScatterSymbolGroupStyle)))
				grpColl = localGroups;

			if (null != grpColl)
			{
				grpStyle = (ScatterSymbolGroupStyle)grpColl.GetPlotGroupStyle(typeof(ScatterSymbolGroupStyle));
				grpColl.OnBeforeApplication(typeof(ScatterSymbolGroupStyle));
				setter(grpStyle.ShapeAndStyle);
			}
		}

		#endregion Static helpers
	}
}