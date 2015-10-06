using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Collections;
	using Altaxo.Graph3D;
	using Altaxo.Graph3D.Camera;

	public class Graph3DController : IDisposable, IMVCANController
	{
		public event EventHandler TitleNameChanged;

		public IGraph3DView _view;

		protected GraphDocument3D _doc;

		public GraphDocument3D Doc { get { return _doc; } }

		/// <summary>Number of the currently selected layer (or null if no layer is present).</summary>
		protected IList<int> _currentLayerNumber = new List<int>();

		/// <summary>Number of the currently selected plot (or -1 if no plot is present on the layer).</summary>
		protected int _currentPlotNumber = -1;

		public object ModelObject
		{
			get
			{
				return Doc;
			}
		}

		public virtual object ViewObject
		{
			get
			{
				return _view;
			}

			set
			{
				if (_view != null)
					_view.Controller = null;

				_view = value as IGraph3DView;

				if (_view != null)
				{
					_view.Controller = this;
				}
			}
		}

		public UseDocument UseDocumentCopy
		{
			set
			{
			}
		}

		public IList<object> SelectedObjects { get; internal set; }

		public void Dispose()
		{
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is GraphDocument3D))
				return false;

			_doc = (GraphDocument3D)args[0];

			return true;
		}

		/// <summary>
		/// Returns the currently active layer. There is always an active layer.
		/// </summary>
		public HostLayer3D ActiveLayer
		{
			get
			{
				return _doc.RootLayer.ElementAt(this._currentLayerNumber);
			}
			set
			{
				CurrentLayerNumber = value.IndexOf();
			}
		}

		/// <summary>
		/// Get / sets the currently active layer by number.
		/// </summary>
		public IList<int> CurrentLayerNumber
		{
			get
			{
				EnsureValidityOfCurrentLayerNumber();
				return new List<int>(_currentLayerNumber);
			}
			set
			{
				// negative values are only accepted if there is no layer
				if (value == null)
					throw new ArgumentNullException("CurrentLayerNumber");

				if (!_doc.RootLayer.IsValidIndex(value))
					throw new ArgumentOutOfRangeException("CurrentLayerNumber", value, "The provided layer number was invalid");

				bool isDifferent = !System.Linq.Enumerable.SequenceEqual(value, _currentLayerNumber);

				_currentLayerNumber = new List<int>(value);

				// if something changed
				if (isDifferent)
				{
					// reflect the change in layer number in the layer tool bar
					if (_view != null)
						_view.CurrentLayer = this._currentLayerNumber.ToArray();
				}
			}
		}

		/// <summary>
		/// check the validity of the CurrentLayerNumber and correct it
		/// </summary>
		public HostLayer3D EnsureValidityOfCurrentLayerNumber()
		{
			_doc.RootLayer.EnsureValidityOfNodeIndex(_currentLayerNumber);
			return _doc.RootLayer.ElementAt(_currentLayerNumber);
		}

		/// <summary>
		/// Handles the selection of the current layer by the <b>user</b>.
		/// </summary>
		/// <param name="currLayer">The current layer number as selected by the user.</param>
		/// <param name="bAlternative">Normally false, can be set to true if the user clicked for instance with the right mouse button on the layer button.</param>
		public virtual void EhView_CurrentLayerChoosen(int[] currLayer, bool bAlternative)
		{
			var oldCurrLayer = this.CurrentLayerNumber;
			this.CurrentLayerNumber = new List<int>(currLayer);

			// if we have clicked the button already down then open the layer dialog
			if (null != ActiveLayer && System.Linq.Enumerable.SequenceEqual(_currentLayerNumber, oldCurrLayer) && false == bAlternative)
			{
				var activeLayer = ActiveLayer;
				if (activeLayer is XYPlotLayer3D)
					XYPlotLayer3DController.ShowDialog((XYPlotLayer3D)activeLayer);
				else
					HostLayer3DController.ShowDialog(activeLayer);
			}
		}

		internal void CutSelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void CopySelectedObjectsToClipboard()
		{
			throw new NotImplementedException();
		}

		internal void PasteObjectsFromClipboard()
		{
			throw new NotImplementedException();
		}

		internal void RemoveSelectedObjects()
		{
			throw new NotImplementedException();
		}

		public bool Apply(bool disposeController)
		{
			return true;
		}

		public bool Revert(bool disposeController)
		{
			return true;
		}

		internal void ViewFront()
		{
			var up = new Altaxo.Graph3D.VectorD3D(0, 0, 1);
			var target = new Altaxo.Graph3D.PointD3D(Doc.RootLayer.Size.X / 2, Doc.RootLayer.Size.Y / 2, Doc.RootLayer.Size.Z / 2);
			var eye = new Altaxo.Graph3D.PointD3D(0, -750, 0) + (Altaxo.Graph3D.VectorD3D)target;

			var newCamera = (Altaxo.Graph3D.Camera.CameraBase)Doc.Scene.Camera.Clone();

			newCamera.UpVector = up;
			newCamera.TargetPosition = target;
			newCamera.EyePosition = eye;

			Doc.Scene.Camera = newCamera;
		}

		internal void ViewTop()
		{
			var up = new Altaxo.Graph3D.VectorD3D(0, 1, 0);
			var target = new Altaxo.Graph3D.PointD3D(Doc.RootLayer.Size.X / 2, Doc.RootLayer.Size.Y / 2, Doc.RootLayer.Size.Z / 2);
			var eye = new Altaxo.Graph3D.PointD3D(0, 0, 750) + (Altaxo.Graph3D.VectorD3D)target;

			var newCamera = (Altaxo.Graph3D.Camera.CameraBase)Doc.Scene.Camera.Clone();

			newCamera.UpVector = up;
			newCamera.TargetPosition = target;
			newCamera.EyePosition = eye;

			Doc.Scene.Camera = newCamera;
		}

		public void ViewRightFrontTop()
		{
			var up = new Altaxo.Graph3D.VectorD3D(0, 0, 1);
			var target = new Altaxo.Graph3D.PointD3D(Doc.RootLayer.Size.X / 2, Doc.RootLayer.Size.Y / 2, Doc.RootLayer.Size.Z / 2);
			var eye = new Altaxo.Graph3D.PointD3D(250, -500, 500) + (Altaxo.Graph3D.VectorD3D)target;

			var newCamera = (Altaxo.Graph3D.Camera.CameraBase)Doc.Scene.Camera.Clone();

			newCamera.UpVector = up;
			newCamera.TargetPosition = target;
			newCamera.EyePosition = eye;

			Doc.Scene.Camera = newCamera;
		}

		public void EhMouseWheel(double relX, double relY, double aspectRatio, int delta)
		{
			if (Doc.Scene.Camera is OrthographicCamera)
			{
				var cam = Doc.Scene.Camera as OrthographicCamera;
				var eye = cam.NormalizedEyeVector;
				var up = cam.NormalizedUpVectorPerpendicularToEyeVector;
				var scaleBefore = cam.Scale;
				var scaleAfter = delta < 0 ? scaleBefore / 1.25 : scaleBefore * 1.25;

				var tam1h = relX - 0.5;
				var tbm1h = relY - 0.5;

				var shift = new VectorD3D(
					-(scaleAfter - scaleBefore) * (aspectRatio * tbm1h * up.X + eye.Z * tam1h * up.Y - eye.Y * tam1h * up.Z),
					(scaleAfter - scaleBefore) * (eye.Z * tam1h * up.X - aspectRatio * tbm1h * up.Y - eye.X * tam1h * up.Z),
					-(scaleAfter - scaleBefore) * (eye.Y * tam1h * up.X - eye.X * tam1h * up.Y + aspectRatio * tbm1h * up.Z)
					);

				var newCamera = (OrthographicCamera)Doc.Scene.Camera.Clone();
				newCamera.EyePosition += shift;
				newCamera.TargetPosition += shift;
				newCamera.Scale = scaleAfter;
				Doc.Scene.Camera = newCamera;
			}
		}

		public void EhMoveOrRoll(double stepX, double stepY, bool isControlPressed)
		{
			if (isControlPressed)
				EhRoll(stepX, stepY);
			else
				EhMove(stepX, stepY);
		}

		public void EhRoll(double stepX, double stepY)
		{
			var cam = Doc.Scene.Camera;

			// the axis to turn the camera around is in case of stepY the Cross of UpVector and eyeVector
			// in case of stepX it is the cross of the Upvector and the cross of UpVector and eyeVector

			if (stepX != 0)
			{
				double angleRadian = Math.PI * stepX / 18;
				VectorD3D axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, Doc.Scene.Camera.UpVector)));
				var matrix = MatrixD3D.CreateRotationMatrixFromAxisAndAngleRadian(axis, angleRadian, Doc.Scene.Camera.TargetPosition);

				var newEye = matrix.TransformPoint(Doc.Scene.Camera.EyePosition);
				var newUp = matrix.Transform(Doc.Scene.Camera.UpVector);
				var newCamera = (CameraBase)Doc.Scene.Camera.Clone();
				newCamera.EyePosition = newEye;
				newCamera.UpVector = newUp;
				Doc.Scene.Camera = newCamera;
			}

			if (stepY != 0)
			{
				double angleRadian = Math.PI * stepY / 18;
				VectorD3D axis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.NormalizedEyeVector, Doc.Scene.Camera.UpVector));
				var matrix = MatrixD3D.CreateRotationMatrixFromAxisAndAngleRadian(axis, angleRadian, Doc.Scene.Camera.TargetPosition);

				var newEye = matrix.TransformPoint(Doc.Scene.Camera.EyePosition);
				var newUp = matrix.Transform(Doc.Scene.Camera.UpVector);
				var newCamera = (CameraBase)Doc.Scene.Camera.Clone();
				newCamera.EyePosition = newEye;
				newCamera.UpVector = newUp;
				Doc.Scene.Camera = newCamera;
			}
		}

		public void EhMove(double stepX, double stepY)
		{
			VectorD3D xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.NormalizedEyeVector, Doc.Scene.Camera.UpVector));
			VectorD3D yaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, VectorD3D.CrossProduct(Doc.Scene.Camera.EyeVector, Doc.Scene.Camera.UpVector)));

			if (Doc.Scene.Camera is OrthographicCamera)
			{
				var newCamera = Doc.Scene.Camera.Clone() as OrthographicCamera;

				var shift = (xaxis * stepX + yaxis * stepY) * (newCamera.Scale / 20);

				newCamera.EyePosition += shift;
				newCamera.TargetPosition += shift;

				Doc.Scene.Camera = newCamera;
			}
		}

		/// <summary>
		/// Looks for a graph object at pixel position <paramref name="pixelPos"/> and returns true if one is found.
		/// </summary>
		/// <param name="relativeScreenPosition">The relative screen coordinates. X and Y are relative screen coordinate values, Z is the screen's aspect ratio.</param>
		/// <param name="plotItemsOnly">If true, only the plot items where hit tested.</param>
		/// <param name="foundObject">Found object if there is one found, else null</param>
		/// <param name="foundInLayerNumber">The layer the found object belongs to, otherwise 0</param>
		/// <returns>True if a object was found at the pixel coordinates <paramref name="pixelPos"/>, else false.</returns>
		public bool FindGraphObjectAtPixelPosition(PointD3D relativeScreenPosition, bool plotItemsOnly, out IHitTestObject foundObject, out int[] foundInLayerNumber)
		{
			var camera = Doc.Scene.Camera;

			MatrixD3D hitmatrix = Doc.Scene.Camera.GetHitRayMatrix(relativeScreenPosition);
			var hitdata = new HitTestPointData(hitmatrix);

			foundObject = Doc.RootLayer.HitTest(hitdata, plotItemsOnly);

			if (null != foundObject && null != foundObject.ParentLayer)
			{
				foundInLayerNumber = foundObject.ParentLayer.IndexOf().ToArray();
				return true;
			}

			foundObject = null;
			foundInLayerNumber = null;
			return false;
		}
	}
}