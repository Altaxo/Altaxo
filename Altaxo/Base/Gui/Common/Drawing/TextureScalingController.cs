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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.Drawing
{
	public interface ITextureScalingView
	{
		TextureScalingMode ScalingMode { get; set; }

		event Action ScalingModeChanged;

		AspectRatioPreservingMode AspectPreserving { get; set; }

		event Action AspectPreservingChanged;

		double XScale { get; set; }

		double YScale { get; set; }

		double XSize { get; set; }

		double YSize { get; set; }

		event Action XChanged;

		event Action YChanged;

		bool ShowSizeNotScale { set; }
	}

	public class TextureScalingController : MVCANDControllerBase<TextureScaling, ITextureScalingView>
	{
		/// <summary>
		/// Size of the original texture.
		/// </summary>
		private PointD2D? _sourceTextureSize;

		/// <summary>Sets the size of the source texture. This is used by the view to automatically change the value of one size/scale when the value for the other size/scale is changed.</summary>
		/// <value>The size of the source texture.</value>
		public PointD2D SourceTextureSize
		{
			set
			{
				_sourceTextureSize = value;
			}
		}

		protected override void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.ScalingMode = _doc.ScalingMode;
				_view.AspectPreserving = _doc.SourceAspectRatioPreserving;
				if (_doc.ScalingMode == TextureScalingMode.Absolute)
				{
					_view.XSize = _doc.X;
					_view.YSize = _doc.Y;
					_view.XScale = 1;
					_view.YScale = 1;
					_view.ShowSizeNotScale = true;
				}
				else
				{
					_view.XScale = _doc.X;
					_view.YScale = _doc.Y;
					_view.XSize = 10;
					_view.YSize = 10;
					_view.ShowSizeNotScale = false;
				}
			}
		}

		protected override void AttachView()
		{
			_view.ScalingModeChanged += EhScalingModeChanged;
			_view.AspectPreservingChanged += EhAspectPreservingChanged;
			_view.XChanged += EhXChanged;
			_view.YChanged += EhYChanged;

			base.AttachView();
		}

		protected override void DetachView()
		{
			_view.ScalingModeChanged -= EhScalingModeChanged;
			_view.AspectPreservingChanged -= EhAspectPreservingChanged;
			_view.XChanged -= EhXChanged;
			_view.YChanged -= EhYChanged;

			base.DetachView();
		}

		public override bool Apply(bool disposeController)
		{
			_originalDoc = _doc;
			return true;
		}

		private void EhScalingModeChanged()
		{
			_doc.ScalingMode = _view.ScalingMode;
			using (var supp = _suppressDirtyEvent.SuspendGetToken())
			{
				EhXChanged();
				EhYChanged();
				_view.ShowSizeNotScale = (_doc.ScalingMode == TextureScalingMode.Absolute);
			}
			OnMadeDirty();
		}

		private void EhAspectPreservingChanged()
		{
			_doc.SourceAspectRatioPreserving = _view.AspectPreserving;

			if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
			{
				if (_doc.ScalingMode == TextureScalingMode.Absolute)
				{
					if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority && null != _sourceTextureSize)
						_view.YSize = _doc.Y = _doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X;
					else
						_view.XSize = _doc.X = _doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y;
				}
				else
				{
					if (_doc.SourceAspectRatioPreserving == AspectRatioPreservingMode.PreserveXPriority)
						_view.YScale = _doc.Y = _doc.X;
					else
						_view.XScale = _doc.X = _doc.Y;
				}
			}

			OnMadeDirty();
		}

		private void EhXChanged()
		{
			if (_doc.ScalingMode == TextureScalingMode.Absolute)
			{
				_doc.X = _view.XSize;
				if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && null != _sourceTextureSize)
				{
					_doc.Y = _doc.X * _sourceTextureSize.Value.Y / _sourceTextureSize.Value.X;
					_view.YSize = _doc.Y;
				}
			}
			else
			{
				_doc.X = _view.XScale;
				if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
				{
					_view.YScale = _doc.Y = _doc.X;
				}
			}

			OnMadeDirty();
		}

		private void EhYChanged()
		{
			if (_doc.ScalingMode == TextureScalingMode.Absolute)
			{
				_doc.Y = _view.YSize;
				if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None && null != _sourceTextureSize)
				{
					_doc.X = _doc.Y * _sourceTextureSize.Value.X / _sourceTextureSize.Value.Y;
					_view.XSize = _doc.X;
				}
			}
			else
			{
				_doc.Y = _view.YScale;
				if (_doc.SourceAspectRatioPreserving != AspectRatioPreservingMode.None)
				{
					_view.XScale = _doc.X = _doc.Y;
				}
			}

			OnMadeDirty();
		}
	}
}