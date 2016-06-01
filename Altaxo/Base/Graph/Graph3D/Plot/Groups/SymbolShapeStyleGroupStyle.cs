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

	public class SymbolShapeStyleGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private IScatterSymbolShape _shapeAndStyle;
		private bool _isStepEnabled = true;

		private static List<IScatterSymbolShape> _standardSymbolOrder;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SymbolShapeStyleGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				SymbolShapeStyleGroupStyle s = (SymbolShapeStyleGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				SymbolShapeStyleGroupStyle s = null != o ? (SymbolShapeStyleGroupStyle)o : new SymbolShapeStyleGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		static SymbolShapeStyleGroupStyle()
		{
			_standardSymbolOrder = new List<IScatterSymbolShape>();
		}

		public SymbolShapeStyleGroupStyle()
		{
		}

		public SymbolShapeStyleGroupStyle(SymbolShapeStyleGroupStyle from)
		{
			this._isInitialized = from._isInitialized;
			this._shapeAndStyle = from._shapeAndStyle;
		}

		#endregion Constructors

		#region ICloneable Members

		public SymbolShapeStyleGroupStyle Clone()
		{
			return new SymbolShapeStyleGroupStyle(this);
		}

		object ICloneable.Clone()
		{
			return new SymbolShapeStyleGroupStyle(this);
		}

		#endregion ICloneable Members

		#region IGroupStyle Members

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			SymbolShapeStyleGroupStyle from = (SymbolShapeStyleGroupStyle)fromb;
			this._isInitialized = from._isInitialized;
			this._shapeAndStyle = from._shapeAndStyle;
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
			var list = _standardSymbolOrder;
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

		#endregion IGroupStyle Members

		#region Other members

		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		public void Initialize(IScatterSymbolShape s)
		{
			_isInitialized = true;
			_shapeAndStyle = s;
		}

		public IScatterSymbolShape ShapeAndStyle
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
			if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(SymbolShapeStyleGroupStyle)))
			{
				SymbolShapeStyleGroupStyle gstyle = new SymbolShapeStyleGroupStyle();
				gstyle.IsStepEnabled = true;
				externalGroups.Add(gstyle);
			}
		}

		public static void AddLocalGroupStyle(
		 IPlotGroupStyleCollection externalGroups,
		 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(SymbolShapeStyleGroupStyle)))
				localGroups.Add(new SymbolShapeStyleGroupStyle());
		}

		public delegate IScatterSymbolShape Getter();

		public static void PrepareStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Getter getter)
		{
			if (!externalGroups.ContainsType(typeof(SymbolShapeStyleGroupStyle))
				&& null != localGroups
				&& !localGroups.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
			{
				localGroups.Add(new SymbolShapeStyleGroupStyle());
			}

			SymbolShapeStyleGroupStyle grpStyle = null;
			if (externalGroups.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
				grpStyle = (SymbolShapeStyleGroupStyle)externalGroups.GetPlotGroupStyle(typeof(SymbolShapeStyleGroupStyle));
			else if (localGroups != null)
				grpStyle = (SymbolShapeStyleGroupStyle)localGroups.GetPlotGroupStyle(typeof(SymbolShapeStyleGroupStyle));

			if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
				grpStyle.Initialize(getter());
		}

		public delegate void Setter(IScatterSymbolShape val);

		public static void ApplyStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Setter setter)
		{
			SymbolShapeStyleGroupStyle grpStyle = null;
			IPlotGroupStyleCollection grpColl = null;
			if (externalGroups.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
				grpColl = externalGroups;
			else if (localGroups != null && localGroups.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
				grpColl = localGroups;

			if (null != grpColl)
			{
				grpStyle = (SymbolShapeStyleGroupStyle)grpColl.GetPlotGroupStyle(typeof(SymbolShapeStyleGroupStyle));
				grpColl.OnBeforeApplication(typeof(SymbolShapeStyleGroupStyle));
				setter(grpStyle.ShapeAndStyle);
			}
		}

		#endregion Static helpers
	}
}