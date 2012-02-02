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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;

namespace Altaxo.Graph.Gdi.Axis
{

	/// <remarks>AbstractXYAxisLabelStyle is the abstract base class of all LabelStyles.</remarks>
	public abstract class AxisLabelStyleBase
		:
		Main.IChangedEventSource,
		IRoutedPropertyReceiver,
		Main.IDocumentNode,
		System.ICloneable
	{
		[field: NonSerialized]
		event System.EventHandler _changed;

		[NonSerialized]
		object _parent;

		/// <summary>
		/// Paints the axis style labels.
		/// </summary>
		/// <param name="g">Graphics environment.</param>
		/// <param name="coordSyst">The coordinate system. Used to get the path along the axis.</param>
		/// <param name="scaleWithTicks">Scale and appropriate ticks.</param>
		/// <param name="styleInfo">Information about begin of axis, end of axis.</param>
		/// <param name="outerDistance">Distance between axis and labels.</param>
		/// <param name="useMinorTicks">If true, minor ticks are shown.</param>
		public abstract void Paint(
			Graphics g,
			G2DCoordinateSystem coordSyst,
			ScaleWithTicks scaleWithTicks,
			CSAxisInformation styleInfo,
			double outerDistance,
			bool useMinorTicks);

		#region IChangedEventSource Members

		event EventHandler Altaxo.Main.IChangedEventSource.Changed
		{
			add { _changed += value; }
			remove { _changed -= value; }
		}


		protected virtual void OnChanged()
		{
			if (_parent is Main.IChildChangedEventSink)
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, EventArgs.Empty);

			if (null != _changed)
				_changed(this, new EventArgs());
		}

		#endregion

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		public abstract object Clone();

		public abstract IHitTestObject HitTest(IPlotArea layer, PointD2D pt);

		public abstract double FontSize { get; set; }

		#region IDocumentNode Members

		public object ParentObject
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public string Name
		{
			get { return this.GetType().Name; }
		}

		#endregion

		#region IRoutedPropertyReceiver Members

		public virtual void SetRoutedProperty(IRoutedSetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					{
						var prop = (RoutedSetterProperty<double>)property;
						this.FontSize = prop.Value;
						OnChanged();
					}
					break;
			}
		}

		public virtual void GetRoutedProperty(IRoutedGetterProperty property)
		{
			switch (property.Name)
			{
				case "FontSize":
					((RoutedGetterProperty<double>)property).Merge(this.FontSize);
					break;
			}
		}

		#endregion
	}


}
