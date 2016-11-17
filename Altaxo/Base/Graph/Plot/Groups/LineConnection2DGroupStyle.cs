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

using Altaxo.Graph.Gdi.Plot.Styles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
	/// <summary>
	/// This group style is intended to make sure that all substyles have the same line connection.
	/// Thus it is only intended for local use (only amound substyles of a single plot item).
	/// plot styles.
	/// </summary>
	public class LineConnection2DGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private ILineConnectionStyle _lineConnectionStyle;
		private static readonly Type MyType = typeof(LineConnection2DGroupStyle);

		#region Serialization

		/// <summary>
		/// 2016-11-17 Initial version
		/// </summary>
		/// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LineConnection2DGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				LineConnection2DGroupStyle s = (LineConnection2DGroupStyle)obj;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				LineConnection2DGroupStyle s = null != o ? (LineConnection2DGroupStyle)o : new LineConnection2DGroupStyle();
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public LineConnection2DGroupStyle()
		{
		}

		public LineConnection2DGroupStyle(LineConnection2DGroupStyle from)
		{
			this._isInitialized = from._isInitialized;
			this._lineConnectionStyle = from._lineConnectionStyle;
		}

		#endregion Constructors

		#region ICloneable Members

		public LineConnection2DGroupStyle Clone()
		{
			return new LineConnection2DGroupStyle(this);
		}

		object ICloneable.Clone()
		{
			return new LineConnection2DGroupStyle(this);
		}

		#endregion ICloneable Members

		#region IGroupStyle Members

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			LineConnection2DGroupStyle from = (LineConnection2DGroupStyle)fromb;
			this._isInitialized = from._isInitialized;
			_lineConnectionStyle = from._lineConnectionStyle;
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
				return false;
			}
		}

		public bool CanStep
		{
			get
			{
				return false;
			}
		}

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

		public bool IsInitialized
		{
			get
			{
				return _isInitialized;
			}
		}

		public void Initialize(ILineConnectionStyle lineConnectionStyle)
		{
			_isInitialized = true;
			_lineConnectionStyle = lineConnectionStyle;
		}

		public ILineConnectionStyle LineConnectionStyle
		{
			get
			{
				return _lineConnectionStyle;
			}
		}

		#endregion Other members

		#region Static helpers

		public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
		{
			// this group style is local only, so no addition is made here
		}

		public static void AddLocalGroupStyle(
		 IPlotGroupStyleCollection externalGroups,
		 IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(LineConnection2DGroupStyle)))
				localGroups.Add(new LineConnection2DGroupStyle());
		}

		public static void PrepareStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Func<ILineConnectionStyle> getter)
		{
			if (!externalGroups.ContainsType(typeof(LineConnection2DGroupStyle))
				&& null != localGroups
				&& !localGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
			{
				localGroups.Add(new LineConnection2DGroupStyle());
			}

			LineConnection2DGroupStyle grpStyle = null;
			if (externalGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
				grpStyle = (LineConnection2DGroupStyle)externalGroups.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));
			else if (localGroups != null)
				grpStyle = (LineConnection2DGroupStyle)localGroups.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));

			if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
				grpStyle.Initialize(getter());
		}

		/// <summary>
		/// Try to apply the symbol size group style. Returns true if successfull applied.
		/// </summary>
		/// <param name="externalGroups"></param>
		/// <param name="localGroups"></param>
		/// <param name="setter"></param>
		/// <returns></returns>
		public static bool ApplyStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Action<ILineConnectionStyle> setter)
		{
			LineConnection2DGroupStyle grpStyle = null;
			IPlotGroupStyleCollection grpColl = null;
			if (externalGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
				grpColl = externalGroups;
			else if (localGroups != null && localGroups.ContainsType(typeof(LineConnection2DGroupStyle)))
				grpColl = localGroups;

			if (null != grpColl)
			{
				grpStyle = (LineConnection2DGroupStyle)grpColl.GetPlotGroupStyle(typeof(LineConnection2DGroupStyle));
				grpColl.OnBeforeApplication(typeof(LineConnection2DGroupStyle));
				setter(grpStyle.LineConnectionStyle);
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