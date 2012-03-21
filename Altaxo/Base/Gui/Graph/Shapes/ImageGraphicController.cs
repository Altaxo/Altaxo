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
using System.Text;

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IImageGraphicView
	{
		PointD2D DocPosition { get; set; }
		PointD2D DocSize { get; set; }
		PointD2D SourceSize { set; }
		PointD2D DocScale { get; set; }
		double DocRotation { get; set; }
		double DocShear { get; set; }
		Altaxo.Graph.AspectRatioPreservingMode AspectPreserving { get; set; }
		bool IsSizeCalculationBasedOnSourceSize { get; set; }

		event Action AspectPreservingChanged;
		event Action ScalingModeChanged;

		event Action ScaleXChanged;
		event Action ScaleYChanged;
		event Action SizeXChanged;
		event Action SizeYChanged;

	}

	[UserControllerForObject(typeof(ImageGraphic))]
	[ExpectedTypeOfView(typeof(IImageGraphicView))]
	public class ImageGraphicController : MVCANControllerBase<ImageGraphic, IImageGraphicView>
	{
		PointD2D _srcSize;
		PointD2D _docScale;
		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_doc.NormalizeToScaleOne();
				_srcSize = _doc.GetImageSizePt();
				_docScale = new PointD2D(_doc.Size.X / _srcSize.X, _doc.Size.Y / _srcSize.Y);
			}
			if (_view != null)
			{
				_view.DocPosition = _doc.Position;
				_view.DocRotation = _doc.Rotation;
				_view.DocShear = _doc.Shear;

				_view.SourceSize = _srcSize;
				_view.DocSize = _doc.Size;
				_view.DocScale = _docScale;
				_view.AspectPreserving = _doc.AspectRatioPreserving;
				_view.IsSizeCalculationBasedOnSourceSize = _doc.IsSizeCalculationBasedOnSourceSize;
			}
		}

		public override bool Apply()
		{

			_doc.Position = _view.DocPosition;
			_doc.Rotation = _view.DocRotation;
			_doc.Shear = _view.DocShear;

			// all other properties where already set during the session


			if (!object.ReferenceEquals(_originalDoc, _doc))
				_originalDoc.CopyFrom(_doc);


			return true;
		}


		protected override void AttachView()
		{
			_view.AspectPreservingChanged += EhAspectRatioPreservingChanged;
			_view.ScalingModeChanged += EhScalingModeChanged;

			_view.ScaleXChanged += EhScaleXChanged;
			_view.ScaleYChanged += EhScaleYChanged;
			_view.SizeXChanged += EhSizeXChanged;
			_view.SizeYChanged += EhSizeYChanged;
		}

		protected override void DetachView()
		{
			_view.AspectPreservingChanged -= EhAspectRatioPreservingChanged;
			_view.ScalingModeChanged -= EhScalingModeChanged;

			_view.ScaleXChanged -= EhScaleXChanged;
			_view.ScaleYChanged -= EhScaleYChanged;
			_view.SizeXChanged -= EhSizeXChanged;
			_view.SizeYChanged -= EhSizeYChanged;
		}

		void EhAspectRatioPreservingChanged()
		{
			_doc.AspectRatioPreserving = _view.AspectPreserving;
			_docScale = new PointD2D(_doc.Size.X / _srcSize.X, _doc.Size.Y / _srcSize.Y);
			_view.DocScale = _docScale;
			_view.DocSize = _doc.Size;
		}




		void EhScalingModeChanged()
		{
			_doc.IsSizeCalculationBasedOnSourceSize = _view.IsSizeCalculationBasedOnSourceSize;
		}


		void EhScaleXChanged()
		{
			if (_doc.IsSizeCalculationBasedOnSourceSize)
			{
				_docScale.X = _view.DocScale.X;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					_docScale.Y = _docScale.X;
				}
				_doc.Size = new PointD2D(_srcSize.X * _docScale.X, _srcSize.Y * _docScale.Y);
				_view.DocScale = _docScale;
				_view.DocSize = _doc.Size;
			}
		}

		void EhScaleYChanged()
		{
			if (_doc.IsSizeCalculationBasedOnSourceSize)
			{
				_docScale.Y = _view.DocScale.Y;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					_docScale.X = _docScale.Y;
				}
				_doc.Size = new PointD2D(_srcSize.X * _docScale.X, _srcSize.Y * _docScale.Y);
				_view.DocScale = _docScale;
				_view.DocSize = _doc.Size;
			}
		}

		void EhSizeXChanged()
		{
			if (!_doc.IsSizeCalculationBasedOnSourceSize)
			{
				PointD2D size = _view.DocSize;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					size.Y = size.X * _srcSize.Y / _srcSize.X;
				}
				_doc.Size = size;
				size = _doc.Size;
				_docScale = new PointD2D(size.X / _srcSize.X, size.Y / _srcSize.Y);
				_view.DocScale = _docScale;
				_view.DocSize = size;

			}
		}

		void EhSizeYChanged()
		{
			if (!_doc.IsSizeCalculationBasedOnSourceSize)
			{
				PointD2D size = _view.DocSize;
				if (_doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority || _doc.AspectRatioPreserving == AspectRatioPreservingMode.PreserveYPriority)
				{
					size.X = size.Y * _srcSize.X / _srcSize.Y;
				}
				_doc.Size = size;
				size = _doc.Size;
				_docScale = new PointD2D(size.X / _srcSize.X, size.Y / _srcSize.Y);
				_view.DocScale = _docScale;
				_view.DocSize = size;

			}
		}

	}
}
