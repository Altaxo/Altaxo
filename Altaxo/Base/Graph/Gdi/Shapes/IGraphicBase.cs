#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Graph.Gdi.Shapes
{
	public interface IGraphicBase
		:
		Main.IChangedEventSource,
		Main.IDocumentNode,
		Main.ICopyFrom
	{
		new object ParentObject { set; }

		/// <summary>
		/// Announces the size of the parent layer in order to make own calculations for size and position.
		/// </summary>
		/// <param name="parentSize">Size of the parent layer.</param>
		/// <param name="isTriggeringChangedEvent">If set to <c>true</c>, the Changed event is triggered if the size of the parent differs from the cached parent's size.</param>
		void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent);

		//bool AllowNegativeSize { get; }
		//bool AutoSize { get; }
		//System.Drawing.Drawing2D.GraphicsPath GetObjectOutlineForArrangements();
		//System.Drawing.Drawing2D.GraphicsPath GetRectangularObjectOutline();
		//double Height { get; set; }
		Altaxo.Graph.Gdi.IHitTestObject HitTest(Altaxo.Graph.Gdi.HitTestPointData hitData);

		//Altaxo.Graph.Gdi.IHitTestObject HitTest(Altaxo.Graph.Gdi.HitTestRectangularData rectHit);
		//void OnDeserialization(object obj);

		void PaintPreprocessing(object parent);

		void Paint(System.Drawing.Graphics g, object obj);

		/// <summary>
		/// Determines whether this graphical object is compatible with the parent specified in the argument.
		/// </summary>
		/// <param name="parentObject">The parent object.</param>
		/// <returns><c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.</returns>
		bool IsCompatibleWithParent(object parentObject);

		//Altaxo.Graph.PointD2D ParentCoordinatesToLocalDifference(Altaxo.Graph.PointD2D pivot, Altaxo.Graph.PointD2D point);
		Altaxo.Graph.PointD2D Position { get; set; }

		//Altaxo.Graph.PointD2D RelativeLocalToAbsoluteLocalCoordinates(Altaxo.Graph.PointD2D p);
		//Altaxo.Graph.PointD2D RelativeLocalToAbsoluteParentCoordinates(Altaxo.Graph.PointD2D p);
		//Altaxo.Graph.PointD2D RelativeLocalToAbsoluteParentVector(Altaxo.Graph.PointD2D p);
		//Altaxo.Graph.PointD2D RelativeToAbsolutePosition(Altaxo.Graph.PointD2D p, bool withRotation);
		//double Rotation { get; set; }
		//Altaxo.Graph.PointD2D Scale { get; set; }
		//double ScaleX { get; }
		//double ScaleY { get; }
		//void SetBoundsFrom(Altaxo.Graph.PointD2D fixrPosition, Altaxo.Graph.PointD2D fixaPosition, Altaxo.Graph.PointD2D relDrawGrip, Altaxo.Graph.PointD2D diff, Altaxo.Graph.PointD2D initialSize);
		//object SetObjectData(object obj, System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Runtime.Serialization.ISurrogateSelector selector);
		//void SetPosition(Altaxo.Graph.PointD2D value);
		//void SetRotationFrom(Altaxo.Graph.PointD2D relPivot, Altaxo.Graph.PointD2D absPivot, Altaxo.Graph.PointD2D relDrawGrip, Altaxo.Graph.PointD2D diff);
		//void SetScalesFrom(Altaxo.Graph.PointD2D fixrPosition, Altaxo.Graph.PointD2D fixaPosition, Altaxo.Graph.PointD2D relDrawGrip, Altaxo.Graph.PointD2D diff, double initialScaleX, double initialScaleY);
		//void SetShearFrom(Altaxo.Graph.PointD2D fixrPosition, Altaxo.Graph.PointD2D fixaPosition, Altaxo.Graph.PointD2D relDrawGrip, Altaxo.Graph.PointD2D diff, double initialRotation, double initialShear, double initialScaleX, double initialScaleY);
		//double Shear { get; set; }
		//void SilentSetPosition(Altaxo.Graph.PointD2D newPosition);
		//Altaxo.Graph.PointD2D Size { get; set; }
		//Altaxo.Graph.PointD2D ToRotatedDifference(Altaxo.Graph.PointD2D pivot, Altaxo.Graph.PointD2D point);
		//Altaxo.Graph.PointD2D ToUnrotatedCoordinates(Altaxo.Graph.PointD2D pivot, Altaxo.Graph.PointD2D point);
		//Altaxo.Graph.PointD2D ToUnrotatedDifference(Altaxo.Graph.PointD2D diff);
		//Altaxo.Graph.PointD2D ToUnrotatedDifference(Altaxo.Graph.PointD2D pivot, Altaxo.Graph.PointD2D point);
		//double Width { get; set; }
		//double X { get; set; }
		//double Y { get; set; }
	}
}