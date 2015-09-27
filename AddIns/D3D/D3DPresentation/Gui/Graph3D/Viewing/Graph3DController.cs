using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Graph3D.Viewing
{
	using Altaxo.Graph3D;
	using Altaxo.Graph3D.Camera;

	public class Graph3DController : IDisposable, IMVCANController
	{
		public event EventHandler TitleNameChanged;

		public object _view;

		public Altaxo.Graph3D.GraphDocument3D Doc { get; set; }

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
				_view = value;

				if (_view is IGraph3DView)
				{
					((IGraph3DView)_view).Controller = this;
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
			if (null == args || args.Length == 0 || !(args[0] is Altaxo.Graph3D.GraphDocument3D))
				return false;

			Doc = (Altaxo.Graph3D.GraphDocument3D)args[0];

			return true;
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
	}
}