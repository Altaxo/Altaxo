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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Altaxo.Graph.Gdi.LineCaps
{
	[Serializable]
	[System.ComponentModel.ImmutableObject(true)]
	public abstract class LineCapExtension
	{
		double _minimumAbsoluteSizePt;
		double _minimumRelativeSize;

		protected LineCapExtension()
		{
			_minimumAbsoluteSizePt = DefaultMinimumAbsoluteSizePt;
			_minimumRelativeSize = DefaultMinimumRelativeSize;
		}


		public abstract LineCapExtension Clone(double minimumAbsoluteSizePt, double minimumRelativeSize);


		protected LineCapExtension(double minimumAbsoluteSizePt, double minimumRelativeSize)
		{
			_minimumAbsoluteSizePt = minimumAbsoluteSizePt;
			_minimumRelativeSize = minimumRelativeSize;
		}


		public virtual double MinimumAbsoluteSizePt
		{
			get { return _minimumAbsoluteSizePt; }
		}


		public virtual double MinimumRelativeSize
		{
			get { return _minimumRelativeSize; }
		}

		public virtual void SetStartCap(Pen pen)
		{
			double size = Math.Max(_minimumAbsoluteSizePt, pen.Width * _minimumRelativeSize);
			SetStartCap(pen, (float)size);
		}

		public virtual void SetEndCap(Pen pen)
		{
			double size = Math.Max(_minimumAbsoluteSizePt, pen.Width * _minimumRelativeSize);
			SetEndCap(pen, (float)size);
		}

		public abstract string Name { get; }

		/// <summary>Gets the default minimum absolute size in points (1/72 inch).</summary>
		public abstract double DefaultMinimumAbsoluteSizePt { get; }

		/// <summary>Gets the default minimum relative size of the cap.</summary>
		/// <value>The value is multiplied with the pen with to get the minimum absolute size of the cap.</value>
		public abstract double DefaultMinimumRelativeSize { get; }

		public abstract void SetStartCap(Pen pen, float size);
		public abstract void SetEndCap(Pen pen, float size);

		public override bool Equals(object obj)
		{
			var from = obj as LineCapExtension;
			return ((null != from)
				&& (this._minimumAbsoluteSizePt == from._minimumAbsoluteSizePt)
				&& (this._minimumRelativeSize == from._minimumRelativeSize)
				&& (this.GetType() == from.GetType())
				);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() + _minimumAbsoluteSizePt.GetHashCode() + _minimumRelativeSize.GetHashCode();
		}

		public static bool operator ==(LineCapExtension a, LineCapExtension b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			return a.GetType() == b.GetType() && a._minimumAbsoluteSizePt == b._minimumAbsoluteSizePt && a._minimumRelativeSize == b._minimumRelativeSize;
		}

		public static bool operator !=(LineCapExtension a, LineCapExtension b)
		{
			return !(a == b);
		}


		#region Cap styles registry

		static LineCaps.FlatCap _defaultStyle;
		static System.Collections.Generic.SortedDictionary<string, LineCapExtension> _registeredStyles;
		static System.Collections.Generic.List<LineCapExtension> _registeredStylesSortedByName;
		static System.Collections.Generic.SortedDictionary<string, LineCapExtension> _deprecatedGdiStyles;

		public static LineCapExtension Flat
		{
			get { return _defaultStyle; }
		}

		public bool IsDefaultStyle
		{
			get
			{
				return this is LineCaps.FlatCap;
			}
		}

		public static LineCapExtension FromName(string name)
		{
			LineCapExtension currentStyle;

			if (_registeredStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle;
			}
			else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle;
			}
			else
				throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
		}

		public static LineCapExtension FromNameAndAbsSize(string name, double sizePt)
		{
			LineCapExtension currentStyle;

			if (_registeredStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle.Clone(sizePt, currentStyle.DefaultMinimumRelativeSize);
			}
			else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle;
			}
			else
				throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
		}


		public static LineCapExtension FromNameAndAbsAndRelSize(string name, double sizePt, double relSize)
		{
			LineCapExtension currentStyle;

			if (_registeredStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle.Clone(sizePt, relSize);
			}
			else if (_deprecatedGdiStyles.TryGetValue(name, out currentStyle))
			{
				return currentStyle;
			}
			else
			{
				throw new ArgumentException(string.Format("Unknown LineCapEx style: {0}", name), "name");
			}
		}

		public static IEnumerable<LineCapExtension> GetRegisteredValues()
		{
			return _registeredStylesSortedByName;
		}


		static LineCapExtension()
		{
			_defaultStyle = new LineCaps.FlatCap();

			// first register the old deprecated Gdi styles
			_deprecatedGdiStyles = new System.Collections.Generic.SortedDictionary<string, LineCapExtension>();
			foreach (LineCap cap in Enum.GetValues(typeof(LineCap)))
			{

				switch (cap)
				{
					case LineCap.AnchorMask:
					case LineCap.Flat:
					case LineCap.NoAnchor:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), _defaultStyle);
						break;
					case LineCap.Square:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.SquareFLineCap(0, 1));
						break;
					case LineCap.Round:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.CircleFLineCap(0, 1));
						break;
					case LineCap.Triangle:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.DiamondFLineCap(0, 1));
						break;
					case LineCap.ArrowAnchor:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.ArrowF10LineCap(0, 2));
						break;
					case LineCap.SquareAnchor:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.SquareFLineCap(0, 1.5));
						break;
					case LineCap.RoundAnchor:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.CircleFLineCap(0, 2));
						break;
					case LineCap.DiamondAnchor:
						_deprecatedGdiStyles.Add(Enum.GetName(typeof(LineCap), cap), new LineCaps.DiamondFLineCap(0, 2));
						break;
				}
			}

			// now the other linecaps
			_registeredStyles = new System.Collections.Generic.SortedDictionary<string, LineCapExtension>();
			var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(LineCapExtension));
			foreach (var t in types)
			{
				var ex = (LineCapExtension)Activator.CreateInstance(t);
				_registeredStyles.Add(ex.Name, ex);
			}

			// now sort them by name
			List<string> nameList = new List<string>(_registeredStyles.Keys);
			nameList.Remove(_defaultStyle.Name);
			nameList.Sort();
			nameList.Insert(0, _defaultStyle.Name);
			_registeredStylesSortedByName = new List<LineCapExtension>(nameList.Select(x => _registeredStyles[x]));
		}


		#endregion
	}
}
