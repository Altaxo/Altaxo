using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Grips
{
	/// <summary>
	/// Used to manipulate an object by dragging it's grip area around.
	/// </summary>
	public interface IGripManipulationHandle
	{
		/// <summary>
		/// Activates this grip, providing the initial position of the mouse.
		/// </summary>
		/// <param name="initialPosition">Initial position of the mouse.</param>
		/// <param name="isActivatedUponCreation">If true the activation is called right after creation of this handle. If false,
		/// thie activation is due to a regular mouse click in this grip.</param>
		void Activate(PointD2D initialPosition, bool isActivatedUponCreation);

		/// <summary>
		/// Announces the deactivation of this grip.
		/// </summary>
		/// <returns>True if the nextgrip level should be displayed, otherwise false.</returns>
		bool Deactivate();

		/// <summary>
		/// Moves the grip to the new position.
		/// </summary>
		/// <param name="newPosition"></param>
		void MoveGrip(PointD2D newPosition);

		/// <summary>
		/// Draws the grip in the graphics context.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="pageScale">Current zoom factor that can be used to calculate pen width etc. for displaying the handle (see remarks). Attention: this factor must not be used to transform the path of the handle.</param>
		/// <remarks>
		/// To paint a handle that should appear with a line width of 2.5 pts on the screen, you should create a pen as follows:
		/// <code language="C#">
		/// using(var pen = new Pen(Color.Blue, (float)(2.5/pageScale)))
		///	{
		///		// do your painting here
		///	}
		/// </code>
		/// </remarks>
		void Show(IOverlayContext3D g, double pageScale);

		/// <summary>
		/// Tests if the grip is hitted.
		/// </summary>
		/// <param name="point">Coordinates of the mouse pointer in unscaled page coordinates (points).</param>
		/// <returns></returns>
		bool IsGripHitted(PointD2D point);
	}
}