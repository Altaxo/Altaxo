using System;
namespace Altaxo.Graph.Gdi.Shapes
{
	public interface IGraphicBase
		:
		Main.IChangedEventSource,
		Main.IDocumentNode,
		Main.ICopyFrom
	{
		object ParentObject { set; }
		//bool AllowNegativeSize { get; }
		//bool AutoSize { get; }
		//System.Drawing.Drawing2D.GraphicsPath GetObjectOutlineForArrangements();
		//System.Drawing.Drawing2D.GraphicsPath GetRectangularObjectOutline();
		//double Height { get; set; }
		Altaxo.Graph.Gdi.IHitTestObject HitTest(Altaxo.Graph.Gdi.HitTestPointData hitData);
		//Altaxo.Graph.Gdi.IHitTestObject HitTest(Altaxo.Graph.Gdi.HitTestRectangularData rectHit);
		//void OnDeserialization(object obj);
		void Paint(System.Drawing.Graphics g, object obj);
		//Altaxo.Graph.PointD2D ParentCoordinatesToLocalDifference(Altaxo.Graph.PointD2D pivot, Altaxo.Graph.PointD2D point);
		//Altaxo.Graph.PointD2D Position { get; set; }
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
