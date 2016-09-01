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

using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
	/// <summary>
	/// Style used to group plot items by color, and to step between the colors in a plot color set.
	/// </summary>
	/// <remarks>
	/// We must take into account, that some plot color sets could not be reconstructed on deserialization (for instance special user or application color sets). This is for instance
	/// the case for a single plot item, that uses a plot color, but is not member of an external plot group style. Because normally, if an external plot group style is present, the colors in
	/// the plot color set of the PlotGroupStyle will all be serialized. But here, without an external plot group style, this is not the case, and on deserialization, the plot color set could not be
	/// reconstructed.
	/// The issue is, that the color of the deserialized plot item should not change. Before 2013-02-05, the color changed because the substyle of the plot style of the plot item acts as a ColorProvider,
	/// and as such, was creating a PlotGroupStyle for local use. And the PlotGroupStyle coerced the value of the color to a valid plot color during initialization, and thus changed the color.
	/// This problem was solved 2013-02-05 by distinguishing between local use and external use of the ColorGroupStyle. For external use, the color is (as before) coerced to a valid plot color
	/// during initialization. For local use, the rules are not so strict, and it is allowed to provide an non plot color or an invalid plot color during initialization without changing the color value.
	/// </remarks>
	public class ColorGroupStyle
		:
		Main.SuspendableDocumentLeafNodeWithEventArgs,
		IPlotGroupStyle
	{
		private bool _isInitialized;
		private bool _isStepEnabled;
		private Drawing.ColorManagement.IColorSet _listOfValues;

		/// <summary>Index of the current color in the color set.</summary>
		private int _colorIndex;

		/// <summary>
		/// The current color of this group style. This color is cached and should never be serialized. It is allowed that this color is not a valid plot color, but only when the group style is used only locally (inside of a plot style collection),
		/// so that it is never serialized. In this case the <see cref="_listOfValues"/> is <c>null</c>, and the _colorIndex is undefined.
		/// </summary>
		private NamedColor _cachedColor;

		/// <summary>
		/// If <c>true</c> indicates that this group style is used only inside a plot style collection and thus should never be serialized. In this case it is allowed the group style is initialized with
		/// an invalid plot color.
		/// </summary>
		private bool _isLocalGroupStyle;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorGroupStyle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ColorGroupStyle s = (ColorGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ColorGroupStyle s = null != o ? (ColorGroupStyle)o : ColorGroupStyle.NewExternalGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				return s;
			}
		}

		/// <summary>
		///
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorGroupStyle), 1)] // 2011-05-11 adding ColorSet
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ColorGroupStyle s = (ColorGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
				info.AddValue("ColorSet", s._listOfValues);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ColorGroupStyle s = null != o ? (ColorGroupStyle)o : ColorGroupStyle.NewExternalGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				s._listOfValues = (Drawing.ColorManagement.IColorSet)info.GetValue("ColorSet", s);
				ColorSetManager.Instance.TryRegisterList(s._listOfValues, Main.ItemDefinitionLevel.Project, out s._listOfValues);

				return s;
			}
		}

		/// <summary>
		/// <para>Date: 2012-10-25</para>
		/// <para>Add: ColorIndex</para>
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorGroupStyle), 2)]
		private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ColorGroupStyle s = (ColorGroupStyle)obj;
				info.AddValue("StepEnabled", s._isStepEnabled);
				info.AddValue("ColorSet", s._listOfValues);
				info.AddValue("ColorIndex", s._colorIndex);

				if (s._isLocalGroupStyle)
					throw new ArgumentOutOfRangeException("Trying to serialize a local ColorPlotGroupStyle is not allowed. Please report this bug to the forum.");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				ColorGroupStyle s = null != o ? (ColorGroupStyle)o : ColorGroupStyle.NewExternalGroupStyle();
				s._isStepEnabled = info.GetBoolean("StepEnabled");
				IColorSet listOfValues = (Drawing.ColorManagement.IColorSet)info.GetValue("ColorSet", s);
				if (null != listOfValues)
				{
					ColorSetManager.Instance.TryRegisterList(listOfValues, Main.ItemDefinitionLevel.Project, out s._listOfValues);
				}

				s._colorIndex = info.GetInt32("ColorIndex");
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		protected ColorGroupStyle(bool isLocalGroupStyle)
		{
			_listOfValues = ColorSetManager.Instance.BuiltinDarkPlotColors;
			_colorIndex = 0;
			_cachedColor = _listOfValues[_colorIndex];

			_isLocalGroupStyle = isLocalGroupStyle;
			if (isLocalGroupStyle)
			{
				_isStepEnabled = false;
			}
			else
			{
				_isStepEnabled = true;
			}
		}

		/// <summary>
		/// Initializes an external <see cref="ColorGroupStyle"/>. This constructor is for internal purposes (automatic construction) only,
		/// please use <see cref="NewExternalGroupStyle"/> or <see cref="NewLocalGroupStyle"/> instead.
		/// </summary>
		public ColorGroupStyle()
			: this(false)
		{
		}

		/// <summary>
		/// Creates a new group style for external use. See the remarks of the class documentation for what is the difference between instances for external use and local use.
		/// </summary>
		/// <returns>A new <see cref="ColorGroupStyle"/> instance for external use.</returns>
		public static ColorGroupStyle NewExternalGroupStyle()
		{
			return new ColorGroupStyle(false);
		}

		/// <summary>
		/// Creates a new group style for local use only. See the remarks of the class documentation for what is the difference between instances for external use and local use.
		/// </summary>
		/// <returns>A new <see cref="ColorGroupStyle"/> instance for local use. Those instances could not be serialized.</returns>
		public static ColorGroupStyle NewLocalGroupStyle()
		{
			return new ColorGroupStyle(true);
		}

		public ColorGroupStyle(ColorGroupStyle from)
		{
			this._isStepEnabled = from._isStepEnabled;
			this._isInitialized = from._isInitialized;
			this._listOfValues = from._listOfValues;
			this._colorIndex = from._colorIndex;
			this._cachedColor = from._cachedColor;
			this._isLocalGroupStyle = from._isLocalGroupStyle;
		}

		#endregion Constructors

		#region ICloneable Members

		public ColorGroupStyle Clone()
		{
			return new ColorGroupStyle(this);
		}

		object ICloneable.Clone()
		{
			return new ColorGroupStyle(this);
		}

		#endregion ICloneable Members

		#region IGroupStyle Members

		public void TransferFrom(IPlotGroupStyle fromb)
		{
			ColorGroupStyle from = (ColorGroupStyle)fromb;
			//System.Diagnostics.Debug.WriteLine(string.Format("ColorTransfer: myIni={0}, myCol={1}, fromI={2}, fromC={3}", _isInitialized, _color.Color.ToString(), from._isInitialized, from._color.Color.ToString()));
			this._isInitialized = from._isInitialized;
			this._listOfValues = from._listOfValues;
			this._colorIndex = from._colorIndex;
			this._cachedColor = from._cachedColor;
		}

		public void BeginPrepare()
		{
			_isInitialized = false;
			//System.Diagnostics.Debug.WriteLine(string.Format("ColorGroupStyle.BeginPrepare"));
		}

		public void PrepareStep()
		{
		}

		public void EndPrepare()
		{
			//System.Diagnostics.Debug.WriteLine(string.Format("ColorGroupStyle.EndPrepare, ini={0}, col={1}",_isInitialized,_color.Color.ToString()));
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
			if (this._listOfValues == null)
				return 0;

			int wraps;
			this._colorIndex = ColorSetExtensions.GetNextPlotColorIndex(_listOfValues, _colorIndex, step, out wraps);
			this._cachedColor = InternalGetColorFromColorSetAndIndex();
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

		public void Initialize(NamedColor c)
		{
			// we will not accept the known color set here
			// this has historical reasons: until 2012 we don't even have the concept of color sets
			// thus all plot colors were part of the know color set, and we could not distinguish between known colors and plot colors
			if (null != c.ParentColorSet &&
					!object.ReferenceEquals(c.ParentColorSet, ColorSetManager.Instance.BuiltinKnownColors)
				)
			{
				_listOfValues = c.ParentColorSet;
				ColorSetManager.Instance.DeclareAsPlotColorList(_listOfValues);
			}

			_colorIndex = Math.Max(0, _listOfValues.IndexOf(c));
			_cachedColor = _listOfValues[_colorIndex];
			_isInitialized = true;
		}

		public NamedColor Color
		{
			get
			{
				return _cachedColor;
			}
		}

		/// <summary>
		/// The list of symbols to switch through
		/// </summary>
		public Drawing.ColorManagement.IColorSet ListOfValues
		{
			get
			{
				return _listOfValues;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				// we will not accept the known color set here
				// this has historical reasons: until 2012 we don't even have the concept of color sets
				// thus all plot colors were part of the know color set, and we could not distinguish between known colors and plot colors
				if (object.ReferenceEquals(value, ColorSetManager.Instance.BuiltinKnownColors))
					throw new ArgumentException(string.Format("The color set {0} is not allowed to be a plot color set", ColorSetManager.Instance.BuiltinKnownColors.Name));

				if (!object.ReferenceEquals(_listOfValues, value))
				{
					_listOfValues = value;
					int idx = _listOfValues.IndexOf(_cachedColor);
					if (idx < 0)
					{
						_colorIndex = 0;
						_cachedColor = _listOfValues[0];
					}
					else
					{
						_colorIndex = idx;
						_cachedColor = _listOfValues[idx];
					}
					ColorSetManager.Instance.DeclareAsPlotColorList(_listOfValues);

					EhSelfChanged();
				}
			}
		}

		private NamedColor InternalGetColorFromColorSetAndIndex()
		{
			if (_listOfValues != null && _listOfValues.Count > 0)
			{
				if (_colorIndex < 0)
					_colorIndex = 0;
				if (_colorIndex >= _listOfValues.Count)
					_colorIndex = _listOfValues.Count - 1;
				return _listOfValues[_colorIndex];
			}
			else
			{
				return ColorSetManager.Instance.BuiltinDarkPlotColors[0];
			}
		}

		#endregion Other members

		#region Static helpers

		public static void AddExternalGroupStyle(IPlotGroupStyleCollection externalGroups)
		{
			if (PlotGroupStyle.ShouldAddExternalGroupStyle(externalGroups, typeof(ColorGroupStyle)))
			{
				ColorGroupStyle gstyle = ColorGroupStyle.NewExternalGroupStyle();
				externalGroups.Add(gstyle);
			}
		}

		public static void AddLocalGroupStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups)
		{
			if (PlotGroupStyle.ShouldAddLocalGroupStyle(externalGroups, localGroups, typeof(ColorGroupStyle)))
				localGroups.Add(ColorGroupStyle.NewLocalGroupStyle());
		}

		public delegate NamedColor Getter();

		public static void PrepareStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Getter getter)
		{
			if (!externalGroups.ContainsType(typeof(ColorGroupStyle))
				&& null != localGroups
				&& !localGroups.ContainsType(typeof(ColorGroupStyle)))
			{
				localGroups.Add(ColorGroupStyle.NewLocalGroupStyle());
			}

			ColorGroupStyle grpStyle = null;
			if (externalGroups.ContainsType(typeof(ColorGroupStyle)))
				grpStyle = (ColorGroupStyle)externalGroups.GetPlotGroupStyle(typeof(ColorGroupStyle));
			else if (localGroups != null)
				grpStyle = (ColorGroupStyle)localGroups.GetPlotGroupStyle(typeof(ColorGroupStyle));

			if (grpStyle != null && getter != null && !grpStyle.IsInitialized)
				grpStyle.Initialize(getter());
		}

		public delegate void Setter(NamedColor c);

		public static void ApplyStyle(
			IPlotGroupStyleCollection externalGroups,
			IPlotGroupStyleCollection localGroups,
			Setter setter)
		{
			ColorGroupStyle grpStyle = null;
			IPlotGroupStyleCollection grpColl = null;
			if (externalGroups.ContainsType(typeof(ColorGroupStyle)))
				grpColl = externalGroups;
			else if (localGroups != null && localGroups.ContainsType(typeof(ColorGroupStyle)))
				grpColl = localGroups;

			if (null != grpColl)
			{
				grpStyle = (ColorGroupStyle)grpColl.GetPlotGroupStyle(typeof(ColorGroupStyle));
				grpColl.OnBeforeApplication(typeof(ColorGroupStyle));
				setter(grpStyle.Color);
			}
		}

		#endregion Static helpers
	}
}