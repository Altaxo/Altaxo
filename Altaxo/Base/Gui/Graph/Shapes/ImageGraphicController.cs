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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IImageGraphicView
	{
		PointD2D SourceSize { set; }

		Altaxo.Graph.AspectRatioPreservingMode AspectPreserving { get; set; }

		bool IsSizeCalculationBasedOnSourceSize { get; set; }

		event Action AspectPreservingChanged;

		event Action ScalingModeChanged;

		object LocationView { set; }
	}

	[UserControllerForObject(typeof(ImageGraphic))]
	[ExpectedTypeOfView(typeof(IImageGraphicView))]
	public class ImageGraphicController : MVCANControllerEditOriginalDocBase<ImageGraphic, IImageGraphicView>
	{
		private PointD2D _srcSize;
		private PointD2D _docScale;
		private ItemLocationDirect _docLocation;
		private ItemLocationDirectController _locationController;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_srcSize = _doc.GetImageSizePt();
				_docScale = new PointD2D(_doc.Size.X / _srcSize.X, _doc.Size.Y / _srcSize.Y);
				_docLocation = new ItemLocationDirect();
				_docLocation.CopyFrom(_doc.Location);
				_docLocation.Scale = new PointD2D(_doc.Size.X / _srcSize.X, _doc.Size.Y / _srcSize.Y);
				_locationController = new ItemLocationDirectController() { UseDocumentCopy = UseDocument.Directly };
				_locationController.InitializeDocument(new object[] { _docLocation });
				Current.Gui.FindAndAttachControlTo(_locationController);

				_locationController.SizeXChanged += EhLocController_SizeXChanged;
				_locationController.SizeYChanged += EhLocController_SizeYChanged;
				_locationController.ScaleXChanged += EhLocController_ScaleXChanged;
				_locationController.ScaleYChanged += EhLocController_ScaleYChanged;
				_locationController.ShowSizeElements(true, !_doc.IsSizeCalculationBasedOnSourceSize);
				_locationController.ShowScaleElements(true, _doc.IsSizeCalculationBasedOnSourceSize);
			}
			if (_view != null)
			{
				_view.SourceSize = _srcSize;
				_view.AspectPreserving = _doc.AspectRatioPreserving;
				_view.IsSizeCalculationBasedOnSourceSize = _doc.IsSizeCalculationBasedOnSourceSize;
				_view.LocationView = _locationController.ViewObject;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_locationController.Apply(disposeController))
				return false;

			_docLocation = (ItemLocationDirect)_locationController.ModelObject;

			if (!object.ReferenceEquals(_doc.Location, _docLocation))
				_doc.Location.CopyFrom((ItemLocationDirect)_docLocation);

			// all other properties where already set during the session

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			_view.AspectPreservingChanged += EhAspectRatioPreservingChanged;
			_view.ScalingModeChanged += EhScalingModeChanged;
		}

		protected override void DetachView()
		{
			_view.AspectPreservingChanged -= EhAspectRatioPreservingChanged;
			_view.ScalingModeChanged -= EhScalingModeChanged;
		}

		private void EhAspectRatioPreservingChanged()
		{
			_doc.AspectRatioPreserving = _view.AspectPreserving;
			_docScale = new PointD2D(_doc.Size.X / _srcSize.X, _doc.Size.Y / _srcSize.Y);
			//_view.DocScale = _docScale;
			//_view.DocSize = _doc.Size;
		}

		private void EhScalingModeChanged()
		{
			_doc.IsSizeCalculationBasedOnSourceSize = _view.IsSizeCalculationBasedOnSourceSize; // false if Size should be used directly, true if the Scale should be used

			_locationController.ShowSizeElements(true, !_doc.IsSizeCalculationBasedOnSourceSize);
			_locationController.ShowScaleElements(true, _doc.IsSizeCalculationBasedOnSourceSize);
		}

		private void EhLocController_SizeXChanged(RADouble sizeX)
		{
			if (!_doc.IsSizeCalculationBasedOnSourceSize)
			{
				var sizeY = _docLocation.SizeY;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					_docLocation.SizeY = sizeX * (_srcSize.Y / _srcSize.X);
				}
				_docLocation.SizeX = sizeX;
				_docScale = new PointD2D(_docLocation.AbsoluteSizeX / _srcSize.X, _docLocation.AbsoluteSizeY / _srcSize.Y);
				_locationController.ScaleX = _docScale.X;
				_locationController.ScaleY = _docScale.Y;
				_locationController.SizeY = _docLocation.SizeY;
			}
		}

		private void EhLocController_SizeYChanged(RADouble sizeY)
		{
			if (!_doc.IsSizeCalculationBasedOnSourceSize)
			{
				var sizeX = _docLocation.SizeX;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
				{
					_docLocation.SizeX = sizeY * (_srcSize.X / _srcSize.Y);
				}
				_docLocation.SizeY = sizeY;
				_docScale = new PointD2D(_docLocation.AbsoluteSizeX / _srcSize.X, _docLocation.AbsoluteSizeY / _srcSize.Y);
				_locationController.ScaleX = _docScale.X;
				_locationController.ScaleY = _docScale.Y;
				_locationController.SizeX = _docLocation.SizeX;
			}
		}

		private void EhLocController_ScaleXChanged(double scaleX)
		{
			_docLocation.ScaleX = scaleX;
			if (_doc.IsSizeCalculationBasedOnSourceSize)
			{
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					_docLocation.ScaleY = scaleX;
				}
				var size = new PointD2D(_srcSize.X * _docLocation.ScaleX, _srcSize.Y * _docLocation.ScaleY);
				_docLocation.AbsoluteSize = size;

				_locationController.ScaleY = _docLocation.ScaleY;
				_locationController.SizeX = _docLocation.SizeX;
				_locationController.SizeY = _docLocation.SizeY;
			}
		}

		private void EhLocController_ScaleYChanged(double scaleY)
		{
			_docLocation.ScaleY = scaleY;
			if (_doc.IsSizeCalculationBasedOnSourceSize)
			{
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					_docLocation.ScaleX = scaleY;
				}
				var size = new PointD2D(_srcSize.X * _docLocation.ScaleX, _srcSize.Y * _docLocation.ScaleY);
				_docLocation.AbsoluteSize = size;

				_locationController.ScaleX = _docLocation.ScaleX;
				_locationController.SizeX = _docLocation.SizeX;
				_locationController.SizeY = _docLocation.SizeY;
			}
		}
	}
}